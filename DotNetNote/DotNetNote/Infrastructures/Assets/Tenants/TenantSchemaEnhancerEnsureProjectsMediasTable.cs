using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Azunt.Web.Infrastructures.Assets.Tenants;

public class TenantSchemaEnhancerEnsureProjectsMediasTable
{
    private readonly string _masterConnectionString;
    private readonly ILogger<TenantSchemaEnhancerEnsureProjectsMediasTable> _logger;

    public TenantSchemaEnhancerEnsureProjectsMediasTable(
        string masterConnectionString,
        ILogger<TenantSchemaEnhancerEnsureProjectsMediasTable> logger)
    {
        _masterConnectionString = masterConnectionString;
        _logger = logger;
    }

    public void EnhanceTenantDatabases()
    {
        var tenantConnectionStrings = GetTenantConnectionStrings();

        foreach (var connStr in tenantConnectionStrings)
        {
            try
            {
                EnsureProjectsMediasTable(connStr);
                _logger.LogInformation($"ProjectsMedias table processed (tenant DB): {connStr}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{connStr}] Error processing tenant DB");
            }
        }
    }

    public void EnhanceMasterDatabase()
    {
        try
        {
            EnsureProjectsMediasTable(_masterConnectionString);
            _logger.LogInformation("ProjectsMedias table processed (master DB)");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing master DB");
        }
    }

    private List<string> GetTenantConnectionStrings()
    {
        var result = new List<string>();

        using (var connection = new SqlConnection(_masterConnectionString))
        {
            connection.Open();
            var cmd = new SqlCommand("SELECT ConnectionString FROM dbo.Tenants", connection);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var connectionString = reader["ConnectionString"]?.ToString();
                    if (!string.IsNullOrEmpty(connectionString))
                    {
                        result.Add(connectionString);
                    }
                }
            }
        }

        return result;
    }

    private void EnsureProjectsMediasTable(string connectionString)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            var cmdCheck = new SqlCommand(@"
                SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_NAME = 'ProjectsMedias'", connection);

            int tableCount = (int)cmdCheck.ExecuteScalar();

            if (tableCount == 0)
            {
                var cmdCreate = new SqlCommand(@"
                    CREATE TABLE [dbo].[ProjectsMedias] (
                        [Id] INT NOT NULL PRIMARY KEY Identity(1, 1),
                        ProjectId Int Null,
                        ParentId Int Null,
                        Status NVarChar(255) Null,
                        Code NVarChar(255) Null,
                        ComponentId NVarChar(255) NULL,
                        ManufacturerId Int Null,
                        ManufacturerName NVarChar(255) Null,
                        Theme NVarChar(255) Null,

                        --<Excel Upload>
                        Name NVarChar(255) Not Null,
                        Version NVarChar(255) Null,
                        PublishingCompany NVarChar(255) Null, 
                        Hash NVarChar(255) Null,
                        PurchaseOrder NVarChar(255) Null,
                        --</Excel Upload>

                        DateReceived DateTime NULL,
                        DateTested DateTime NULL,
                        DateReleased DateTime NULL,

                        Content NVarChar(Max) Null,

                        [CreatedBy] NVarChar(255) Null,
                        [Created] DateTime Default(GetDate()) Null,
                        [ModifiedBy] NVarChar(255) Null,
                        [Modified] DateTime Null
                    )", connection);

                cmdCreate.ExecuteNonQuery();

                _logger.LogInformation("ProjectsMedias table created.");
            }
            else
            {
                var expectedColumns = new Dictionary<string, string>
                {
                    ["ProjectId"] = "Int Null",
                    ["ParentId"] = "Int Null",
                    ["Status"] = "NVarChar(255) Null",
                    ["Code"] = "NVarChar(255) Null",
                    ["ComponentId"] = "NVarChar(255) Null",
                    ["ManufacturerId"] = "Int Null",
                    ["ManufacturerName"] = "NVarChar(255) Null",
                    ["Theme"] = "NVarChar(255) Null",
                    ["Name"] = "NVarChar(255) Not Null",
                    ["Version"] = "NVarChar(255) Null",
                    ["PublishingCompany"] = "NVarChar(255) Null",
                    ["Hash"] = "NVarChar(255) Null",
                    ["PurchaseOrder"] = "NVarChar(255) Null",
                    ["DateReceived"] = "DateTime Null",
                    ["DateTested"] = "DateTime Null",
                    ["DateReleased"] = "DateTime Null",
                    ["Content"] = "NVarChar(Max) Null",
                    ["CreatedBy"] = "NVarChar(255) Null",
                    ["Created"] = "DateTime Null",
                    ["ModifiedBy"] = "NVarChar(255) Null",
                    ["Modified"] = "DateTime Null"
                };

                foreach (var kvp in expectedColumns)
                {
                    var columnName = kvp.Key;
                    var columnType = kvp.Value;

                    var cmdColumnCheck = new SqlCommand(@"
                        SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'ProjectsMedias' AND COLUMN_NAME = @ColumnName", connection);
                    cmdColumnCheck.Parameters.AddWithValue("@ColumnName", columnName);

                    int colExists = (int)cmdColumnCheck.ExecuteScalar();

                    if (colExists == 0)
                    {
                        var alterCmd = new SqlCommand(
                            $"ALTER TABLE [dbo].[ProjectsMedias] ADD [{columnName}] {columnType}", connection);
                        alterCmd.ExecuteNonQuery();

                        _logger.LogInformation($"Column added: {columnName} ({columnType})");
                    }
                }
            }
        }
    }

    /// <summary>
    /// Entry point to run from Program.cs or Startup.cs
    /// forMaster == true: only master DB
    /// forMaster == false: only tenant DBs
    /// </summary>
    public static void Run(IServiceProvider services, bool forMaster)
    {
        try
        {
            var logger = services.GetRequiredService<ILogger<TenantSchemaEnhancerEnsureProjectsMediasTable>>();
            var config = services.GetRequiredService<IConfiguration>();
            var masterConnectionString = config.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(masterConnectionString))
            {
                throw new InvalidOperationException("DefaultConnection is not configured in appsettings.json.");
            }

            var enhancer = new TenantSchemaEnhancerEnsureProjectsMediasTable(masterConnectionString, logger);

            if (forMaster)
            {
                enhancer.EnhanceMasterDatabase();
            }
            else
            {
                enhancer.EnhanceTenantDatabases();
            }
        }
        catch (Exception ex)
        {
            var fallbackLogger = services.GetService<ILogger<TenantSchemaEnhancerEnsureProjectsMediasTable>>();
            fallbackLogger?.LogError(ex, "Error while processing ProjectsMedias table.");
        }
    }
}
