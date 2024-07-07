using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace DotNetNote.Infrastructures.Tenants
{
    public class TenantSchemaEnhancerCreatePartnersTable
    {
        private string _masterConnectionString;

        // 생성자: masterConnectionString을 설정합니다.
        public TenantSchemaEnhancerCreatePartnersTable(string masterConnectionString)
        {
            _masterConnectionString = masterConnectionString;
        }

        // 모든 테넌트 데이터베이스를 향상시키는 메서드
        public void EnhanceAllTenantDatabases()
        {
            List<(string ConnectionString, bool IsMultiPortalEnabled)> tenantDetails = GetTenantDetails();

            foreach (var tenant in tenantDetails)
            {
                // Partners 테이블이 없으면 생성합니다.
                CreatePartnersTableIfNotExists(tenant.ConnectionString);

                // IsMultiPortalEnabled가 true인 경우 기본 파트너를 추가합니다.
                if (tenant.IsMultiPortalEnabled)
                {
                    AddDefaultPartnerIfNotExists(tenant.ConnectionString);
                }
            }
        }

        // 모든 테넌트의 연결 문자열과 IsMultiPortalEnabled 값을 가져오는 메서드
        private List<(string ConnectionString, bool IsMultiPortalEnabled)> GetTenantDetails()
        {
            List<(string ConnectionString, bool IsMultiPortalEnabled)> result = new List<(string, bool)>();

            using (SqlConnection connection = new SqlConnection(_masterConnectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("SELECT ConnectionString, IsMultiPortalEnabled FROM dbo.Tenants", connection);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add((reader["ConnectionString"].ToString(), (bool)reader["IsMultiPortalEnabled"]));
                    }
                }

                connection.Close();
            }

            return result;
        }

        // 특정 테넌트 데이터베이스에 Partners 테이블이 없으면 생성하는 메서드
        private void CreatePartnersTableIfNotExists(string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmdCheck = new SqlCommand(@"
                    SELECT COUNT(*) 
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_SCHEMA = 'dbo' 
                    AND TABLE_NAME = 'Partners'", connection);

                int tableCount = (int)cmdCheck.ExecuteScalar();

                if (tableCount == 0)
                {
                    SqlCommand cmdCreateTable = new SqlCommand(@"
                        CREATE TABLE [dbo].[Partners](
                            [ID] bigint IDENTITY(1,1) NOT NULL PRIMARY KEY CLUSTERED,
                            [BaseAPIAddress] nvarchar(max) NULL,
                            [Name] nvarchar(max) NULL,
                            [Password] nvarchar(max) NULL,
                            [PrimaryEmail] nvarchar(max) NULL
                        )", connection);

                    cmdCreateTable.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        // 특정 테넌트 데이터베이스에 기본 파트너가 없으면 추가하는 메서드
        private void AddDefaultPartnerIfNotExists(string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmdCheck = new SqlCommand(@"
                    SELECT COUNT(*) 
                    FROM dbo.Partners 
                    WHERE Name = 'VisualAcademy'", connection);

                int partnerCount = (int)cmdCheck.ExecuteScalar();

                if (partnerCount == 0)
                {
                    // 다음 코드는 교육용 암호를 사용한 것입니다. 실제 프로덕션 환경에서는 사용하지 마세요.
                    SqlCommand cmdInsert = new SqlCommand(@"
                        INSERT INTO dbo.Partners (BaseAPIAddress, Name, Password, PrimaryEmail) 
                        VALUES ('https://portal.visualacademy.com', 'VisualAcademy', 'Pa$$w0rd', 'visualacademy@visualacademy.com')", connection);

                    cmdInsert.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
    }
}
