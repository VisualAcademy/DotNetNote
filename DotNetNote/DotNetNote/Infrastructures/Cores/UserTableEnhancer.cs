namespace Dalbodre;

public class UserTableEnhancer(string connectionString)
{
    public async Task<bool> TableExistsAsync(string tableName)
    {
        using var connection = new SqlConnection(connectionString);

        await connection.OpenAsync();

        using var checkTableCmd = new SqlCommand
        {
            Connection = connection,
            CommandText = """
                SELECT COUNT(*)
                FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_SCHEMA = 'dbo'
                  AND TABLE_NAME = @tableName;
                """,
            CommandType = System.Data.CommandType.Text
        };

        checkTableCmd.Parameters.AddWithValue("@tableName", tableName);

        var result = await checkTableCmd.ExecuteScalarAsync();

        var tableCount = result is int count
            ? count
            : 0;

        return tableCount > 0;
    }

    public async Task AddColumnIfNotExistsAsync(
        string tableName,
        string columnName,
        string columnDefinition)
    {
        using var connection = new SqlConnection(connectionString);

        await connection.OpenAsync();

        using var checkAndAddColumnCmd = new SqlCommand
        {
            Connection = connection,
            CommandText = $"""
                IF NOT EXISTS
                (
                    SELECT 1
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = 'dbo'
                      AND TABLE_NAME = @tableName
                      AND COLUMN_NAME = @columnName
                )
                BEGIN
                    ALTER TABLE dbo.{tableName}
                    ADD {columnName} {columnDefinition};
                END;
                """,
            CommandType = System.Data.CommandType.Text
        };

        checkAndAddColumnCmd.Parameters.AddWithValue(
            "@tableName",
            tableName);

        checkAndAddColumnCmd.Parameters.AddWithValue(
            "@columnName",
            columnName);

        await checkAndAddColumnCmd.ExecuteNonQueryAsync();
    }

    public async Task EnsureUserTableColumnsAsync()
    {
        if (!await TableExistsAsync("AspNetUsers"))
        {
            return;
        }

        await AddColumnIfNotExistsAsync(
            "AspNetUsers",
            "CriminalHistory",
            "NVARCHAR(MAX) NULL");

        await AddColumnIfNotExistsAsync(
            "AspNetUsers",
            "SecondaryPhone",
            "NVARCHAR(50) NULL");

        await AddColumnIfNotExistsAsync(
            "AspNetUsers",
            "MiddleName",
            "NVARCHAR(50) NULL");

        await AddColumnIfNotExistsAsync(
            "AspNetUsers",
            "SSN",
            "NVARCHAR(50) NULL");
    }
}