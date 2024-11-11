namespace Dalbodre.Infrastructures.Tenants;

public class TenantSchemaEnhancerAddColumnApplications(string masterConnectionString)
{
    public void EnhanceAllTenantDatabases()
    {
        List<string> tenantConnectionStrings = GetTenantConnectionStrings();

        foreach (string connStr in tenantConnectionStrings)
        {
            EnsureNewColumns(connStr);
        }
    }

    private List<string> GetTenantConnectionStrings()
    {
        List<string> result = new List<string>();

        using (SqlConnection connection = new SqlConnection(masterConnectionString))
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

    private void EnsureNewColumns(string connectionString)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            List<(string Name, string Type)> columnsToAdd = new List<(string, string)>
            {
                ("PortalName", "nvarchar(max) Default('Hawaso') NULL"),
                ("Language", "nvarchar(255) Default('en-US') NULL"),
            };

            foreach (var column in columnsToAdd)
            {
                SqlCommand cmdCheck = new SqlCommand($@"
                        SELECT COUNT(*) 
                        FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'Applications' 
                        AND COLUMN_NAME = '{column.Name}'", connection);

                int columnCount = (int)cmdCheck.ExecuteScalar();
                if (columnCount == 0)
                {
                    SqlCommand cmdAddColumn = new SqlCommand($@"
                            ALTER TABLE [dbo].[Applications]
                            ADD [{column.Name}] {column.Type}", connection);
                    cmdAddColumn.ExecuteNonQuery();
                }
            }

            connection.Close();
        }
    }
}
