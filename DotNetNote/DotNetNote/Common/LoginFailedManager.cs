using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace DotNetNote.Common
{
    /// <summary>
    /// 인터페이스: Manager, Repository, Service, ...
    /// </summary>
    public interface ILoginFailedManager
    {
        bool IsLoginFailed(string username);
        void ClearLoginFailed(string username);
    }

    /// <summary>
    /// 리포지토리 클래스: LoginFailedManager, LoginFailedRepository, LoginFailedService, ...
    /// </summary>
    public class LoginFailedManager : ILoginFailedManager
    {
        private ILoginFailedRepository _repo;

        public LoginFailedManager(ILoginFailedRepository repo)
        {
            _repo = repo;
        }

        public void ClearLoginFailed(string username)
        {
            _repo.ClearLogin(username);
        }

        public bool IsLoginFailed(string username)
        {
            if (_repo.IsLoginUser(username))
            {
                // 로그인 유저
                // 카운터가 5 이상이고 최근 10분내 로그인 시도면
                // TODO: 
                if (_repo.IsFiveOverCount(username) && _repo.IsLastLoginTenMinute(username))
                {
                    // 조회 
                    return true; // 계정 잠금
                }
                // 카운터가 5 이상이고 최근 10분이 지났으면 => 클리어
                else if (_repo.IsFiveOverCount(username)
                    && !_repo.IsLastLoginTenMinute(username))
                {
                    _repo.ClearLogin(username);
                    return false;
                }
                else
                {
                    // 업데이트
                    _repo.UpdateLoginCount(username);
                    return false; // 아직은 계정 잠금 전 
                }
            }
            else
            {
                // 처음 로그인
                _repo.AddLogin(new UserLog() { Username = username });
                return false; // 로그인 성공
            }
            //return true; // 로그인 실패 
        }
    }

    /// <summary>
    /// 모델 클래스 
    /// </summary>
    public class UserLog
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int FailedPasswordAttemptCount { get; set; }
        public DateTime FailedPasswordAttemptWindowStart { get; set; }
    }

    /// <summary>
    /// 리포지토리 인터페이스
    /// </summary>
    public interface ILoginFailedRepository
    {
        UserLog AddLogin(UserLog model);
        void ClearLogin(string username);
        bool IsLoginUser(string username);
        void UpdateLoginCount(string username);
        bool IsFiveOverCount(string username);
        bool IsLastLoginTenMinute(string username);
    }

    /// <summary>
    /// 리포지토리 클래스 with Dapper
    /// </summary>
    public class LoginFailedRepository : ILoginFailedRepository
    {
        private SqlConnection db;
        private IConfiguration _config;

        public LoginFailedRepository(IConfiguration config)
        {
            _config = config;
            db = new SqlConnection(
                _config.GetSection("ConnectionStrings")
                    .GetSection("DefaultConnection").Value);
        }

        public UserLog AddLogin(UserLog model)
        {
            var sql =
                "Insert Into UserLogs (Username) Values (@Username); " +
                "Select Cast(SCOPE_IDENTITY() As Int);";

            var id = db.Query<int>(sql, model).Single();

            model.Id = id;
            return model;
        }

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

        public bool IsFiveOverCount(string username)
        {
            string sql =
                "Select FailedPasswordAttemptCount From UserLogs Where Username = @Username";
            int r = db.Query<int>(sql, new { Username = username }).SingleOrDefault();
            if (r >= 5)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsLastLoginTenMinute(string username)
        {
            string sql = @"
                Select DateDiff(mi, FailedPasswordAttemptWindowStart, GetDate())
                From UserLogs 
                Where Username = @Username";
            int r = db.Query<int>(sql, new { Username = username }).SingleOrDefault();
            if (r <= 10)
            {
                return true; // 10분에 로그인한 사용자
            }
            else
            {
                return false; // 10분 지남 => 클리어
            }
        }

        public bool IsLoginUser(string username)
        {
            string sql = "Select Count(*) From UserLogs Where Username = @Username";
            int cnt = db.Query<int>(sql, new { Username = username }).Single();

            if (cnt > 0)
            {
                return true; // 기존에 로그인한 사용자 
            }
            else
            {
                return false; // 한번도 로그인하지 않은 사용자 
            }
        }

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
}
