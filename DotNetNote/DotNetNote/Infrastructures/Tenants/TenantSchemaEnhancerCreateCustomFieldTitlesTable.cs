using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace Hawaso.Infrastructures.Tenants
{
    public class TenantSchemaEnhancerCreateCustomFieldTitlesTable
    {
        private readonly string _masterConnectionString;

        public TenantSchemaEnhancerCreateCustomFieldTitlesTable(string masterConnectionString)
        {
            _masterConnectionString = masterConnectionString;
        }

        public void EnhanceAllTenantDatabases()
        {
            List<string> tenantConnectionStrings = GetTenantConnectionStrings();

            foreach (string connStr in tenantConnectionStrings)
            {
                CreateTableIfNotExists(connStr);
                InsertDefaultValuesIfEmpty(connStr);
            }
        }

        private List<string> GetTenantConnectionStrings()
        {
            List<string> result = new();

            using var connection = new SqlConnection(_masterConnectionString);
            connection.Open();

            var cmd = new SqlCommand("SELECT ConnectionString FROM dbo.Tenants", connection);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                result.Add(reader["ConnectionString"].ToString());
            }

            return result;
        }

        private void CreateTableIfNotExists(string connectionString)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var checkCmd = new SqlCommand(@"
                SELECT COUNT(*) 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_SCHEMA = 'dbo' 
                AND TABLE_NAME = 'CustomFieldTitles'", connection);

            int count = (int)checkCmd.ExecuteScalar();
            if (count > 0) return;

            var createCmd = new SqlCommand(@"
                CREATE TABLE [dbo].[CustomFieldTitles](
                    [ID] bigint IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    [Type] nvarchar(50) NOT NULL,
                    [Field] nvarchar(max) NOT NULL,
                    [Title] nvarchar(max) NULL,
                    [Visible] bit NOT NULL DEFAULT(0),
                    [Searchable] bit NOT NULL DEFAULT(0)
                ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]", connection);

            createCmd.ExecuteNonQuery();
        }

        private void InsertDefaultValuesIfEmpty(string connectionString)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var countCmd = new SqlCommand("SELECT COUNT(*) FROM dbo.CustomFieldTitles", connection);
            int rowCount = (int)countCmd.ExecuteScalar();
            if (rowCount > 0) return;

            var insertCmd = new SqlCommand(@"
                INSERT INTO dbo.CustomFieldTitles ([Type], [Field], [Title], [Visible], [Searchable])
                VALUES 
                    ('EmployeeProfile', 'Custom1', NULL, 0, 0),
                    ('EmployeeProfile', 'Custom2', NULL, 0, 0),
                    ('EmployeeProfile', 'Custom3', NULL, 0, 0),
                    ('EmployeeProfile', 'Custom4', NULL, 0, 0),
                    ('EmployeeProfile', 'Custom5', NULL, 0, 0),
                    ('EmployeeProfile', 'Custom6', NULL, 0, 0),
                    ('StateLicense', 'Custom1', NULL, 0, 0),
                    ('StateLicense', 'Custom2', NULL, 0, 0),
                    ('StateLicense', 'Custom3', NULL, 0, 0),
                    ('StateLicense', 'Custom4', NULL, 0, 0),
                    ('StateLicense', 'Custom5', NULL, 0, 0),
                    ('StateLicense', 'Custom6', NULL, 0, 0)", connection);

            insertCmd.ExecuteNonQuery();
        }
    }
}
