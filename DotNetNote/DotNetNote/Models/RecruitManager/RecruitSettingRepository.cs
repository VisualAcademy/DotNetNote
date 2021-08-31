using Dapper;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetNote.Models.RecruitManager
{
    public interface IRecruitSettingRepository
    {
        RecruitSetting Add(RecruitSetting model);               // 입력 
        Task<RecruitSetting> AddAsync(RecruitSetting model);    // 입력 

        List<RecruitSetting> GetAll();                          // 출력
        Task<IEnumerable<RecruitSetting>> GetAllAsync();        // 출력

        RecruitSetting GetById(int id);                         // 상세
        Task<RecruitSetting> GetByIdAsync(int id);              // 상세

        RecruitSetting Update(RecruitSetting model);            // 수정
        void Remove(int id);                                    // 삭제

        bool IsRecruitSettings(string boardName, int boardNum);


        bool IsClosedRecruit(string boardName, int boardNum);

        bool IsFinishedRecruit(string boardName, int boardNum);
    }

    public class RecruitSettingRepository : IRecruitSettingRepository
    {
        // 데이터베이스 연결 문자열 가져온 후 DB 개체 생성하기 
        private IConfiguration _config;
        private IDbConnection db;
        public RecruitSettingRepository(IConfiguration config)
        {
            _config = config;
            db = new SqlConnection(
                _config.GetSection("ConnectionStrings")
                    .GetSection("DefaultConnection").Value);
        }

        #region 모집 정보 설정 기록 
        /// <summary>
        /// 모집 정보 설정 기록 
        /// </summary>
        public RecruitSetting Add(RecruitSetting model)
        {
            var sql = @"
                Insert Into RecruitSettings (
                    Remarks,
                    BoardName, 
                    BoardNum, 
                    BoardTitle, 
                    BoardContent,
                    StartDate, 
                    EventDate, 
                    EndDate, 
                    MaxCount
                ) 
                Values (
                    @Remarks,
                    @BoardName, 
                    @BoardNum, 
                    @BoardTitle, 
                    @BoardContent,
                    @StartDate, 
                    @EventDate, 
                    @EndDate, 
                    @MaxCount
                ); 

                Select Cast(SCOPE_IDENTITY() As Int);
            ";
            var id = db.Query<int>(sql, model).Single();
            model.Id = id;
            return model;
        }
        /// <summary>
        /// 모집 정보 설정 기록 
        /// </summary>
        public async Task<RecruitSetting> AddAsync(RecruitSetting model)
        {
            var sql = @"
                Insert Into RecruitSettings (
                    Remarks,
                    BoardName, 
                    BoardNum, 
                    BoardTitle, 
                    BoardContent,
                    StartDate, 
                    EventDate, 
                    EndDate, 
                    MaxCount
                ) 
                Values (
                    @Remarks,
                    @BoardName, 
                    @BoardNum, 
                    @BoardTitle, 
                    @BoardContent,
                    @StartDate, 
                    @EventDate, 
                    @EndDate, 
                    @MaxCount
                ); 

                Select Cast(SCOPE_IDENTITY() As Int);
            ";
            var id = await db.QuerySingleAsync<int>(sql, model);
            model.Id = id;
            return model;
        } 
        #endregion

        #region 전체 모집 정보 출력
        /// <summary>
        /// 전체 모집 정보 출력
        /// </summary>
        public List<RecruitSetting> GetAll()
        {
            string sql = @"
                Select * 
                From RecruitSettings
                Order By Id Desc
            ";
            return db.Query<RecruitSetting>(sql).ToList();
        }
        /// <summary>
        /// 전체 모집 정보 출력
        /// </summary>
        public async Task<IEnumerable<RecruitSetting>> GetAllAsync()
        {
            string sql = @"
                Select * 
                From RecruitSettings
                Order By Id Desc
            ";
            return await db.QueryAsync<RecruitSetting>(sql);
        }
        #endregion

        #region 상세보기 액션 메서드들 
        /// <summary>
        /// 상세 
        /// </summary>
        public RecruitSetting GetById(int id)
        {
            string sql = @"
                Select * 
                From RecruitSettings
                Where Id = @Id 
            ";
            return db.Query<RecruitSetting>(sql,
                new { id }).SingleOrDefault();
        }
        /// <summary>
        /// 상세 
        /// </summary>
        public async Task<RecruitSetting> GetByIdAsync(int id)
        {
            string sql = @"
                Select * 
                From RecruitSettings
                Where Id = @Id 
            ";
            return await
                db.QuerySingleOrDefaultAsync<RecruitSetting>(sql, new { id });
        } 
        #endregion

        /// <summary>
        /// 모집 설정 정보 수정
        /// </summary>
        public RecruitSetting Update(RecruitSetting model)
        {
            var sql =
                " Update RecruitSettings                    " +
                " Set                                       " +
                "    Remarks       =       @Remarks,        " +
                "    BoardName     =       @BoardName,      " +
                "    BoardNum      =       @BoardNum,       " +
                "    BoardTitle    =       @BoardTitle,     " +
                "    BoardContent  =       @BoardContent,   " +
                "    StartDate     =       @StartDate,      " +
                "    EventDate     =       @EventDate,      " +
                "    EndDate       =       @EndDate,        " +
                "    MaxCount      =       @MaxCount        " +
                " Where Id = @Id                 ";
            db.Execute(sql, model);
            return model;
        }
        /// <summary>
        /// 모집 설정 정보 수정
        /// </summary>
        public RecruitSetting UpdateAsync(RecruitSetting model)
        {
            var sql =
                " Update RecruitSettings                    " +
                " Set                                       " +
                "    Remarks       =       @Remarks,        " +
                "    BoardName     =       @BoardName,      " +
                "    BoardNum      =       @BoardNum,       " +
                "    BoardTitle    =       @BoardTitle,     " +
                "    BoardContent  =       @BoardContent,   " +
                "    StartDate     =       @StartDate,      " +
                "    EventDate     =       @EventDate,      " +
                "    EndDate       =       @EndDate,        " +
                "    MaxCount      =       @MaxCount        " +
                " Where Id = @Id                 ";
            db.Execute(sql, model);
            return model;
        }

        /// <summary>
        /// 모집 정보 삭제
        /// </summary>
        public void Remove(int id)
        {
            string sql = "Delete From RecruitSettings Where Id = @Id";
            db.Execute(sql, new { Id = id });
        }

        /// <summary>
        /// 특정 게시판에 대한 모집 관련 세부 설정이 되었는지 안되었는지 확인
        /// </summary>
        /// <param name="boardName">게시판 별칭</param>
        /// <param name="boardNum">게시판 아티클 번호</param>
        /// <returns>True면 이미 세부 설정이 완료됨</returns>
        public bool IsRecruitSettings(string boardName, int boardNum)
        {
            var sqlCount = @"
                Select Count(*) 
                From RecruitSettings 
                Where 
                    BoardName = @BoardName 
                    And 
                    BoardNum = @BoardNum
            ";

            var count = db.Query<int>(sqlCount, new
            {
                BoardName = boardName,
                BoardNum = boardNum
            }).Single();

            if (count > 0)
            {
                return true; // 이미 이벤트 관련 세부 설정이 등록된 상태
            }
            return false;
        }

        /// <summary>
        /// 모집 종료: 최대 등록 인원을 0으로 설정하면 종료된 이벤트로 처리 
        /// </summary>
        public bool IsClosedRecruit(string boardName, int boardNum)
        {
            var sql = @"
                Select MaxCount 
                From RecruitSettings 
                Where 
                    BoardName = @BoardName 
                    And 
                    BoardNum = @BoardNum";
            var cnt = this.db.Query<int>(sql, new
            {
                BoardName = boardName,
                BoardNum = boardNum
            }).SingleOrDefault();

            if (cnt == 0)
            {
                return true; // 최대 등록자 수를 0으로 두면 종료된 이벤트
            }
            return false;
        }

        /// <summary>
        /// 모집 마감 여부 확인
        /// </summary>
        /// <param name="boardName">모집 게시판 별칭</param>
        /// <param name="boardNum">모집 게시판 번호</param>
        /// <returns>모집 마감(true), 마감 전(false)</returns>
        public bool IsFinishedRecruit(string boardName, int boardNum)
        {
            // 최대 모집 카운트
            var sqlCount1 = @"
                Select MaxCount From RecruitSettings 
                Where 
                    BoardName = @BoardName And BoardNum = @BoardNum";
            var count1 = db.Query<int>(
                sqlCount1,
                new
                {
                    BoardName = boardName,
                    BoardNum = boardNum
                }
                ).SingleOrDefault();

            // 모집 등록 카운
            var sqlCount2 = @"
                Select Count(*) From RecruitSettings 
                Where BoardName = @BoardName And BoardNum = @BoardNum";
            var count2 = db.Query<int>(
                sqlCount2,
                new
                {
                    BoardName = boardName,
                    BoardNum = boardNum
                }
                ).Single();

            // 모집에 등록된 숫자가 같거나, 더 많으면 마감된 모집로 봄
            if (count1 != 0 && count1 <= count2)
            {
                return true; // 모집 마감
            }

            return false; // 모집 중...
        }

    }
}
