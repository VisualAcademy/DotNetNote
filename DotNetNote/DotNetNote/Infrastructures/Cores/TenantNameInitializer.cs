namespace Dalbodre.Infrastructures;

public class TenantNameInitializer(string connectionString)
{
    public void InitializeEmptyTenantNames()
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            SqlCommand cmdUpdate = new(@"
                    UPDATE ApplicantsTransfers
                    SET TenantName = 'VisualAcademy'
                    WHERE TenantName IS NULL OR TenantName = ''", connection);

            cmdUpdate.ExecuteNonQuery();

            connection.Close();
        }
    }
}
