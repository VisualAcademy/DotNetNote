using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Azunt.Web.Infrastructures.All;

public class TenantSchemaEnhancerEnsureAllowedIpRangesTable
{
    private readonly string _masterConnectionString;
    private readonly ILogger<TenantSchemaEnhancerEnsureAllowedIpRangesTable> _logger;

    public TenantSchemaEnhancerEnsureAllowedIpRangesTable(
        string masterConnectionString,
        ILogger<TenantSchemaEnhancerEnsureAllowedIpRangesTable> logger)
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
                EnsureAllowedIpRangesTable(connStr);
                _logger.LogInformation($"AllowedIpRanges table processed (tenant DB): {connStr}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{connStr}] Error processing AllowedIpRanges table");
            }
        }
    }

    public void EnhanceMasterDatabase()
    {
        try
        {
            EnsureAllowedIpRangesTable(_masterConnectionString);
            _logger.LogInformation("AllowedIpRanges table processed (master DB)");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing master DB AllowedIpRanges table");
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

    private void EnsureAllowedIpRangesTable(string connectionString)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            var cmdCheck = new SqlCommand(@"
                SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_NAME = 'AllowedIpRanges'", connection);

            int tableCount = (int)cmdCheck.ExecuteScalar();

            if (tableCount == 0)
            {
                var cmdCreate = new SqlCommand(@"
                    CREATE TABLE [dbo].[AllowedIpRanges] (
                        [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
                        [StartIpRange] VARCHAR(15),
                        [EndIpRange] VARCHAR(15),
                        [Description] NVARCHAR(MAX),
                        [CreateDate] DATETIME DEFAULT (GETDATE()),
                        [TenantId] BIGINT
                    )", connection);

                cmdCreate.ExecuteNonQuery();

                _logger.LogInformation("AllowedIpRanges table created.");
            }
            else
            {
                EnsureAllowedIpRangesColumns(connection);
            }

            EnsureDefaultAllowedIpRanges(connection);
        }
    }

    private void EnsureAllowedIpRangesColumns(SqlConnection connection)
    {
        var expectedColumns = new Dictionary<string, string>
        {
            ["StartIpRange"] = "VARCHAR(15)",
            ["EndIpRange"] = "VARCHAR(15)",
            ["Description"] = "NVARCHAR(MAX)",
            ["CreateDate"] = "DATETIME DEFAULT (GETDATE())",
            ["TenantId"] = "BIGINT"
        };

        foreach (var kvp in expectedColumns)
        {
            var columnName = kvp.Key;
            var columnType = kvp.Value;

            var cmdColumnCheck = new SqlCommand(@"
                SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = 'AllowedIpRanges' AND COLUMN_NAME = @ColumnName", connection);
            cmdColumnCheck.Parameters.AddWithValue("@ColumnName", columnName);

            int colExists = (int)cmdColumnCheck.ExecuteScalar();

            if (colExists == 0)
            {
                var alterCmd = new SqlCommand(
                    $"ALTER TABLE [dbo].[AllowedIpRanges] ADD [{columnName}] {columnType}", connection);
                alterCmd.ExecuteNonQuery();

                _logger.LogInformation($"Column added to AllowedIpRanges: {columnName} ({columnType})");
            }
        }
    }

    private void EnsureDefaultAllowedIpRanges(SqlConnection connection)
    {
        var cmdRowCount = new SqlCommand("SELECT COUNT(*) FROM [dbo].[AllowedIpRanges]", connection);
        int rowCount = (int)cmdRowCount.ExecuteScalar();

        if (rowCount > 0)
        {
            _logger.LogInformation("AllowedIpRanges table already contains data. Skipping default insert.");
            return;
        }

        var cmdInsertDefaults = new SqlCommand(@"
            INSERT INTO [dbo].[AllowedIpRanges] (StartIpRange, EndIpRange, Description, TenantId)
            VALUES 
                ('127.0.0.1', '127.0.0.1', N'Default Local Network', 1),
                ('192.168.1.1', '192.168.1.255', N'Default Local Network', 1),
                ('10.0.0.1', '10.0.0.255', N'Default Internal Network', 1)", connection);

        int inserted = cmdInsertDefaults.ExecuteNonQuery();
        _logger.LogInformation($"AllowedIpRanges 기본 데이터 {inserted}건 삽입 완료");
    }

    public static void Run(IServiceProvider services, bool forMaster)
    {
        try
        {
            var logger = services.GetRequiredService<ILogger<TenantSchemaEnhancerEnsureAllowedIpRangesTable>>();
            var config = services.GetRequiredService<IConfiguration>();
            var masterConnectionString = config.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(masterConnectionString))
            {
                throw new InvalidOperationException("DefaultConnection is not configured in appsettings.json.");
            }

            var enhancer = new TenantSchemaEnhancerEnsureAllowedIpRangesTable(masterConnectionString, logger);

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
            var fallbackLogger = services.GetService<ILogger<TenantSchemaEnhancerEnsureAllowedIpRangesTable>>();
            fallbackLogger?.LogError(ex, "Error while processing AllowedIpRanges table.");
        }
    }
}
