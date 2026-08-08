namespace DotNetNote;

public class DotNetNoteUserStore :
    IUserStore<DotNetNoteUser>,
    IUserPasswordStore<DotNetNoteUser>
{
    private const string ConnectionString =
        "server=(localdb)\\mssqllocaldb;" +
        "database=DotNetNote;" +
        "integrated security=true;";

    public static IDbConnection GetDbConnection()
    {
        var connection = new SqlConnection(ConnectionString);
        connection.Open();

        return connection;
    }

    public async Task<IdentityResult> CreateAsync(
        DotNetNoteUser user,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);
        cancellationToken.ThrowIfCancellationRequested();

        const string sql = """
            INSERT INTO DotNetNoteUsers
            (
                Id,
                UserName,
                NormalizedUserName,
                PasswordHash
            )
            VALUES
            (
                @Id,
                @UserName,
                @NormalizedUserName,
                @PasswordHash
            )
            """;

        using var connection = GetDbConnection();

        var command = new CommandDefinition(
            sql,
            new
            {
                user.Id,
                user.UserName,
                user.NormalizedUserName,
                user.PasswordHash
            },
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(
        DotNetNoteUser user,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);
        cancellationToken.ThrowIfCancellationRequested();

        const string sql = """
            DELETE FROM DotNetNoteUsers
            WHERE Id = @Id
            """;

        using var connection = GetDbConnection();

        var command = new CommandDefinition(
            sql,
            new
            {
                user.Id
            },
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return IdentityResult.Success;
    }

    public async Task<DotNetNoteUser?> FindByIdAsync(
        string userId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        cancellationToken.ThrowIfCancellationRequested();

        const string sql = """
            SELECT
                Id,
                UserName,
                NormalizedUserName,
                PasswordHash
            FROM DotNetNoteUsers
            WHERE Id = @Id
            """;

        using var connection = GetDbConnection();

        var command = new CommandDefinition(
            sql,
            new
            {
                Id = userId
            },
            cancellationToken: cancellationToken);

        return await connection
            .QueryFirstOrDefaultAsync<DotNetNoteUser>(command);
    }

    public async Task<DotNetNoteUser?> FindByNameAsync(
        string normalizedUserName,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            normalizedUserName);

        cancellationToken.ThrowIfCancellationRequested();

        const string sql = """
            SELECT
                Id,
                UserName,
                NormalizedUserName,
                PasswordHash
            FROM DotNetNoteUsers
            WHERE NormalizedUserName = @NormalizedUserName
            """;

        using var connection = GetDbConnection();

        var command = new CommandDefinition(
            sql,
            new
            {
                NormalizedUserName = normalizedUserName
            },
            cancellationToken: cancellationToken);

        return await connection
            .QueryFirstOrDefaultAsync<DotNetNoteUser>(command);
    }

    public Task<string> GetUserIdAsync(
        DotNetNoteUser user,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(user.Id);
    }

    public Task<string?> GetUserNameAsync(
        DotNetNoteUser user,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult<string?>(user.UserName);
    }

    public Task<string?> GetNormalizedUserNameAsync(
        DotNetNoteUser user,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult<string?>(
            user.NormalizedUserName);
    }

    public Task SetUserNameAsync(
        DotNetNoteUser user,
        string? userName,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);
        cancellationToken.ThrowIfCancellationRequested();

        user.UserName = userName ?? string.Empty;

        return Task.CompletedTask;
    }

    public Task SetNormalizedUserNameAsync(
        DotNetNoteUser user,
        string? normalizedName,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);
        cancellationToken.ThrowIfCancellationRequested();

        user.NormalizedUserName = normalizedName ?? string.Empty;

        return Task.CompletedTask;
    }

    public Task SetPasswordHashAsync(
        DotNetNoteUser user,
        string? passwordHash,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);
        cancellationToken.ThrowIfCancellationRequested();

        user.PasswordHash = passwordHash ?? string.Empty;

        return Task.CompletedTask;
    }

    public Task<string?> GetPasswordHashAsync(
        DotNetNoteUser user,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult<string?>(
            user.PasswordHash);
    }

    public Task<bool> HasPasswordAsync(
        DotNetNoteUser user,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(
            !string.IsNullOrEmpty(user.PasswordHash));
    }

    public async Task<IdentityResult> UpdateAsync(
        DotNetNoteUser user,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);
        cancellationToken.ThrowIfCancellationRequested();

        const string sql = """
            UPDATE DotNetNoteUsers
            SET
                UserName = @UserName,
                NormalizedUserName = @NormalizedUserName,
                PasswordHash = @PasswordHash
            WHERE Id = @Id
            """;

        using var connection = GetDbConnection();

        var command = new CommandDefinition(
            sql,
            new
            {
                user.Id,
                user.UserName,
                user.NormalizedUserName,
                user.PasswordHash
            },
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return IdentityResult.Success;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}