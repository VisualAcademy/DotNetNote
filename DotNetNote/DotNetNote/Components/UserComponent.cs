using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;

namespace DotNetNote.Components;

public class UserComponent
{
    // Empty
}

/// <summary>
/// 모델 클래스: Users 테이블과 일대일로 매핑
/// </summary>
public class UserModel
{
    public int UID { get; set; }
    public string UserID { get; set; } = "";
    public string Password { get; set; } = "";
    public string Username { get; set; } = "";
}

public interface IUserModelRepository
{
    UserModel? GetUserInfor(int uid);
    UserModel? GetUserInforCache(int uid);
    void RemoveUserInforCache(int uid);
}

public class UserModelRepository : IUserModelRepository
{
    private readonly IConfiguration _config;
    private readonly IDbConnection db;
    private readonly IMemoryCache _cache;

    public UserModelRepository(IConfiguration config, IMemoryCache memoryCache)
    {
        _config = config;
        db = new SqlConnection(
            _config.GetSection("ConnectionString").Value
            ?? throw new InvalidOperationException("ConnectionString is not configured."));
        _cache = memoryCache;
    }

    /// <summary>
    /// 상세 패턴: 저장 프로시저 사용
    /// </summary>
    /// <param name="uid">Id</param>
    /// <returns>T</returns>
    public UserModel? GetUserInfor(int uid)
    {
        string sql = "GetUsers";

        var parameters = new DynamicParameters();
        parameters.Add("@UID", value: uid, dbType: DbType.Int32, direction: ParameterDirection.Input);

        return db.Query<UserModel>(
            sql,
            parameters,
            commandType: CommandType.StoredProcedure).SingleOrDefault();
    }

    public UserModel? GetUserInforCache(int uid)
    {
        string sql = "GetUsers";

        var parameters = new DynamicParameters();
        parameters.Add("@UID", value: uid, dbType: DbType.Int32, direction: ParameterDirection.Input);

        UserModel? um;

        if (!_cache.TryGetValue($"GetUsers_{uid}", out um))
        {
            um = db.Query<UserModel>(
                sql,
                parameters,
                commandType: CommandType.StoredProcedure).SingleOrDefault();

            _cache.Set(
                $"GetUsers_{uid}",
                um,
                new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(60)));
        }

        return um;
    }

    // 특정 사용자 정보 캐싱 제거
    public void RemoveUserInforCache(int uid) => _cache.Remove($"GetUsers_{uid}");
}