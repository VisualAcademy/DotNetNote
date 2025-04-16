namespace Azunt.Infrastructures.Auth;

public class TenantSchemaEnhancerEnsureUsersTable
{
    private readonly string _masterConnectionString;
    private readonly ILogger<TenantSchemaEnhancerEnsureUsersTable> _logger;

    public TenantSchemaEnhancerEnsureUsersTable(
        string masterConnectionString,
        ILogger<TenantSchemaEnhancerEnsureUsersTable> logger)
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
                EnsureUsersTable(connStr);
                _logger.LogInformation($"AspNetUsers table processed (tenant DB): {connStr}");
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
            EnsureUsersTable(_masterConnectionString);
            _logger.LogInformation("AspNetUsers table processed (master DB)");
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

    private void EnsureUsersTable(string connectionString)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // 테이블 존재 여부 확인
            var cmdCheckTable = new SqlCommand(@"
                    SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_NAME = 'AspNetUsers'", connection);

            int tableExists = (int)cmdCheckTable.ExecuteScalar();

            if (tableExists == 0)
            {
                // 테이블 생성
                var createCmd = new SqlCommand(@"
                        CREATE TABLE [dbo].[AspNetUsers] (
                            [Id] NVARCHAR(450) NOT NULL PRIMARY KEY,
                            [UserName] NVARCHAR(256) NULL,
                            [NormalizedUserName] NVARCHAR(256) NULL,
                            [Email] NVARCHAR(256) NULL,
                            [NormalizedEmail] NVARCHAR(256) NULL,
                            [EmailConfirmed] BIT NOT NULL,
                            [PasswordHash] NVARCHAR(MAX) NULL,
                            [SecurityStamp] NVARCHAR(MAX) NULL,
                            [ConcurrencyStamp] NVARCHAR(MAX) NULL,
                            [PhoneNumber] NVARCHAR(MAX) NULL,
                            [PhoneNumberConfirmed] BIT NOT NULL,
                            [TwoFactorEnabled] BIT NOT NULL,
                            [LockoutEnd] DATETIMEOFFSET(7) NULL,
                            [LockoutEnabled] BIT NOT NULL,
                            [AccessFailedCount] INT NOT NULL,
                            [Address] NVARCHAR(MAX) NULL,
                            [FirstName] NVARCHAR(MAX) NULL,
                            [LastName] NVARCHAR(MAX) NULL,
                            [Timezone] NVARCHAR(MAX) NULL,
                            [TenantName] NVARCHAR(MAX) DEFAULT 'Azunt',
                            [RegistrationDate] DATETIMEOFFSET NULL DEFAULT SYSDATETIMEOFFSET(),
                            [ShowInDropdown] BIT NULL DEFAULT 0,
                            [RefreshToken] NVARCHAR(MAX) NULL,
                            [RefreshTokenExpiryTime] DATETIME NULL,
                            [DivisionId] BIGINT NULL DEFAULT 0,
                            [TenantId] BIGINT NOT NULL DEFAULT CONVERT(BIGINT, 0)
                        );

                        CREATE NONCLUSTERED INDEX [EmailIndex]
                        ON [dbo].[AspNetUsers]([NormalizedEmail] ASC);

                        CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex]
                        ON [dbo].[AspNetUsers]([NormalizedUserName] ASC) 
                        WHERE ([NormalizedUserName] IS NOT NULL);
                    ", connection);

                createCmd.ExecuteNonQuery();
                _logger.LogInformation("AspNetUsers table created.");
            }
            else
            {
                // 필수 컬럼 존재 여부 및 추가
                var expectedColumns = new Dictionary<string, string>
                {
                    ["Address"] = "NVARCHAR(MAX) NULL",
                    ["FirstName"] = "NVARCHAR(MAX) NULL",
                    ["LastName"] = "NVARCHAR(MAX) NULL",
                    ["Timezone"] = "NVARCHAR(MAX) NULL",
                    ["TenantName"] = "NVARCHAR(MAX) DEFAULT 'Azunt'",
                    ["RegistrationDate"] = "DATETIMEOFFSET NULL DEFAULT SYSDATETIMEOFFSET()",
                    ["ShowInDropdown"] = "BIT NULL DEFAULT 0",
                    ["RefreshToken"] = "NVARCHAR(MAX) NULL",
                    ["RefreshTokenExpiryTime"] = "DATETIME NULL",
                    ["DivisionId"] = "BIGINT NULL DEFAULT 0",
                    ["TenantId"] = "BIGINT NOT NULL DEFAULT CONVERT(BIGINT, 0)"
                };

                foreach (var kvp in expectedColumns)
                {
                    var columnName = kvp.Key;
                    var columnType = kvp.Value;

                    var cmdCheckColumn = new SqlCommand(@"
                            SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                            WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME = @ColumnName", connection);
                    cmdCheckColumn.Parameters.AddWithValue("@ColumnName", columnName);

                    int columnExists = (int)cmdCheckColumn.ExecuteScalar();

                    if (columnExists == 0)
                    {
                        var alterCmd = new SqlCommand(
                            $"ALTER TABLE [dbo].[AspNetUsers] ADD [{columnName}] {columnType}", connection);
                        alterCmd.ExecuteNonQuery();

                        _logger.LogInformation($"Column added: {columnName} ({columnType})");
                    }
                }
            }

            // 기본 유저는 DbContext 기반으로 생성하므로 이 클래스에서는 테이블과 컬럼만 책임집니다.
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
            var logger = services.GetRequiredService<ILogger<TenantSchemaEnhancerEnsureUsersTable>>();
            var config = services.GetRequiredService<IConfiguration>();
            var masterConnectionString = config.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(masterConnectionString))
            {
                throw new InvalidOperationException("Master connection string 'DefaultConnection' is not configured.");
            }

            var enhancer = new TenantSchemaEnhancerEnsureUsersTable(masterConnectionString, logger);

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
            var fallbackLogger = services.GetService<ILogger<TenantSchemaEnhancerEnsureUsersTable>>();
            fallbackLogger?.LogError(ex, "Error while processing AspNetUsers table.");
        }
    }
}
