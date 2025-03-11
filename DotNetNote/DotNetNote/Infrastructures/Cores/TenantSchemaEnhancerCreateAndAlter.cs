using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Dalbodre.Infrastructures.Cores;

public class TenantSchemaEnhancerCreateAndAlter
{
    private readonly string _connectionString;
    private readonly string _defaultPortalName;

    public TenantSchemaEnhancerCreateAndAlter(string connectionString, IConfiguration configuration)
    {
        _connectionString = connectionString;
        _defaultPortalName = configuration["AppKeys__PortalName"] ?? "Hawaso";
    }

    // Tenants 테이블이 없으면 생성하는 메서드
    public void EnsureTenantsTableExists()
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            SqlCommand cmdCheck = new SqlCommand(@"
                    SELECT COUNT(*) 
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_SCHEMA = 'dbo' 
                    AND TABLE_NAME = 'Tenants'", connection);

            int tableCount = (int)cmdCheck.ExecuteScalar();

            if (tableCount == 0)
            {
                SqlCommand cmdCreateTable = new SqlCommand($@"
                        CREATE TABLE [dbo].[Tenants](
                            [ID] bigint IDENTITY(1,1) NOT NULL PRIMARY KEY CLUSTERED,
                            [ConnectionString] nvarchar(max) NULL,
                            [Name] nvarchar(max) NULL,
                            [AuthenticationHeader] nvarchar(max) NULL,
                            [AccountID] nvarchar(max) NULL,
                            [GSConnectionString] nvarchar(max) NULL,
                            [ReportWriterURL] nvarchar(max) NULL,
                            [BadgePhotoType] nvarchar(50) NULL,
                            [PortalName] nvarchar(max) NULL DEFAULT ('{_defaultPortalName}'),
                            [ScreeningPartnerName] nvarchar(max) NULL DEFAULT ('{_defaultPortalName}'),
                            [IsMultiPortalEnabled] BIT NULL DEFAULT 0,
                            [IsNewPortalOnly] BIT NULL DEFAULT 0
                        )", connection);

                cmdCreateTable.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

    // 특정 테이블에 특정 컬럼이 없으면 추가하는 메서드
    public void AddColumnIfNotExists(string tableName, string columnName, string columnDefinition)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            SqlCommand cmdCheck = new SqlCommand(@"
                SELECT COUNT(*) 
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_SCHEMA = 'dbo'
                AND TABLE_NAME = @tableName
                AND COLUMN_NAME = @columnName", connection);

            cmdCheck.Parameters.AddWithValue("@tableName", tableName);
            cmdCheck.Parameters.AddWithValue("@columnName", columnName);

            int columnCount = (int)cmdCheck.ExecuteScalar();

            if (columnCount == 0) // 컬럼이 존재하지 않는 경우
            {
                string alterTableQuery = $"ALTER TABLE dbo.{tableName} ADD {columnName} {columnDefinition};";
                SqlCommand cmdAlter = new SqlCommand(alterTableQuery, connection);
                cmdAlter.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

    // 새로운 컬럼 추가: IsNewPortalOnly (없으면 추가)
    public void AddIsNewPortalOnlyColumnIfNotExists()
    {
        AddColumnIfNotExists("Tenants", "IsNewPortalOnly", "BIT NULL DEFAULT 0");
    }

    // 기존의 NULL 값을 False로 설정
    public void UpdateNullIsNewPortalOnlyToFalse()
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            SqlCommand cmdUpdate = new SqlCommand(@"
                    UPDATE dbo.Tenants
                    SET IsNewPortalOnly = 0
                    WHERE IsNewPortalOnly IS NULL", connection);

            cmdUpdate.ExecuteNonQuery();

            connection.Close();
        }
    }

    // IsMultiPortalEnabled 컬럼이 없으면 추가
    public void AddIsMultiPortalEnabledColumnIfNotExists()
    {
        AddColumnIfNotExists("Tenants", "IsMultiPortalEnabled", "BIT NULL DEFAULT 0");
    }

    // 기존의 NULL 값을 False로 설정
    public void UpdateNullIsMultiPortalEnabledToFalse()
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            SqlCommand cmdUpdate = new SqlCommand(@"
                    UPDATE dbo.Tenants
                    SET IsMultiPortalEnabled = 0
                    WHERE IsMultiPortalEnabled IS NULL", connection);

            cmdUpdate.ExecuteNonQuery();

            connection.Close();
        }
    }

    // EmployeeURL 컬럼 추가
    public void AddEmployeeURLColumnIfNotExists()
    {
        AddColumnIfNotExists("Tenants", "EmployeeURL", "nvarchar(max) NULL");
    }

    // VendorURL 컬럼 추가
    public void AddVendorURLColumnIfNotExists()
    {
        AddColumnIfNotExists("Tenants", "VendorURL", "nvarchar(max) NULL");
    }

    // InternalAuditURL 컬럼 추가
    public void AddInternalAuditURLColumnIfNotExists()
    {
        AddColumnIfNotExists("Tenants", "InternalAuditURL", "nvarchar(max) NULL");
    }

    // 스키마를 보장하는 메서드
    public void EnsureSchema()
    {
        EnsureTenantsTableExists();  // 테이블이 먼저 생성되어야 컬럼 추가 가능

        // IsMultiPortalEnabled 관련 컬럼 추가 및 NULL 처리
        AddIsMultiPortalEnabledColumnIfNotExists();
        UpdateNullIsMultiPortalEnabledToFalse();

        // IsNewPortalOnly 관련 컬럼 추가 및 NULL 처리
        AddIsNewPortalOnlyColumnIfNotExists();
        UpdateNullIsNewPortalOnlyToFalse();

        // 기타 URL 컬럼 추가
        AddEmployeeURLColumnIfNotExists();
        AddVendorURLColumnIfNotExists();
        AddInternalAuditURLColumnIfNotExists();
    }

    // 특정 테넌트의 IsMultiPortalEnabled 열을 1로 설정하는 메서드
    public void SetMultiPortalEnabledForSpecificTenants()
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            SqlCommand cmdUpdate = new SqlCommand(@"
                    UPDATE dbo.Tenants
                    SET IsMultiPortalEnabled = 1
                    WHERE Name IN ('KodeeLite', 'ChickenRanch')", connection);

            cmdUpdate.ExecuteNonQuery();

            connection.Close();
        }
    }
}
