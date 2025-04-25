using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Azunt.Infrastructures.Community;

/// <summary>
/// 게시판 아티클에 첨부되는 파일 정보 테이블(AttachFiles)을 생성하는 클래스입니다.
/// </summary>
public class TenantSchemaEnhancerEnsureAttachFilesTable
{
    private readonly string _masterConnectionString;
    private readonly ILogger<TenantSchemaEnhancerEnsureAttachFilesTable> _logger;

    public TenantSchemaEnhancerEnsureAttachFilesTable(
        string masterConnectionString,
        ILogger<TenantSchemaEnhancerEnsureAttachFilesTable> logger)
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
                EnsureAttachFilesTable(connStr);
                _logger.LogInformation($"AttachFiles table processed (tenant DB): {connStr}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{connStr}] Error processing AttachFiles table");
            }
        }
    }

    public void EnhanceMasterDatabase()
    {
        try
        {
            EnsureAttachFilesTable(_masterConnectionString);
            _logger.LogInformation("AttachFiles table processed (master DB)");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing AttachFiles table (master DB)");
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

    private void EnsureAttachFilesTable(string connectionString)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();

        var cmdCheckTable = new SqlCommand(@"
            SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES 
            WHERE TABLE_NAME = 'AttachFiles'", connection);
        var tableExists = (int)cmdCheckTable.ExecuteScalar();

        if (tableExists == 0)
        {
            var createCmd = new SqlCommand(@"
                CREATE TABLE [dbo].[AttachFiles]
                (
                    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
                    [UserId] INT NULL,
                    [BoardId] INT NULL,
                    [ArticleId] INT NULL,
                    [FileName] NVARCHAR(255) NULL,
                    [FileSize] INT NULL,
                    [DownCount] INT NULL DEFAULT 0,
                    [TimeStamp] DATETIMEOFFSET(7) NULL DEFAULT GETDATE()
                );", connection);

            createCmd.ExecuteNonQuery();
            _logger.LogInformation("AttachFiles table created.");
        }
    }

    /// <summary>
    /// Program.cs 또는 Startup.cs에서 호출되는 진입점입니다.
    /// </summary>
    public static void Run(IServiceProvider services, bool forMaster)
    {
        try
        {
            var logger = services.GetRequiredService<ILogger<TenantSchemaEnhancerEnsureAttachFilesTable>>();
            var config = services.GetRequiredService<IConfiguration>();
            var masterConnectionString = config.GetConnectionString("DefaultConnection")!;

            var enhancer = new TenantSchemaEnhancerEnsureAttachFilesTable(masterConnectionString, logger);

            if (forMaster)
                enhancer.EnhanceMasterDatabase();
            else
                enhancer.EnhanceTenantDatabases();
        }
        catch (Exception ex)
        {
            var fallbackLogger = services.GetService<ILogger<TenantSchemaEnhancerEnsureAttachFilesTable>>();
            fallbackLogger?.LogError(ex, "Error while processing AttachFiles table");
        }
    }
}
