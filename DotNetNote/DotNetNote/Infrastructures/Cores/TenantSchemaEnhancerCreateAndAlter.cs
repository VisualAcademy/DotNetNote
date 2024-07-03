namespace Dalbodre.Infrastructures.Cores;

public class TenantSchemaEnhancerCreateAndAlter
{
    private readonly string _connectionString;

    public TenantSchemaEnhancerCreateAndAlter(string connectionString)
    {
        _connectionString = connectionString;
    }

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

    public void AddIsMultiPortalEnabledColumnIfNotExists()
    {
        AddColumnIfNotExists("Tenants", "IsMultiPortalEnabled", "BIT NULL DEFAULT 0");
    }

    public void EnsureSchema()
    {
        EnsureTenantsTableExists();
        AddIsMultiPortalEnabledColumnIfNotExists();
    }
}
