namespace Dalbodre.Infrastructures.Tenants;

public class TenantSchemaEnhancerAddColumnApplications(
    string masterConnectionString)
{
    public void EnhanceAllTenantDatabases()
    {
        List<string> tenantConnectionStrings =
            GetTenantConnectionStrings();

        foreach (var connectionString in tenantConnectionStrings)
        {
            EnsureNewColumns(connectionString);
        }
    }

    private List<string> GetTenantConnectionStrings()
    {
        var result = new List<string>();

        using var connection =
            new SqlConnection(masterConnectionString);

        connection.Open();

        const string query = """
            SELECT ConnectionString
            FROM dbo.Tenants
            WHERE ConnectionString IS NOT NULL;
            """;

        using var command =
            new SqlCommand(query, connection);

        using var reader =
            command.ExecuteReader();

        var connectionStringOrdinal =
            reader.GetOrdinal("ConnectionString");

        while (reader.Read())
        {
            if (reader.IsDBNull(connectionStringOrdinal))
            {
                continue;
            }

            var tenantConnectionString =
                reader.GetString(connectionStringOrdinal);

            if (!string.IsNullOrWhiteSpace(tenantConnectionString))
            {
                result.Add(tenantConnectionString);
            }
        }

        return result;
    }

    private static void EnsureNewColumns(string connectionString)
    {
        using var connection =
            new SqlConnection(connectionString);

        connection.Open();

        var columnsToAdd =
            new List<(string Name, string Definition)>
            {
                (
                    "PortalName",
                    "nvarchar(max) DEFAULT('Hawaso') NULL"
                ),
                (
                    "Language",
                    "nvarchar(255) DEFAULT('en-US') NULL"
                )
            };

        foreach (var column in columnsToAdd)
        {
            const string checkColumnQuery = """
                SELECT COUNT(*)
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = 'dbo'
                  AND TABLE_NAME = 'Applications'
                  AND COLUMN_NAME = @ColumnName;
                """;

            using var checkCommand =
                new SqlCommand(checkColumnQuery, connection);

            checkCommand.Parameters.Add(
                "@ColumnName",
                System.Data.SqlDbType.NVarChar,
                128
            ).Value = column.Name;

            var result =
                checkCommand.ExecuteScalar();

            var columnCount =
                Convert.ToInt32(result);

            if (columnCount > 0)
            {
                continue;
            }

            var addColumnQuery = $"""
                ALTER TABLE [dbo].[Applications]
                ADD [{column.Name}] {column.Definition};
                """;

            using var addColumnCommand =
                new SqlCommand(addColumnQuery, connection);

            addColumnCommand.ExecuteNonQuery();
        }
    }
}