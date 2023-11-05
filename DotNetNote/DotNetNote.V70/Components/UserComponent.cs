using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;

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
    public string UserID { get; set; }
    public string Password { get; set; }
    public string Username { get; set; }
}

public interface IUserModelRepository
{
    UserModel GetUserInfor(int uid);
    UserModel GetUserInforCache(int uid);
    void RemoveUserInforCache(int uid);
}

public class UserModelRepository : IUserModelRepository
{
    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-5.0
    private IConfiguration _config;
    private IDbConnection db;
    private IMemoryCache _cache;

    public UserModelRepository(IConfiguration config, IMemoryCache memoryCache)
    {
        _config = config;
        db = new SqlConnection(_config.GetSection("ConnectionString").Value);

        _cache = memoryCache;
    }

    /// <summary>
    /// 상세 패턴: 저장 프로시저 사용
    /// </summary>
    /// <param name="uid">Id</param>
    /// <returns>T</returns>
    public UserModel GetUserInfor(int uid)
    {
        // 저장 프로시저 이름 또는 인라인 SQL 문(Ad HOC 쿼리)
        string sql = "GetUsers";

        // 파라미터 추가
        var parameters = new DynamicParameters();
        parameters.Add("@UID", value: uid, dbType: DbType.Int32, direction: ParameterDirection.Input);

        // 저장 프로시저 실행
        return db.Query<UserModel>(sql, parameters, commandType: CommandType.StoredProcedure).SingleOrDefault();
    }

    public UserModel GetUserInforCache(int uid)
    {
        // 저장 프로시저 이름 또는 인라인 SQL 문(Ad HOC 쿼리)
        string sql = "GetUsers";

        // 파라미터 추가
        var parameters = new DynamicParameters();
        parameters.Add("@UID", value: uid, dbType: DbType.Int32, direction: ParameterDirection.Input);

        // 저장 프로시저 실행
        // var result = db.Query<UserModel>(sql, parameters, commandType: CommandType.StoredProcedure).SingleOrDefault();
        // return result;

        // 캐시에 담을 개체
        UserModel um;

        // 캐시에 데이터가 들어있으면 해당 데이터를 가져오기
        if (!_cache.TryGetValue($"GetUsers_{uid}", out um))
        {
            // 캐시에 개체 값을 담기
            um = db.Query<UserModel>(sql, parameters, commandType: CommandType.StoredProcedure).SingleOrDefault();

            // 캐시에 현재 사용자 정보를 담기
            _cache.Set($"GetUsers_{uid}", um, (new MemoryCacheEntryOptions()).SetAbsoluteExpiration(TimeSpan.FromSeconds(60)));
        }

        return um;
    }

    // 특정 사용자 정보 캐싱 제거
    public void RemoveUserInforCache(int uid) => _cache.Remove($"GetUsers_{uid}");
}
