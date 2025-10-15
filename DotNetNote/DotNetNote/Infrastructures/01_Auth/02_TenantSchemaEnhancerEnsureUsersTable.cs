using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;

namespace Azunt.Web.Infrastructures.Auth;

/// <summary>
/// 테넌트 및 마스터 데이터베이스에 AspNetUsers 테이블을 생성 및 보강하고,
/// 필요한 컬럼들을 추가하는 클래스입니다. (KOR)
/// This class ensures the AspNetUsers table is created and extended in both tenant and master databases. (ENG)
/// </summary>
public class TenantSchemaEnhancerEnsureUsersTable
{
    private readonly string _masterConnectionString;
    private readonly ILogger<TenantSchemaEnhancerEnsureUsersTable> _logger;

    /// <summary>
    /// 생성자: 마스터 연결 문자열과 로깅 서비스를 주입받습니다. (KOR)
    /// Constructor: Injects master connection string and logger service. (ENG)
    /// </summary>
    public TenantSchemaEnhancerEnsureUsersTable(
        string masterConnectionString,
        ILogger<TenantSchemaEnhancerEnsureUsersTable> logger)
    {
        _masterConnectionString = masterConnectionString;
        _logger = logger;
    }

    /// <summary>
    /// 모든 테넌트 데이터베이스에 대해 AspNetUsers 테이블을 생성 또는 보강합니다. (KOR)
    /// Ensures the AspNetUsers table exists and is extended in all tenant databases. (ENG)
    /// </summary>
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

    /// <summary>
    /// 마스터 데이터베이스에 대해 AspNetUsers 테이블을 생성 또는 보강합니다. (KOR)
    /// Ensures the AspNetUsers table exists and is extended in the master database. (ENG)
    /// </summary>
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

    /// <summary>
    /// 마스터 DB에서 테넌트 DB의 연결 문자열을 조회합니다. (KOR)
    /// Retrieves tenant DB connection strings from the master DB. (ENG)
    /// </summary>
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
    /// 특정 DB 연결 문자열에 대해 AspNetUsers 테이블 존재 여부를 확인하고,
    /// 없으면 기본 구조를 생성하고, 확장 컬럼을 동적으로 추가합니다. (KOR)
    /// Ensures AspNetUsers table and its expected columns exist in the target DB. (ENG)
    /// </summary>
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

            // 테이블 생성
            if (tableExists == 0)
            {
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
                        [AccessFailedCount] INT NOT NULL
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

            // 누락된 확장 컬럼 보강
            var expectedColumns = new Dictionary<string, string>
            {
                // 사용자 기본 정보
                ["FirstName"] = "NVARCHAR(256) NULL",
                ["LastName"] = "NVARCHAR(256) NULL",
                ["MiddleName"] = "NVARCHAR(35) NULL",
                ["AliasNames"] = "NVARCHAR(MAX) NULL",
                ["NameSuffix"] = "NVARCHAR(MAX) NULL",
                ["Address"] = "NVARCHAR(70) NULL",
                ["City"] = "NVARCHAR(70) NULL",
                ["County"] = "NVARCHAR(MAX) NULL",
                ["PostalCode"] = "NVARCHAR(35) NULL",
                ["State"] = "NVARCHAR(2) NULL",
                ["Timezone"] = "NVARCHAR(MAX) NULL",
                ["UsernameChangeLimit"] = "INT NULL DEFAULT(0)",
                ["Photo"] = "NVARCHAR(MAX) NULL",
                ["ProfilePicture"] = "VARBINARY(MAX) NULL",
                ["PersonalEmail"] = "NVARCHAR(254) NULL",

                // 출생 정보
                ["DOB"] = "NVARCHAR(MAX) NULL",
                ["Age"] = "INT NULL",
                ["BirthCity"] = "NVARCHAR(70) NULL",
                ["BirthState"] = "NVARCHAR(2) NULL",
                ["BirthCountry"] = "NVARCHAR(70) NULL",
                ["BirthCounty"] = "NVARCHAR(MAX) NULL",
                ["BirthPlace"] = "NVARCHAR(MAX) NULL",
                ["Gender"] = "NVARCHAR(35) NULL",
                ["MaritalStatus"] = "NVARCHAR(MAX) NULL",
                ["UsCitizen"] = "NVARCHAR(MAX) NULL",
                ["PhysicalMarks"] = "NVARCHAR(MAX) NULL",
                ["Height"] = "NVARCHAR(MAX) NULL",
                ["HeightFeet"] = "NVARCHAR(MAX) NULL",
                ["HeightInches"] = "NVARCHAR(MAX) NULL",
                ["Weight"] = "NVARCHAR(MAX) NULL",
                ["EyeColor"] = "NVARCHAR(MAX) NULL",
                ["HairColor"] = "NVARCHAR(MAX) NULL",

                // 연락처
                ["PrimaryPhone"] = "NVARCHAR(35) NULL",
                ["SecondaryPhone"] = "NVARCHAR(35) NULL",
                ["MobilePhone"] = "NVARCHAR(MAX) NULL",
                ["HomePhone"] = "NVARCHAR(MAX) NULL",
                ["WorkPhone"] = "NVARCHAR(MAX) NULL",
                ["WorkFax"] = "NVARCHAR(MAX) NULL",

                // 직장 및 비즈니스
                ["OfficeAddress"] = "NVARCHAR(MAX) NULL",
                ["OfficeCity"] = "NVARCHAR(MAX) NULL",
                ["OfficeState"] = "NVARCHAR(MAX) NULL",
                ["Department"] = "NVARCHAR(MAX) NULL",
                ["Title"] = "NVARCHAR(MAX) NULL",
                ["BusinessStructure"] = "NVARCHAR(MAX) NULL",
                ["BusinessStructureOther"] = "NVARCHAR(MAX) NULL",

                // 신분 및 인증 정보
                ["SSN"] = "NVARCHAR(MAX) NULL",
                ["DriverLicenseNumber"] = "NVARCHAR(35) NULL",
                ["DriverLicenseState"] = "NVARCHAR(2) NULL",
                ["DriverLicenseExpiration"] = "DATETIME2(7) NULL",
                ["LicenseNumber"] = "NVARCHAR(35) NULL",

                // 시스템/보안
                ["IsEnrollment"] = "BIT NOT NULL DEFAULT(0) WITH VALUES",
                ["IsEnabled"] = "BIT NULL",
                ["IsPrimary"] = "BIT NULL",
                ["IsKodeeSupport"] = "BIT NULL",
                ["ConfidentialAccess"] = "BIT NULL",
                ["Group1Access"] = "BIT NULL",
                ["Group2Access"] = "BIT NULL",
                ["Group3Access"] = "BIT NULL",
                ["ShowStartUpMsg"] = "BIT NULL DEFAULT(0)",
                ["OpensToAppointments"] = "BIT NULL",
                ["LastInvitationSent"] = "DATETIME NULL",
                ["DateTimePasswordUpdated"] = "DATETIMEOFFSET(7) NULL",
                ["PswToOverwrite"] = "TINYINT NULL DEFAULT(1)",

                // 과거 비밀번호
                ["OldPsw1"] = "NVARCHAR(MAX) NULL",
                ["OldPsw2"] = "NVARCHAR(MAX) NULL",
                ["OldPsw3"] = "NVARCHAR(MAX) NULL",
                ["OldPsw4"] = "NVARCHAR(MAX) NULL",
                ["OldPsw5"] = "NVARCHAR(MAX) NULL",
                ["OldPsw6"] = "NVARCHAR(MAX) NULL",
                ["OldPsw7"] = "NVARCHAR(MAX) NULL",
                ["OldPsw8"] = "NVARCHAR(MAX) NULL",
                ["OldPsw9"] = "NVARCHAR(MAX) NULL",

                // 로그인 제약 정보
                ["IpAddress1"] = "NVARCHAR(MAX) NULL",
                ["IpAddress2"] = "NVARCHAR(MAX) NULL",
                ["LimitIP"] = "BIT NULL",
                ["RefreshToken"] = "NVARCHAR(MAX) NULL",
                ["RefreshTokenExpiryTime"] = "DATETIME NULL",

                // 멀티테넌시/권한 정보
                ["TenantID"] = "BIGINT NULL DEFAULT(0)",
                ["TenantName"] = "NVARCHAR(MAX) DEFAULT(N'Azunt')",
                ["RoleID"] = "BIGINT NULL",
                ["DivisionId"] = "BIGINT NULL DEFAULT(0)",
                ["DivisionName"] = "NVARCHAR(255) NULL DEFAULT(N'')",

                // 신규 플래그: 여러 테넌트 이력/후보 보유 여부 (NULL 허용, 기본값 0)
                ["HasMultipleTenants"] = "BIT NULL",

                // 기타
                ["CriminalHistory"] = "NVARCHAR(MAX) NULL",
                ["Name"] = "NVARCHAR(MAX) NULL",
                ["RegistrationDate"] = "DATETIMEOFFSET NULL DEFAULT SYSDATETIMEOFFSET()",
                ["ShowInDropdown"] = "BIT NULL DEFAULT(0)",
                ["ConcurrencyToken"] = "ROWVERSION"
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

            // RegistrationDate 기본값/백필 보장
            EnsureRegistrationDateDefaultAndBackfill(connection);

            // HasMultipleTenants 기본값/백필 보장 (NULL 허용 + DEFAULT(0) + 기존 NULL은 0으로 백필)
            EnsureHasMultipleTenantsDefaultAndBackfill(connection);
        }
    }

    /// <summary>
    /// Program.cs 또는 Startup.cs에서 호출되는 진입점입니다. (KOR)
    /// Entry point to be called from Program.cs or Startup.cs. (ENG)
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

    /// <summary>
    /// AspNetUsers.RegistrationDate 컬럼의 기본값(DEFAULT)과 기존 NULL 값 백필을 보장합니다.
    /// </summary>
    private void EnsureRegistrationDateDefaultAndBackfill(SqlConnection connection)
    {
        // 1) 컬럼 없으면 추가 (방어적)
        using (var addColumnCmd = new SqlCommand(@"
        IF NOT EXISTS (
            SELECT 1
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_NAME = N'AspNetUsers' AND COLUMN_NAME = N'RegistrationDate'
        )
        BEGIN
            ALTER TABLE [dbo].[AspNetUsers]
            ADD [RegistrationDate] DATETIMEOFFSET NULL;
        END
    ", connection))
        {
            addColumnCmd.ExecuteNonQuery();
        }

        // 2) DEFAULT 제약 없으면 추가 (SYSDATETIMEOFFSET)
        using (var addDefaultCmd = new SqlCommand(@"
        IF NOT EXISTS (
            SELECT 1
            FROM sys.default_constraints dc
            INNER JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
            INNER JOIN sys.tables t ON t.object_id = c.object_id
            WHERE t.name = N'AspNetUsers' AND c.name = N'RegistrationDate'
        )
        BEGIN
            ALTER TABLE [dbo].[AspNetUsers]
            ADD CONSTRAINT [DF_AspNetUsers_RegistrationDate] DEFAULT (SYSDATETIMEOFFSET()) FOR [RegistrationDate];
        END
    ", connection))
        {
            addDefaultCmd.ExecuteNonQuery();
        }

        // 3) 기존 NULL 값 백필
        using (var updateNullsCmd = new SqlCommand(@"
        UPDATE [dbo].[AspNetUsers]
        SET [RegistrationDate] = SYSDATETIMEOFFSET()
        WHERE [RegistrationDate] IS NULL;
    ", connection))
        {
            var affected = updateNullsCmd.ExecuteNonQuery();
            _logger.LogInformation($"AspNetUsers.RegistrationDate backfilled: {affected} row(s).");
        }
    }

    /// <summary>
    /// AspNetUsers.HasMultipleTenants 컬럼의 DEFAULT(0)과 기존 NULL 값 백필을 보장합니다.
    /// - 컬럼은 NULL 허용(Bit)로 유지
    /// - DEFAULT 제약이 없으면 추가
    /// - 기존 NULL 값은 0(false)로 백필
    /// </summary>
    private void EnsureHasMultipleTenantsDefaultAndBackfill(SqlConnection connection)
    {
        // 1) 컬럼 없으면 추가 (방어적) — NULL 허용
        using (var addColumnCmd = new SqlCommand(@"
        IF NOT EXISTS (
            SELECT 1
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_NAME = N'AspNetUsers' AND COLUMN_NAME = N'HasMultipleTenants'
        )
        BEGIN
            ALTER TABLE [dbo].[AspNetUsers]
            ADD [HasMultipleTenants] BIT NULL;
        END
    ", connection))
        {
            addColumnCmd.ExecuteNonQuery();
        }

        // 2) DEFAULT 제약 없으면 추가 (0)
        using (var addDefaultCmd = new SqlCommand(@"
        IF NOT EXISTS (
            SELECT 1
            FROM sys.default_constraints dc
            INNER JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
            INNER JOIN sys.tables t ON t.object_id = c.object_id
            WHERE t.name = N'AspNetUsers' AND c.name = N'HasMultipleTenants'
        )
        BEGIN
            ALTER TABLE [dbo].[AspNetUsers]
            ADD CONSTRAINT [DF_AspNetUsers_HasMultipleTenants] DEFAULT (0) FOR [HasMultipleTenants];
        END
    ", connection))
        {
            addDefaultCmd.ExecuteNonQuery();
        }

        // 3) 기존 NULL 값 백필 → 0(false)
        using (var updateNullsCmd = new SqlCommand(@"
        UPDATE [dbo].[AspNetUsers]
        SET [HasMultipleTenants] = 0
        WHERE [HasMultipleTenants] IS NULL;
    ", connection))
        {
            var affected = updateNullsCmd.ExecuteNonQuery();
            _logger.LogInformation($"AspNetUsers.HasMultipleTenants backfilled to 0: {affected} row(s).");
        }
    }

    /// <summary>
    /// 마스터 DB에서 특정 이메일의 TenantID/TenantId 값을 지정 값으로 업데이트합니다.
    /// </summary>
    public int SetTenantIdForEmailInMaster(string email, long tenantId)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("email is required.", nameof(email));

        try
        {
            using (var connection = new SqlConnection(_masterConnectionString))
            {
                connection.Open();

                var columnName = ResolveTenantIdColumnName(connection);
                if (columnName == null)
                {
                    _logger.LogWarning("AspNetUsers 테이블에 TenantID/TenantId 컬럼이 없습니다. EnsureUsersTable 실행 후 재시도하세요.");
                    return 0;
                }

                var normalized = email.Trim().ToUpperInvariant();

                // NormalizedEmail 우선, 없으면 Email 대소문자 무시 매칭
                var sql = $@"
UPDATE [dbo].[AspNetUsers]
   SET [{columnName}] = @TenantId
 WHERE (NormalizedEmail = @NormalizedEmail)
    OR (NormalizedEmail IS NULL AND Email IS NOT NULL AND UPPER(Email) = @NormalizedEmail);";

                using (var cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add(new SqlParameter("@TenantId", SqlDbType.BigInt) { Value = tenantId });
                    cmd.Parameters.Add(new SqlParameter("@NormalizedEmail", SqlDbType.NVarChar, 256) { Value = normalized });

                    var affected = cmd.ExecuteNonQuery();
                    _logger.LogInformation($"[MASTER] Updated TenantID for {email} -> {tenantId} (rows: {affected})");
                    return affected;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[MASTER] Failed to update TenantID for {email}");
            return 0;
        }
    }

    /// <summary>
    /// 스키마에 존재하는 테넌트 컬럼명을 확인(우선순위: TenantID -> TenantId)하여 반환합니다.
    /// </summary>
    private static string? ResolveTenantIdColumnName(SqlConnection connection)
    {
        static string? Check(SqlConnection conn, string name)
        {
            using var checkCmd = new SqlCommand(@"
SELECT COUNT(*)
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME = @Name;", conn);

            checkCmd.Parameters.AddWithValue("@Name", name);
            var exists = (int)checkCmd.ExecuteScalar();
            return exists > 0 ? name : null;
        }

        // 우선순위: TenantID -> TenantId
        return Check(connection, "TenantID") ?? Check(connection, "TenantId");
    }

    /// <summary>
    /// Startup.cs 등에서 바로 호출 가능한 정적 진입점.
    /// - Ensure(보강) 먼저 하고
    /// - 이어서 특정 이메일의 TenantID 갱신까지 한 번에 처리.
    /// </summary>
    public static int RunSetTenantIdForEmail(IServiceProvider services, string email, long tenantId)
    {
        var logger = services.GetRequiredService<ILogger<TenantSchemaEnhancerEnsureUsersTable>>();
        var config = services.GetRequiredService<IConfiguration>();
        var masterConnectionString = config.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(masterConnectionString))
            throw new InvalidOperationException("Master connection string 'DefaultConnection' is not configured.");

        var enhancer = new TenantSchemaEnhancerEnsureUsersTable(masterConnectionString, logger);

        // 1) 스키마 보강(선 방어)
        enhancer.EnhanceMasterDatabase();

        // 2) 값 갱신
        return enhancer.SetTenantIdForEmailInMaster(email, tenantId);
    }
}
