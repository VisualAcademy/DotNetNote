using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Dalbodre
{
    public class UserTableEnhancer
    {
        private readonly string _connectionString;

        public UserTableEnhancer(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> TableExistsAsync(string tableName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var checkTableCmd = new SqlCommand
                {
                    Connection = connection,
                    CommandText = @"
                        SELECT COUNT(*) 
                        FROM INFORMATION_SCHEMA.TABLES 
                        WHERE TABLE_SCHEMA = 'dbo' 
                        AND TABLE_NAME = @tableName",
                    CommandType = System.Data.CommandType.Text
                };

                checkTableCmd.Parameters.AddWithValue("@tableName", tableName);

                int tableCount = (int)await checkTableCmd.ExecuteScalarAsync();

                connection.Close();

                return tableCount > 0;
            }
        }

        public async Task AddColumnIfNotExistsAsync(string tableName, string columnName, string columnDefinition)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var checkAndAddColumnCmd = new SqlCommand
                {
                    Connection = connection,
                    CommandText = $@"
                        IF NOT EXISTS (
                            SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                            WHERE TABLE_NAME = @tableName AND COLUMN_NAME = @columnName
                        ) 
                        BEGIN
                            ALTER TABLE dbo.{tableName} ADD {columnName} {columnDefinition};
                        END;",
                    CommandType = System.Data.CommandType.Text
                };

                checkAndAddColumnCmd.Parameters.AddWithValue("@tableName", tableName);
                checkAndAddColumnCmd.Parameters.AddWithValue("@columnName", columnName);

                await checkAndAddColumnCmd.ExecuteNonQueryAsync();

                connection.Close();
            }
        }

        public async Task EnsureUserTableColumnsAsync()
        {
            if (await TableExistsAsync("AspNetUsers"))
            {
                await AddColumnIfNotExistsAsync("AspNetUsers", "CriminalHistory", "NVARCHAR(MAX) NULL");
                await AddColumnIfNotExistsAsync("AspNetUsers", "SecondaryPhone", "NVARCHAR(50) NULL");
                await AddColumnIfNotExistsAsync("AspNetUsers", "MiddleName", "NVARCHAR(50) NULL");
                await AddColumnIfNotExistsAsync("AspNetUsers", "SSN", "NVARCHAR(50) NULL");
            }
        }
    }
}
