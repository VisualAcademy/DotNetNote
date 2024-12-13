namespace Dalbodre.Infrastructures
{
    public class DocumentSchemaEnhancer
    {
        private readonly string _connectionString;

        public DocumentSchemaEnhancer(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void EnsureDocumentsTableAndAlterTitleColumn()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                SqlCommand cmdCheckTable = new SqlCommand(@"
                    SELECT COUNT(*) 
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_SCHEMA = 'dbo' 
                    AND TABLE_NAME = 'Documents'", connection);

                int tableCount = (int)cmdCheckTable.ExecuteScalar();

                if (tableCount > 0)
                {
                    SqlCommand cmdCheckColumn = new SqlCommand(@"
                        SELECT CHARACTER_MAXIMUM_LENGTH
                        FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'Documents' 
                        AND COLUMN_NAME = 'Title'", connection);

                    int? columnLength = (int?)cmdCheckColumn.ExecuteScalar();

                    if (columnLength != 512)
                    {
                        SqlCommand cmdAlterColumn = new SqlCommand(@"
                            ALTER TABLE dbo.Documents 
                            ALTER COLUMN Title NVARCHAR(512) NOT NULL", connection);

                        cmdAlterColumn.ExecuteNonQuery();
                    }
                }

                connection.Close();
            }
        }

        // Method to add the Language column if it doesn't exist
        public void AddLanguageColumnIfNotExists()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Check if the Language column exists in the Documents table
                SqlCommand cmdCheckColumn = new SqlCommand(@"
                    IF NOT EXISTS (
                        SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'Documents' 
                        AND COLUMN_NAME = 'Language'
                    )
                    BEGIN
                        ALTER TABLE dbo.Documents ADD Language NVARCHAR(255) NULL DEFAULT 'ko-KR';
                    END", connection);

                cmdCheckColumn.ExecuteNonQuery();

                connection.Close();
            }
        }

        // Method to update Language column values to 'en-US' if they are NULL
        public void UpdateNullLanguageValuesToDefault()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Update Language column to 'en-US' where it is NULL
                SqlCommand cmdUpdate = new SqlCommand(@"
                    UPDATE dbo.Documents
                    SET Language = 'en-US'
                    WHERE Language IS NULL", connection);

                cmdUpdate.ExecuteNonQuery();

                connection.Close();
            }
        }
    }
}
