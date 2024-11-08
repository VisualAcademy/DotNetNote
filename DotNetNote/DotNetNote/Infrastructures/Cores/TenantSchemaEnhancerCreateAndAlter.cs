namespace Dalbodre.Infrastructures.Cores;

public class TenantSchemaEnhancerCreateAndAlter
{
    private readonly string _connectionString;

    public TenantSchemaEnhancerCreateAndAlter(string connectionString)
    {
        _connectionString = connectionString;
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
                SqlCommand cmdCreateTable = new SqlCommand(@"
                        CREATE TABLE [dbo].[Tenants](
                            [ID] bigint IDENTITY(1,1) NOT NULL PRIMARY KEY CLUSTERED,
                            [ConnectionString] nvarchar(max) NULL,
                            [Name] nvarchar(max) NULL,
                            [AuthenticationHeader] nvarchar(max) NULL,
                            [AccountID] nvarchar(max) NULL,
                            [GSConnectionString] nvarchar(max) NULL,
                            [ReportWriterURL] nvarchar(max) NULL,
                            [BadgePhotoType] nvarchar(50) NULL,
                            [PortalName] nvarchar(max) NULL DEFAULT ('AssureHire'),
                            [ScreeningPartnerName] nvarchar(max) NULL DEFAULT ('AssureHire'),
                            [IsMultiPortalEnabled] BIT NULL DEFAULT 0
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

            SqlCommand cmdCheck = new SqlCommand($@"
                    IF NOT EXISTS (
                        SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = @tableName AND COLUMN_NAME = @columnName
                    ) 
                    BEGIN
                        ALTER TABLE dbo.{tableName} ADD {columnName} {columnDefinition};
                    END", connection);

            cmdCheck.Parameters.AddWithValue("@tableName", tableName);
            cmdCheck.Parameters.AddWithValue("@columnName", columnName);

            cmdCheck.ExecuteNonQuery();

            connection.Close();
        }
    }

    // Tenants 테이블에 각 열이 없으면 추가하는 메서드
    public void AddEmployeeURLColumnIfNotExists()
    {
        AddColumnIfNotExists("Tenants", "EmployeeURL", "nvarchar(max) NULL");
    }

    public void AddVendorURLColumnIfNotExists()
    {
        AddColumnIfNotExists("Tenants", "VendorURL", "nvarchar(max) NULL");
    }

    public void AddInternalAuditURLColumnIfNotExists()
    {
        AddColumnIfNotExists("Tenants", "InternalAuditURL", "nvarchar(max) NULL");
    }

    // Tenants 테이블에 IsMultiPortalEnabled 열이 없으면 추가하는 메서드
    public void AddIsMultiPortalEnabledColumnIfNotExists()
    {
        AddColumnIfNotExists("Tenants", "IsMultiPortalEnabled", "BIT NULL DEFAULT 0");
    }

    // 스키마를 보장하는 메서드
    public void EnsureSchema()
    {
        EnsureTenantsTableExists();
        AddIsMultiPortalEnabledColumnIfNotExists();
        UpdateNullIsMultiPortalEnabledToFalse();
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

    // IsMultiPortalEnabled 열의 값이 NULL인 경우 false로 업데이트하는 메서드
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
}
