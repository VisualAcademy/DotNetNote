namespace Azunt.Infrastructures.Auth;

/// <summary>
/// 테넌트 및 마스터 데이터베이스에 AspNetRoles 테이블을 생성 및 보강하고,
/// 기본 역할 데이터를 삽입하는 역할을 수행하는 클래스입니다.
/// </summary>
public class TenantSchemaEnhancerEnsureRolesTable
{
    private readonly string _masterConnectionString;
    private readonly ILogger<TenantSchemaEnhancerEnsureRolesTable> _logger;

    /// <summary>
    /// 생성자: 마스터 연결 문자열과 로거를 받아 초기화합니다.
    /// </summary>
    /// <param name="masterConnectionString">마스터 데이터베이스 연결 문자열</param>
    /// <param name="logger">로깅을 위한 ILogger 인스턴스</param>
    public TenantSchemaEnhancerEnsureRolesTable(
        string masterConnectionString,
        ILogger<TenantSchemaEnhancerEnsureRolesTable> logger)
    {
        _masterConnectionString = masterConnectionString;
        _logger = logger;
    }

    /// <summary>
    /// 모든 테넌트 데이터베이스에서 AspNetRoles 테이블이 존재하는지 확인하고,
    /// 존재하지 않으면 생성하며, Description 컬럼이 없으면 추가합니다.
    /// 또한 기본 역할(Administrators, Everyone, Users, Guests)을 삽입합니다.
    /// </summary>
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

    /// <summary>
    /// 마스터 데이터베이스에서 AspNetRoles 테이블을 보장하고 기본 역할을 삽입합니다.
    /// </summary>
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

    /// <summary>
    /// 마스터 데이터베이스에서 모든 테넌트의 연결 문자열을 조회합니다.
    /// </summary>
    /// <returns>테넌트 연결 문자열 리스트</returns>
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

    /// <summary>
    /// 주어진 연결 문자열에 대해 AspNetRoles 테이블 및 Description 컬럼을 보장하고,
    /// 기본 역할 데이터를 삽입합니다.
    /// </summary>
    /// <param name="connectionString">데이터베이스 연결 문자열</param>
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

    /// <summary>
    /// AspNetRoles 테이블에 필요한 기본 역할(Administrators, Everyone, Users, Guests)이
    /// 존재하지 않으면 삽입합니다.
    /// </summary>
    /// <param name="connection">열려 있는 SQL 연결 객체</param>
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
    /// Program.cs 또는 Startup.cs에서 호출되는 진입점입니다.
    /// - <c>forMaster == true</c>: 마스터 DB만 처리
    /// - <c>forMaster == false</c>: 테넌트 DB들만 처리
    /// </summary>
    /// <param name="services">서비스 공급자 (DI 컨테이너)</param>
    /// <param name="forMaster">마스터 DB만 처리할지 여부</param>
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
