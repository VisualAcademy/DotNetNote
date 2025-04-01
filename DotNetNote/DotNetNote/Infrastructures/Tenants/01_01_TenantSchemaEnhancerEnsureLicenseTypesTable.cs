namespace Azunt.Infrastructures.Tenants
{
    public class TenantSchemaEnhancerEnsureLicenseTypesTable
    {
        private readonly string _masterConnectionString;
        private readonly ILogger<TenantSchemaEnhancerEnsureLicenseTypesTable> _logger;

        public TenantSchemaEnhancerEnsureLicenseTypesTable(
            string masterConnectionString,
            ILogger<TenantSchemaEnhancerEnsureLicenseTypesTable> logger)
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
                    EnsureLicenseTypesTable(connStr);
                    _logger.LogInformation($"LicenseTypes 테이블 처리 완료 (테넌트 DB): {connStr}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"[{connStr}] 테넌트 DB 처리 중 오류 발생");
                }
            }
        }

        public void EnhanceMasterDatabase()
        {
            try
            {
                EnsureLicenseTypesTable(_masterConnectionString);
                _logger.LogInformation("LicenseTypes 테이블 처리 완료 (마스터 DB)");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "마스터 DB 처리 중 오류 발생");
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
                        result.Add(reader["ConnectionString"].ToString());
                    }
                }
            }

            return result;
        }

        private void EnsureLicenseTypesTable(string connectionString)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var cmdCheck = new SqlCommand(@"
                    SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_NAME = 'LicenseTypes'", connection);

                int tableCount = (int)cmdCheck.ExecuteScalar();

                if (tableCount == 0)
                {
                    var cmdCreate = new SqlCommand(@"
                        CREATE TABLE [dbo].[LicenseTypes] (
                            [ID]            BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                            [Active]        BIT NULL,
                            [CreatedAt]     DATETIMEOFFSET(7) NULL,
                            [CreatedBy]     NVARCHAR(70) NULL,
                            [Type]          NVARCHAR(450) NULL,
                            [Description]   NVARCHAR(MAX) NULL,
                            [ApplicantType] INT NULL,
                            [BgRequired]    BIT NULL
                        )", connection);
                    cmdCreate.ExecuteNonQuery();

                    _logger.LogInformation("LicenseTypes 테이블을 새로 생성했습니다.");
                }
                else
                {
                    var expectedColumns = new Dictionary<string, string>
                    {
                        ["Active"] = "BIT NULL",
                        ["CreatedAt"] = "DATETIMEOFFSET(7) NULL",
                        ["CreatedBy"] = "NVARCHAR(70) NULL",
                        ["Type"] = "NVARCHAR(450) NULL",
                        ["Description"] = "NVARCHAR(MAX) NULL",
                        ["ApplicantType"] = "INT NULL",
                        ["BgRequired"] = "BIT NULL"
                    };

                    foreach (var kvp in expectedColumns)
                    {
                        var columnName = kvp.Key;
                        var columnType = kvp.Value;

                        var cmdColumnCheck = new SqlCommand(@"
                            SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                            WHERE TABLE_NAME = 'LicenseTypes' AND COLUMN_NAME = @ColumnName", connection);
                        cmdColumnCheck.Parameters.AddWithValue("@ColumnName", columnName);

                        int colExists = (int)cmdColumnCheck.ExecuteScalar();

                        if (colExists == 0)
                        {
                            var alterCmd = new SqlCommand(
                                $"ALTER TABLE [dbo].[LicenseTypes] ADD [{columnName}] {columnType}", connection);
                            alterCmd.ExecuteNonQuery();

                            _logger.LogInformation($"컬럼 추가됨: {columnName} ({columnType})");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Program.cs 또는 Startup.cs에서 호출
        /// forMaster == true : 마스터 DB만 처리
        /// forMaster == false : 테넌트 DB들만 처리
        /// </summary>
        public static void Run(IServiceProvider services, bool forMaster)
        {
            try
            {
                var logger = services.GetRequiredService<ILogger<TenantSchemaEnhancerEnsureLicenseTypesTable>>();
                var config = services.GetRequiredService<IConfiguration>();
                var masterConnectionString = config.GetConnectionString("DefaultConnection");

                var enhancer = new TenantSchemaEnhancerEnsureLicenseTypesTable(masterConnectionString, logger);

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
                var fallbackLogger = services.GetService<ILogger<TenantSchemaEnhancerEnsureLicenseTypesTable>>();
                fallbackLogger?.LogError(ex, "LicenseTypes 테이블 처리 중 예외 발생");
            }
        }
    }
}
