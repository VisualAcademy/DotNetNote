namespace Dalbodre.Infrastructures.Cores
{
    public class AspNetUsersTableEnhancer
    {
        private readonly string _connectionString;

        public AspNetUsersTableEnhancer(string connectionString)
        {
            _connectionString = connectionString;
        }

        // AspNetUsers 테이블에 ShowInDropdown 컬럼이 없으면 추가하는 메서드
        public void AddShowInDropdownColumnIfNotExists()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                SqlCommand cmdCheck = new SqlCommand(@"
                    IF NOT EXISTS (
                        SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME = 'ShowInDropdown'
                    ) 
                    BEGIN
                        ALTER TABLE dbo.AspNetUsers ADD ShowInDropdown BIT NULL DEFAULT 0;
                    END", connection);

                cmdCheck.ExecuteNonQuery();

                connection.Close();
            }
        }
    }
}
