using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace Azunt.Infrastructures.Tenants;

public class TenantSchemaEnhancerEnsureVendorLicenseStatusesTable
{
    private readonly string _masterConnectionString;
    private readonly ILogger<TenantSchemaEnhancerEnsureVendorLicenseStatusesTable> _logger;

    public TenantSchemaEnhancerEnsureVendorLicenseStatusesTable(
        string masterConnectionString,
        ILogger<TenantSchemaEnhancerEnsureVendorLicenseStatusesTable> logger)
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
                EnsureLicenseStatusesTable(connStr);
                _logger.LogInformation($"LicenseStatuses table processed (tenant DB): {connStr}");
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
            EnsureLicenseStatusesTable(_masterConnectionString);
            _logger.LogInformation("LicenseStatuses table processed (master DB)");
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

    private void EnsureLicenseStatusesTable(string connectionString)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            var cmdCheck = new SqlCommand(@"
                SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_NAME = 'LicenseStatuses'", connection);

            int tableExists = (int)cmdCheck.ExecuteScalar();

            if (tableExists == 0)
            {
                var cmdCreate = new SqlCommand(@"
                    CREATE TABLE [dbo].[LicenseStatuses] (
                        [ID] BIGINT IDENTITY(1,1) NOT NULL,
                        [Active] BIT NULL,
                        [CreatedAt] DATETIMEOFFSET(7) NULL,
                        [CreatedBy] NVARCHAR(70) NULL,
                        [Status] NVARCHAR(MAX) NULL,
                        [ApplicantType] INT NULL,
                        CONSTRAINT [PK_LicenseStatuses] PRIMARY KEY CLUSTERED ([ID] ASC)
                    )", connection);

                cmdCreate.ExecuteNonQuery();
                _logger.LogInformation("LicenseStatuses table created.");
            }
            else
            {
                var expectedColumns = new Dictionary<string, string>
                {
                    ["Active"] = "BIT NULL",
                    ["CreatedAt"] = "DATETIMEOFFSET(7) NULL",
                    ["CreatedBy"] = "NVARCHAR(70) NULL",
                    ["Status"] = "NVARCHAR(MAX) NULL",
                    ["ApplicantType"] = "INT NULL"
                };

                foreach (var kvp in expectedColumns)
                {
                    var columnName = kvp.Key;
                    var columnType = kvp.Value;

                    var cmdColumnCheck = new SqlCommand(@"
                        SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'LicenseStatuses' AND COLUMN_NAME = @ColumnName", connection);

                    cmdColumnCheck.Parameters.AddWithValue("@ColumnName", columnName);

                    int colExists = (int)cmdColumnCheck.ExecuteScalar();

                    if (colExists == 0)
                    {
                        var alterCmd = new SqlCommand(
                            $"ALTER TABLE [dbo].[LicenseStatuses] ADD [{columnName}] {columnType}", connection);

                        alterCmd.ExecuteNonQuery();
                        _logger.LogInformation($"Column added: {columnName} ({columnType})");
                    }
                }
            }

            EnsureDefaultLicenseStatuses(connection);
        }
    }

    private void EnsureDefaultLicenseStatuses(SqlConnection connection)
    {
        var cmdRowCount = new SqlCommand("SELECT COUNT(*) FROM [dbo].[LicenseStatuses]", connection);
        int rowCount = (int)cmdRowCount.ExecuteScalar();

        if (rowCount > 0)
        {
            _logger.LogInformation("LicenseStatuses table already contains data. Skipping default insert.");
            return;
        }

        var defaultStatuses = new List<(int ApplicantType, string Status)>
        {
            // ApplicationType = 1 (Vendor)
            (1, "Active"), (1, "Registered"), (1, "Inactive"), (1, "Denied"),
            (1, "Revoked"), (1, "Suspended"), (1, "Temporary"), (1, "Exempt"),
            (1, "Expired"), (1, "Not Renewed"), (1, "Pending"), (1, "Non-Compliant"),

            // ApplicationType = 2 (VendorEmployee)
            (2, "Active"), (2, "Term"), (2, "Denied"), (2, "Revoked"),
            (2, "Suspended"), (2, "Withdrawn"), (2, "Principal/Owner. Background Check Only"),
            (2, "Expired"), (2, "Not Renewed"), (2, "Pending"), (2, "Non-Compliant"),
            (2, "Not Eligible")
        };

        foreach (var (applicantType, status) in defaultStatuses)
        {
            var cmdInsert = new SqlCommand(@"
                INSERT INTO [dbo].[LicenseStatuses]
                ([Active], [CreatedAt], [CreatedBy], [Status], [ApplicantType])
                VALUES (1, SYSDATETIMEOFFSET(), 'System', @Status, @ApplicantType)", connection);

            cmdInsert.Parameters.AddWithValue("@Status", status);
            cmdInsert.Parameters.AddWithValue("@ApplicantType", applicantType);

            cmdInsert.ExecuteNonQuery();
            _logger.LogInformation($"Default LicenseStatus inserted: {status} (ApplicantType: {applicantType})");
        }
    }

    public static void Run(IServiceProvider services, bool forMaster)
    {
        try
        {
            var logger = services.GetRequiredService<ILogger<TenantSchemaEnhancerEnsureVendorLicenseStatusesTable>>();
            var config = services.GetRequiredService<IConfiguration>();
            var masterConnectionString =
                config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' is not configured.");

            var enhancer = new TenantSchemaEnhancerEnsureVendorLicenseStatusesTable(masterConnectionString, logger);

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
            var fallbackLogger = services.GetService<ILogger<TenantSchemaEnhancerEnsureVendorLicenseStatusesTable>>();
            fallbackLogger?.LogError(ex, "Error while processing LicenseStatuses table.");
        }
    }
}
