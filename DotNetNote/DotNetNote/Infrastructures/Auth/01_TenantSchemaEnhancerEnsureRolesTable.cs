namespace Azunt.Infrastructures.Auth;

public class TenantSchemaEnhancerEnsureRolesTable
{
    private readonly string _masterConnectionString;
    private readonly ILogger<TenantSchemaEnhancerEnsureRolesTable> _logger;

    public TenantSchemaEnhancerEnsureRolesTable(
        string masterConnectionString,
        ILogger<TenantSchemaEnhancerEnsureRolesTable> logger)
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
                EnsureRolesTable(connStr);
                _logger.LogInformation($"AspNetRoles table processed (tenant DB): {connStr}");
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
            EnsureRolesTable(_masterConnectionString);
            _logger.LogInformation("AspNetRoles table processed (master DB)");
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

    private void EnsureRolesTable(string connectionString)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // 테이블 존재 여부 확인
            var cmdCheckTable = new SqlCommand(@"
                    SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_NAME = 'AspNetRoles'", connection);

            int tableExists = (int)cmdCheckTable.ExecuteScalar();

            if (tableExists == 0)
            {
                // 테이블 생성
                var createCmd = new SqlCommand(@"
                        CREATE TABLE [dbo].[AspNetRoles] (
                            [Id] NVARCHAR(450) NOT NULL PRIMARY KEY,
                            [Name] NVARCHAR(256) NULL,
                            [NormalizedName] NVARCHAR(256) NULL,
                            [ConcurrencyStamp] NVARCHAR(MAX) NULL,
                            [Description] NVARCHAR(MAX) NULL
                        );

                        CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex]
                        ON [dbo].[AspNetRoles]([NormalizedName] ASC) 
                        WHERE ([NormalizedName] IS NOT NULL);
                    ", connection);

                createCmd.ExecuteNonQuery();
                _logger.LogInformation("AspNetRoles table created.");
            }
            else
            {
                // Description 컬럼 존재 여부 확인
                var cmdCheckColumn = new SqlCommand(@"
                        SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'AspNetRoles' AND COLUMN_NAME = 'Description'", connection);

                int columnExists = (int)cmdCheckColumn.ExecuteScalar();

                if (columnExists == 0)
                {
                    var alterCmd = new SqlCommand(@"
                            ALTER TABLE [dbo].[AspNetRoles]
                            ADD [Description] NVARCHAR(MAX) NULL", connection);

                    alterCmd.ExecuteNonQuery();
                    _logger.LogInformation("Description column added to AspNetRoles table.");
                }
            }

            EnsureDefaultRoles(connection);
        }
    }

    private void EnsureDefaultRoles(SqlConnection connection)
    {
        var existingRoles = new HashSet<string>();

        var cmdGetRoles = new SqlCommand("SELECT [Name] FROM [dbo].[AspNetRoles]", connection);
        using (var reader = cmdGetRoles.ExecuteReader())
        {
            while (reader.Read())
            {
                existingRoles.Add(reader.GetString(0));
            }
        }

        var defaultRoles = new List<(string Name, string Description)>
        {
            ("Administrators", "응용 프로그램을 총 관리하는 관리 그룹 계정"),
            ("Everyone", "응용 프로그램을 사용하는 모든 사용자 그룹 계정"),
            ("Users", "일반 사용자 그룹 계정"),
            ("Guests", "게스트 사용자 그룹 계정")
        };

        foreach (var (name, description) in defaultRoles)
        {
            if (!existingRoles.Contains(name))
            {
                var cmdInsert = new SqlCommand(@"
                        INSERT INTO [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp], [Description])
                        VALUES (NEWID(), @Name, UPPER(@Name), NEWID(), @Description)", connection);

                cmdInsert.Parameters.AddWithValue("@Name", name);
                cmdInsert.Parameters.AddWithValue("@Description", description);

                cmdInsert.ExecuteNonQuery();
                _logger.LogInformation($"Default role inserted: {name}");
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
            var logger = services.GetRequiredService<ILogger<TenantSchemaEnhancerEnsureRolesTable>>();
            var config = services.GetRequiredService<IConfiguration>();
            var masterConnectionString = config.GetConnectionString("DefaultConnection")!;

            var enhancer = new TenantSchemaEnhancerEnsureRolesTable(masterConnectionString, logger);

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
            var fallbackLogger = services.GetService<ILogger<TenantSchemaEnhancerEnsureRolesTable>>();
            fallbackLogger?.LogError(ex, "Error while processing AspNetRoles table.");
        }
    }
}
