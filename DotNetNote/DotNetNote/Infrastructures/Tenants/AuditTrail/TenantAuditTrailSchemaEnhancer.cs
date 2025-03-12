using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Azunt.Infrastructures
{
    public class TenantAuditTrailSchemaEnhancer
    {
        private readonly string _masterConnectionString;

        public TenantAuditTrailSchemaEnhancer(string masterConnectionString)
        {
            _masterConnectionString = masterConnectionString;
        }

        public void EnhanceAllTenantDatabases()
        {
            List<string> tenantConnectionStrings = GetTenantConnectionStrings();

            foreach (string connStr in tenantConnectionStrings)
            {
                //EnsureAuditTrailTableExists(connStr);
                IncreaseNoteColumnSize(connStr);
            }
        }

        private List<string> GetTenantConnectionStrings()
        {
            List<string> result = new List<string>();

            using (SqlConnection connection = new SqlConnection(_masterConnectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("SELECT ConnectionString FROM dbo.Tenants", connection);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(reader["ConnectionString"].ToString());
                    }
                }

                connection.Close();
            }

            return result;
        }

        private void EnsureAuditTrailTableExists(string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
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
                }

                connection.Close();
            }
        }

        private void IncreaseNoteColumnSize(string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
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
                }

                connection.Close();
            }
        }
    }
}
