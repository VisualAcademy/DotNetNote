namespace DotNetNote.Common;

//
// 로그인 실패 처리 관련 인터페이스/매니저/리포지토리/모델 구성
//

/// <summary>
/// 로그인 실패 처리용 인터페이스
/// Manager, Repository, Service 등 역할 분리 시 사용
/// </summary>
public interface ILoginFailedManager
{
    /// <summary>
    /// 계정이 잠금 상태인지 확인
    /// </summary>
    bool IsLoginFailed(string username);

    /// <summary>
    /// 로그인 실패 이력을 초기화
    /// </summary>
    void ClearLoginFailed(string username);
}

/// <summary>
/// 로그인 실패 처리 매니저 클래스
/// (강의에서 사용: LoginFailedManager, LoginFailedRepository, LoginFailedService 등)
/// </summary>
public class LoginFailedManager(ILoginFailedRepository repo) : ILoginFailedManager
{
    /// <summary>
    /// 로그인 실패 기록 초기화
    /// </summary>
    public void ClearLoginFailed(string username) => repo.ClearLogin(username);

    /// <summary>
    /// 로그인 실패 여부 확인  
    /// 5회 이상 실패 + 최근 10분 이내 시도 → 계정 잠금  
    /// 5회 이상 실패 + 10분 경과 → 초기화  
    /// 그 외 → 실패 카운트 증가  
    /// </summary>
    public bool IsLoginFailed(string username)
    {
        if (repo.IsLoginUser(username))
        {
            // 로그인 이력이 있는 유저
            // 카운터가 5 이상이고 최근 10분 내 로그인 시도면
            if (repo.IsFiveOverCount(username) && repo.IsLastLoginTenMinute(username))
            {
                // 계정 잠금 상태
                return true;
            }
            // 카운터가 5 이상이고 최근 10분이 지났으면 => 클리어
            else if (repo.IsFiveOverCount(username)
                && !repo.IsLastLoginTenMinute(username))
            {
                repo.ClearLogin(username); // 초기화
                return false;
            }
            else
            {
                // 실패 카운트 업데이트
                repo.UpdateLoginCount(username);
                return false; // 아직 잠금 전
            }
        }
        else
        {
            // 처음 로그인하는 사용자
            repo.AddLogin(new UserLog() { Username = username });
            return false; // 로그인 성공 상태(실패 기록 없음)
        }

        // return true; // 로그인 실패로 처리할 때 사용 가능
    }
}

/// <summary>
/// 로그인 실패 기록 모델 클래스
/// </summary>
public class UserLog
{
    /// <summary>
    /// PK
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 사용자 아이디
    /// </summary>
    public string Username { get; set; } = ""; 

    /// <summary>
    /// 비밀번호 실패 횟수
    /// </summary>
    public int FailedPasswordAttemptCount { get; set; }

    /// <summary>
    /// 실패 기준 시간(최근 시도 시간)
    /// </summary>
    public DateTime FailedPasswordAttemptWindowStart { get; set; }
}

/// <summary>
/// 로그인 실패 이력 리포지토리 인터페이스
/// </summary>
public interface ILoginFailedRepository
{
    /// <summary>
    /// 첫 로그인 시(UserLogs에 데이터 없을 때) 기록 생성
    /// </summary>
    UserLog AddLogin(UserLog model);

    /// <summary>
    /// 실패 카운트/시간 초기화
    /// </summary>
    void ClearLogin(string username);

    /// <summary>
    /// 지정한 사용자 이력 존재 여부
    /// </summary>
    bool IsLoginUser(string username);

    /// <summary>
    /// 실패 카운트 증가 + 기준 시간 갱신
    /// </summary>
    void UpdateLoginCount(string username);

    /// <summary>
    /// 실패 횟수 5회 이상인지 여부
    /// </summary>
    bool IsFiveOverCount(string username);

    /// <summary>
    /// 마지막 실패가 10분 이내인지 여부
    /// </summary>
    bool IsLastLoginTenMinute(string username);
}

/// <summary>
/// 로그인 실패 이력 리포지토리 구현(Dapper 기반)
/// </summary>
public class LoginFailedRepository : ILoginFailedRepository
{
    private SqlConnection db;
    private IConfiguration _config;

    /// <summary>
    /// DB 연결 초기화
    /// </summary>
    public LoginFailedRepository(IConfiguration config)
    {
        _config = config;
        db = new SqlConnection(
            _config.GetSection("ConnectionStrings")
                .GetSection("DefaultConnection").Value);
    }

    /// <summary>
    /// 첫 로그인 시 UserLogs에 insert
    /// </summary>
    public UserLog AddLogin(UserLog model)
    {
        var sql =
            "Insert Into UserLogs (Username) Values (@Username); " +
            "Select Cast(SCOPE_IDENTITY() As Int);";

        var id = db.Query<int>(sql, model).Single();
        model.Id = id;
        return model;
    }

    /// <summary>
    /// 실패 카운트와 기준 시간 초기화
    /// </summary>
    public void ClearLogin(string username)
    {
        var sql =
            "Update UserLogs                  " +
            "Set                            " +
            "    FailedPasswordAttemptCount = 0, " +
            "    FailedPasswordAttemptWindowStart = GetDate() " +
            "Where Username = @Username                 ";
        db.Execute(sql, new { Username = username });
    }

    /// <summary>
    /// 실패 카운트 5 이상인지 확인
    /// </summary>
    public bool IsFiveOverCount(string username)
    {
        string sql =
            "Select FailedPasswordAttemptCount From UserLogs Where Username = @Username";
        int r = db.Query<int>(sql, new { Username = username }).SingleOrDefault();
        return r >= 5;
    }

    /// <summary>
    /// 마지막 실패가 10분 이내인지 확인
    /// </summary>
    public bool IsLastLoginTenMinute(string username)
    {
        string sql = @"
                Select DateDiff(mi, FailedPasswordAttemptWindowStart, GetDate())
                From UserLogs 
                Where Username = @Username";
        int r = db.Query<int>(sql, new { Username = username }).SingleOrDefault();
        return r <= 10; // 10분 넘으면 false
    }

    /// <summary>
    /// UserLogs 테이블에 사용자 데이터 존재 여부
    /// </summary>
    public bool IsLoginUser(string username)
    {
        string sql = "Select Count(*) From UserLogs Where Username = @Username";
        int cnt = db.Query<int>(sql, new { Username = username }).Single();
        return cnt > 0; // 있으면 기존 사용자
    }

    /// <summary>
    /// 실패 카운트 증가 및 기준 시간 갱신
    /// </summary>
    public void UpdateLoginCount(string username)
    {
        var sql =
            "Update UserLogs                  " +
            "Set                            " +
            "    FailedPasswordAttemptCount = FailedPasswordAttemptCount + 1, " +
            "    FailedPasswordAttemptWindowStart = GetDate() " +
            "Where Username = @Username                 ";
        db.Execute(sql, new { Username = username });
    }
}
