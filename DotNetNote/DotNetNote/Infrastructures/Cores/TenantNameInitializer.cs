using Microsoft.Data.SqlClient;

namespace Dalbodre.Infrastructures
{
    public class TenantNameInitializer
    {
        private readonly string _connectionString;

        public TenantNameInitializer(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void InitializeEmptyTenantNames()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                SqlCommand cmdUpdate = new SqlCommand(@"
                    UPDATE ApplicantsTransfers
                    SET TenantName = 'VisualAcademy'
                    WHERE TenantName IS NULL OR TenantName = ''", connection);

                cmdUpdate.ExecuteNonQuery();

                connection.Close();
            }
        }
    }
}
