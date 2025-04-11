using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Azunt.Web.Infrastructures.Assets.Tenants;

public class TenantSchemaEnhancerEnsureProjectsMachinesTable
{
    private readonly string _masterConnectionString;
    private readonly ILogger<TenantSchemaEnhancerEnsureProjectsMachinesTable> _logger;

    public TenantSchemaEnhancerEnsureProjectsMachinesTable(
        string masterConnectionString,
        ILogger<TenantSchemaEnhancerEnsureProjectsMachinesTable> logger)
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
                EnsureProjectsMachinesTable(connStr);
                _logger.LogInformation($"ProjectsMachines table processed (tenant DB): {connStr}");
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
            EnsureProjectsMachinesTable(_masterConnectionString);
            _logger.LogInformation("ProjectsMachines table processed (master DB)");
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

    private void EnsureProjectsMachinesTable(string connectionString)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            var cmdCheck = new SqlCommand(@"
                SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_NAME = 'ProjectsMachines'", connection);

            int tableCount = (int)cmdCheck.ExecuteScalar();

            if (tableCount == 0)
            {
                var cmdCreate = new SqlCommand(@"
                    CREATE TABLE [dbo].[ProjectsMachines] (
                        [Id] INT NOT NULL PRIMARY KEY Identity(1, 1),
                        ProjectId Int Null,
                        ParentId Int Null,

                        Name NVarChar(255) Not Null,
                        Status NVarChar(255) Null,
                        Code NVarChar(255) Null,

                        Serial NVarChar(255) Null,
                        Seal NVarChar(255) Null,
                        DateManufactured NVarChar(450) Null, 
                        PurchaseOrder NVarChar(255) Null,
                        ModelNumber NVarChar(255) Null,

                        ManufacturerId Int Null,
                        ManufacturerName NVarChar(255) Null,
                        Casino NVarChar(255) Null,
                        License NVarChar(255) Null,
                        MachineTypeId Int Null,
                        DateOnFloor DateTime Null,
                        Bank NVarChar(255) Null,

                        Content NVarChar(Max) Null,
                        AssetLocation NVarChar(255) Null,
                        AssetNumber NVarChar(255) Null,
                        Denomination NVarChar(255) Null,
                        SlotType NVarChar(255) Null,
                        ProgressiveType NVarChar(255) Null,

                        CreatedBy NVarChar(255) Null,
                        Created DateTime Default(GetDate()) Null,
                        ModifiedBy NVarChar(255) Null,
                        Modified DateTime Null
                    )", connection);

                cmdCreate.ExecuteNonQuery();

                _logger.LogInformation("ProjectsMachines table created.");
            }
            else
            {
                var expectedColumns = new Dictionary<string, string>
                {
                    ["ProjectId"] = "Int Null",
                    ["ParentId"] = "Int Null",
                    ["Name"] = "NVarChar(255) Not Null",
                    ["Status"] = "NVarChar(255) Null",
                    ["Code"] = "NVarChar(255) Null",
                    ["Serial"] = "NVarChar(255) Null",
                    ["Seal"] = "NVarChar(255) Null",
                    ["DateManufactured"] = "NVarChar(450) Null",
                    ["PurchaseOrder"] = "NVarChar(255) Null",
                    ["ModelNumber"] = "NVarChar(255) Null",
                    ["ManufacturerId"] = "Int Null",
                    ["ManufacturerName"] = "NVarChar(255) Null",
                    ["Casino"] = "NVarChar(255) Null",
                    ["License"] = "NVarChar(255) Null",
                    ["MachineTypeId"] = "Int Null",
                    ["DateOnFloor"] = "DateTime Null",
                    ["Bank"] = "NVarChar(255) Null",
                    ["Content"] = "NVarChar(Max) Null",
                    ["AssetLocation"] = "NVarChar(255) Null",
                    ["AssetNumber"] = "NVarChar(255) Null",
                    ["Denomination"] = "NVarChar(255) Null",
                    ["SlotType"] = "NVarChar(255) Null",
                    ["ProgressiveType"] = "NVarChar(255) Null",
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
                        WHERE TABLE_NAME = 'ProjectsMachines' AND COLUMN_NAME = @ColumnName", connection);
                    cmdColumnCheck.Parameters.AddWithValue("@ColumnName", columnName);

                    int colExists = (int)cmdColumnCheck.ExecuteScalar();

                    if (colExists == 0)
                    {
                        var alterCmd = new SqlCommand(
                            $"ALTER TABLE [dbo].[ProjectsMachines] ADD [{columnName}] {columnType}", connection);
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
            var logger = services.GetRequiredService<ILogger<TenantSchemaEnhancerEnsureProjectsMachinesTable>>();
            var config = services.GetRequiredService<IConfiguration>();
            var masterConnectionString = config.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(masterConnectionString))
            {
                throw new InvalidOperationException("DefaultConnection is not configured in appsettings.json.");
            }

            var enhancer = new TenantSchemaEnhancerEnsureProjectsMachinesTable(masterConnectionString, logger);

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
            var fallbackLogger = services.GetService<ILogger<TenantSchemaEnhancerEnsureProjectsMachinesTable>>();
            fallbackLogger?.LogError(ex, "Error while processing ProjectsMachines table.");
        }
    }
}
