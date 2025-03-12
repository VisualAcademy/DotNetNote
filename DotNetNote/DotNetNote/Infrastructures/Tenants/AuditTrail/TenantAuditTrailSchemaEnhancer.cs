using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Azunt.Infrastructures
{
    public class TenantAuditTrailSchemaEnhancer
    {
        private readonly string _masterConnectionString;
        private readonly ILogger<TenantAuditTrailSchemaEnhancer>? _logger;

        public TenantAuditTrailSchemaEnhancer(string masterConnectionString, ILogger<TenantAuditTrailSchemaEnhancer>? logger = null)
        {
            _masterConnectionString = masterConnectionString;
            _logger = logger;
        }

        public void EnhanceAllTenantDatabases()
        {
            List<string> tenantConnectionStrings = GetTenantConnectionStrings();

            foreach (string connStr in tenantConnectionStrings)
            {
                try
                {
                    EnsureAuditTrailTableExists(connStr);
                    IncreaseNoteColumnSize(connStr);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, $"테넌트 DB 처리 중 오류 발생: {connStr}");
                }
            }
        }

        private List<string> GetTenantConnectionStrings()
        {
            List<string> result = new List<string>();

            using (SqlConnection connection = new SqlConnection(_masterConnectionString))
            {
                try
                {
                    connection.Open();

                    SqlCommand cmd = new SqlCommand("SELECT ConnectionString FROM dbo.Tenants", connection);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string? connectionString = reader["ConnectionString"] as string;

                            if (!string.IsNullOrWhiteSpace(connectionString) && IsValidConnectionString(connectionString))
                            {
                                result.Add(connectionString);
                            }
                            else
                            {
                                _logger?.LogWarning($"잘못된 ConnectionString 발견: '{connectionString}'");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "테넌트 ConnectionString을 가져오는 중 오류 발생");
                }
            }

            return result;
        }

        private void EnsureAuditTrailTableExists(string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    SqlCommand cmdCheck = new SqlCommand(@"
                        SELECT COUNT(*) 
                        FROM INFORMATION_SCHEMA.TABLES 
                        WHERE TABLE_NAME = 'AuditTrail'", connection);

                    int tableCount = (int)cmdCheck.ExecuteScalar();

                    if (tableCount == 0)
                    {
                        SqlCommand cmdCreateTable = new SqlCommand(@"
                            CREATE TABLE [dbo].[AuditTrail](
                                [ID] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY CLUSTERED,
                                [Active] BIT NOT NULL DEFAULT 1,
                                [CreatedAt] DATETIMEOFFSET(7) NOT NULL,
                                [CreatedBy] NVARCHAR(70),
                                [EntityID] BIGINT NOT NULL,
                                [EntityName] NVARCHAR(128),
                                [Operation] NVARCHAR(70),
                                [EmployeeID] BIGINT,
                                [InvestigationID] BIGINT,
                                [Note] NVARCHAR(256) NULL
                            )", connection);
                        cmdCreateTable.ExecuteNonQuery();

                        _logger?.LogInformation("AuditTrail 테이블을 생성했습니다.");
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "AuditTrail 테이블 확인 중 오류 발생");
                }
            }
        }

        private void IncreaseNoteColumnSize(string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    SqlCommand cmdCheck = new SqlCommand(@"
                        SELECT CHARACTER_MAXIMUM_LENGTH 
                        FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'AuditTrail' AND COLUMN_NAME = 'Note'", connection);

                    object result = cmdCheck.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int currentLength) && currentLength < 256)
                    {
                        SqlCommand cmdAlter = new SqlCommand(@"
                            ALTER TABLE [dbo].[AuditTrail]
                            ALTER COLUMN [Note] NVARCHAR(256) NULL", connection);
                        cmdAlter.ExecuteNonQuery();

                        _logger?.LogInformation("AuditTrail 테이블의 Note 컬럼 크기를 256으로 확장했습니다.");
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Note 컬럼 크기 변경 중 오류 발생");
                }
            }
        }

        private bool IsValidConnectionString(string connectionString)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(connectionString);
                return !string.IsNullOrWhiteSpace(builder.DataSource) && !string.IsNullOrWhiteSpace(builder.InitialCatalog);
            }
            catch
            {
                return false;
            }
        }
    }
}
