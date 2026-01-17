using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Azunt.Web.Infrastructures.Auth;

/// <summary>
/// AspNetUserRoles 테이블을 생성하고 외래 키 및 인덱스를 보장하는 클래스입니다.
/// 이 테이블이 없으면 역할 할당(User-Role 매핑)이 실패하게 됩니다.
/// </summary>
public class TenantSchemaEnhancerEnsureUserRolesTable
{
    private readonly string _masterConnectionString;
    private readonly ILogger<TenantSchemaEnhancerEnsureUserRolesTable> _logger;

    public TenantSchemaEnhancerEnsureUserRolesTable(
        string masterConnectionString,
        ILogger<TenantSchemaEnhancerEnsureUserRolesTable> logger)
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
                EnsureUserRolesTable(connStr);
                _logger.LogInformation($"AspNetUserRoles table processed (tenant DB): {connStr}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{connStr}] Error processing AspNetUserRoles table");
            }
        }
    }

    public void EnhanceMasterDatabase()
    {
        try
        {
            EnsureUserRolesTable(_masterConnectionString);
            _logger.LogInformation("AspNetUserRoles table processed (master DB)");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing AspNetUserRoles table (master DB)");
        }
    }

    private List<string> GetTenantConnectionStrings()
    {
        var result = new List<string>();

        using var connection = new SqlConnection(_masterConnectionString);
        connection.Open();

        var cmd = new SqlCommand("SELECT ConnectionString FROM dbo.Tenants", connection);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            var connStr = reader["ConnectionString"]?.ToString();
            if (!string.IsNullOrEmpty(connStr))
                result.Add(connStr);
        }

        return result;
    }

    private void EnsureUserRolesTable(string connectionString)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();

        var cmdCheckTable = new SqlCommand(@"
            SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_NAME = 'AspNetUserRoles'", connection);
        var tableExists = (int)cmdCheckTable.ExecuteScalar();

        if (tableExists == 0)
        {
            var createCmd = new SqlCommand(@"
                CREATE TABLE [dbo].[AspNetUserRoles] (
                    [UserId] NVARCHAR(450) NOT NULL,
                    [RoleId] NVARCHAR(450) NOT NULL,
                    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
                    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE,
                    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
                );

                CREATE NONCLUSTERED INDEX [IX_AspNetUserRoles_RoleId]
                ON [dbo].[AspNetUserRoles]([RoleId] ASC);
            ", connection);

            createCmd.ExecuteNonQuery();
            _logger.LogInformation("AspNetUserRoles table created.");
        }
    }

    public static void Run(IServiceProvider services, bool forMaster)
    {
        try
        {
            var logger = services.GetRequiredService<ILogger<TenantSchemaEnhancerEnsureUserRolesTable>>();
            var config = services.GetRequiredService<IConfiguration>();
            var masterConnectionString = config.GetConnectionString("DefaultConnection")!;

            var enhancer = new TenantSchemaEnhancerEnsureUserRolesTable(masterConnectionString, logger);

            if (forMaster)
                enhancer.EnhanceMasterDatabase();
            else
                enhancer.EnhanceTenantDatabases();
        }
        catch (Exception ex)
        {
            var fallbackLogger = services.GetService<ILogger<TenantSchemaEnhancerEnsureUserRolesTable>>();
            fallbackLogger?.LogError(ex, "Error while processing AspNetUserRoles table");
        }
    }
}
