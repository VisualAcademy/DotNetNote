namespace DotNetNote;

public class DotNetNoteUserStore : IUserStore<DotNetNoteUser>, IUserPasswordStore<DotNetNoteUser>
{
    public static IDbConnection GetDbConnection()
    {
        var con = new SqlConnection("server=(localdb)\\mssqllocaldb;database=DotNetNote;integrated security=true;");
        con.Open();
        return con;
    }

    public async Task<IdentityResult> CreateAsync(DotNetNoteUser user, CancellationToken cancellationToken)
    {
        string sql =
            "Insert Into DotNetNoteUsers(Id, UserName, NormalizedUserName, PasswordHash) " +
            "Values(@Id, @UserName, @NormalizedUserName, @PasswordHash)";
        using (var con = GetDbConnection())
        {
            await con.ExecuteAsync(sql, new
            {
                Id = user.Id,
                UserName = user.UserName,
                NormalizedUserName = user.NormalizedUserName,
                PasswordHash = user.PasswordHash
            });
        }

        return IdentityResult.Success;
    }

    public Task<IdentityResult> DeleteAsync(DotNetNoteUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {

    }

    public async Task<DotNetNoteUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        var sql = "Select * From DotNetNoteUsers Wher Id = @Id";
        using (var con = GetDbConnection())
        {
            return await con.QueryFirstOrDefaultAsync<DotNetNoteUser>(sql, new { Id = userId });
        }
    }

    public async Task<DotNetNoteUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        var sql = "Select * From DotNetNoteUsers Where NormalizedUserName = @NormalizedUserName";
        using (var con = GetDbConnection())
        {
            return await con.QueryFirstOrDefaultAsync<DotNetNoteUser>(sql, new { NormalizedUserName = normalizedUserName });
        }
    }

    public Task<string> GetNormalizedUserNameAsync(DotNetNoteUser user, CancellationToken cancellationToken) => Task.FromResult(user.NormalizedUserName);

    public Task<string> GetPasswordHashAsync(DotNetNoteUser user, CancellationToken cancellationToken) => Task.FromResult(user.PasswordHash);

    public Task<string> GetUserIdAsync(DotNetNoteUser user, CancellationToken cancellationToken) => Task.FromResult(user.Id);

    public Task<string> GetUserNameAsync(DotNetNoteUser user, CancellationToken cancellationToken) => Task.FromResult(user.UserName);

    public Task<bool> HasPasswordAsync(DotNetNoteUser user, CancellationToken cancellationToken) => Task.FromResult(user.PasswordHash != null);

    public Task SetNormalizedUserNameAsync(DotNetNoteUser user, string normalizedName, CancellationToken cancellationToken)
    {
        user.NormalizedUserName = normalizedName;
        return Task.CompletedTask;
    }

    public Task SetPasswordHashAsync(DotNetNoteUser user, string passwordHash, CancellationToken cancellationToken)
    {
        user.PasswordHash = passwordHash;
        return Task.CompletedTask;
    }

    public Task SetUserNameAsync(DotNetNoteUser user, string userName, CancellationToken cancellationToken)
    {
        user.UserName = userName;
        return Task.CompletedTask;
    }

    public async Task<IdentityResult> UpdateAsync(DotNetNoteUser user, CancellationToken cancellationToken)
    {
        string sql =
            "Update DotNetNoteUsers " +
            "Set Id = @Id, UserName = @UserName, NormalizedUserName = @NormalizedUserName, PasswordHash = @PasswordHash " +
            "Where Id = @Id";
        using (var con = GetDbConnection())
        {
            await con.ExecuteAsync(sql, new
            {
                Id = user.Id,
                UserName = user.UserName,
                NormalizedUserName = user.NormalizedUserName,
                PasswordHash = user.PasswordHash
            });
        }

        return IdentityResult.Success;
    }
}
