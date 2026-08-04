using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace DotNetNote.Models.RecruitManager;

public interface IRecruitSettingRepository
{
    // 입력
    RecruitSetting Add(RecruitSetting model);

    Task<RecruitSetting> AddAsync(RecruitSetting model);

    // 출력
    List<RecruitSetting> GetAll();

    Task<IEnumerable<RecruitSetting>> GetAllAsync();

    // 상세: 데이터가 없으면 null
    RecruitSetting? GetById(int id);

    Task<RecruitSetting?> GetByIdAsync(int id);

    // 수정
    RecruitSetting Update(RecruitSetting model);

    Task<RecruitSetting> UpdateAsync(RecruitSetting model);

    // 삭제
    void Remove(int id);

    bool IsRecruitSettings(
        string boardName,
        int boardNum);

    bool IsClosedRecruit(
        string boardName,
        int boardNum);

    bool IsFinishedRecruit(
        string boardName,
        int boardNum);
}

public class RecruitSettingRepository : IRecruitSettingRepository
{
    private readonly IDbConnection db;

    public RecruitSettingRepository(IConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(config);

        var connectionString =
            config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "DefaultConnection 연결 문자열이 설정되지 않았습니다.");

        db = new SqlConnection(connectionString);
    }

    #region 모집 정보 설정 기록

    /// <summary>
    /// 모집 정보 설정 기록
    /// </summary>
    public RecruitSetting Add(RecruitSetting model)
    {
        ArgumentNullException.ThrowIfNull(model);

        const string sql = @"
            Insert Into RecruitSettings
            (
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
            Values
            (
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
    public async Task<RecruitSetting> AddAsync(
        RecruitSetting model)
    {
        ArgumentNullException.ThrowIfNull(model);

        const string sql = @"
            Insert Into RecruitSettings
            (
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
            Values
            (
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

        var id = await db.QuerySingleAsync<int>(
            sql,
            model);

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
        const string sql = @"
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
        const string sql = @"
            Select *
            From RecruitSettings
            Order By Id Desc
        ";

        return await db.QueryAsync<RecruitSetting>(sql);
    }

    #endregion

    #region 상세보기 액션 메서드

    /// <summary>
    /// 상세 정보를 반환합니다.
    /// 데이터가 없으면 null을 반환합니다.
    /// </summary>
    public RecruitSetting? GetById(int id)
    {
        const string sql = @"
            Select *
            From RecruitSettings
            Where Id = @Id
        ";

        return db.Query<RecruitSetting>(
            sql,
            new { Id = id })
            .SingleOrDefault();
    }

    /// <summary>
    /// 상세 정보를 비동기로 반환합니다.
    /// 데이터가 없으면 null을 반환합니다.
    /// </summary>
    public async Task<RecruitSetting?> GetByIdAsync(int id)
    {
        const string sql = @"
            Select *
            From RecruitSettings
            Where Id = @Id
        ";

        return await db.QuerySingleOrDefaultAsync<RecruitSetting>(
            sql,
            new { Id = id });
    }

    #endregion

    /// <summary>
    /// 모집 설정 정보 수정
    /// </summary>
    public RecruitSetting Update(RecruitSetting model)
    {
        ArgumentNullException.ThrowIfNull(model);

        const string sql = @"
            Update RecruitSettings
            Set
                Remarks = @Remarks,
                BoardName = @BoardName,
                BoardNum = @BoardNum,
                BoardTitle = @BoardTitle,
                BoardContent = @BoardContent,
                StartDate = @StartDate,
                EventDate = @EventDate,
                EndDate = @EndDate,
                MaxCount = @MaxCount
            Where Id = @Id
        ";

        db.Execute(sql, model);

        return model;
    }

    /// <summary>
    /// 모집 설정 정보 수정
    /// </summary>
    public async Task<RecruitSetting> UpdateAsync(
        RecruitSetting model)
    {
        ArgumentNullException.ThrowIfNull(model);

        const string sql = @"
            Update RecruitSettings
            Set
                Remarks = @Remarks,
                BoardName = @BoardName,
                BoardNum = @BoardNum,
                BoardTitle = @BoardTitle,
                BoardContent = @BoardContent,
                StartDate = @StartDate,
                EventDate = @EventDate,
                EndDate = @EndDate,
                MaxCount = @MaxCount
            Where Id = @Id
        ";

        await db.ExecuteAsync(sql, model);

        return model;
    }

    /// <summary>
    /// 모집 정보 삭제
    /// </summary>
    public void Remove(int id)
    {
        const string sql = @"
            Delete From RecruitSettings
            Where Id = @Id
        ";

        db.Execute(
            sql,
            new { Id = id });
    }

    /// <summary>
    /// 특정 게시판에 대한 모집 관련 세부 설정 여부를 확인합니다.
    /// </summary>
    public bool IsRecruitSettings(
        string boardName,
        int boardNum)
    {
        const string sql = @"
            Select Count(*)
            From RecruitSettings
            Where BoardName = @BoardName
                And BoardNum = @BoardNum
        ";

        var count = db.Query<int>(
            sql,
            new
            {
                BoardName = boardName,
                BoardNum = boardNum
            })
            .Single();

        return count > 0;
    }

    /// <summary>
    /// 최대 등록 인원이 0이면 종료된 모집으로 처리합니다.
    /// </summary>
    public bool IsClosedRecruit(
        string boardName,
        int boardNum)
    {
        const string sql = @"
            Select MaxCount
            From RecruitSettings
            Where BoardName = @BoardName
                And BoardNum = @BoardNum
        ";

        var count = db.Query<int>(
            sql,
            new
            {
                BoardName = boardName,
                BoardNum = boardNum
            })
            .SingleOrDefault();

        return count == 0;
    }

    /// <summary>
    /// 모집 마감 여부를 확인합니다.
    /// </summary>
    public bool IsFinishedRecruit(
        string boardName,
        int boardNum)
    {
        const string maxCountSql = @"
            Select MaxCount
            From RecruitSettings
            Where BoardName = @BoardName
                And BoardNum = @BoardNum
        ";

        var maxCount = db.Query<int>(
            maxCountSql,
            new
            {
                BoardName = boardName,
                BoardNum = boardNum
            })
            .SingleOrDefault();

        const string registeredCountSql = @"
            Select Count(*)
            From RecruitSettings
            Where BoardName = @BoardName
                And BoardNum = @BoardNum
        ";

        var registeredCount = db.Query<int>(
            registeredCountSql,
            new
            {
                BoardName = boardName,
                BoardNum = boardNum
            })
            .Single();

        return maxCount != 0 &&
               maxCount <= registeredCount;
    }
}