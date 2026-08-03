using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace DotNetNote.Models;

/// <summary>
/// 모델 클래스
/// </summary>
public class CharacterModel
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public int HeroId { get; set; }
}

/// <summary>
/// 리포지토리 인터페이스
/// </summary>
public interface ICharacterRepository
{
    CharacterModel SetCharacter(CharacterModel model);

    CharacterModel? GetCharacterByUsername(string username);
}

/// <summary>
/// 리포지토리 클래스
/// </summary>
public class CharacterRepository : ICharacterRepository
{
    private readonly IDbConnection _db;

    /// <summary>
    /// 생성자
    /// </summary>
    public CharacterRepository(IConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(config);

        var connectionString =
            config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "DefaultConnection 연결 문자열이 설정되지 않았습니다.");

        _db = new SqlConnection(connectionString);
    }

    /// <summary>
    /// 캐릭터 선택: 처음 입력하거나 기존 정보를 업데이트합니다.
    /// </summary>
    public CharacterModel SetCharacter(CharacterModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        if (string.IsNullOrWhiteSpace(model.Username))
        {
            throw new ArgumentException(
                "Username은 비어 있을 수 없습니다.",
                nameof(model));
        }

        if (GetRecordCounts(model.Username) > 0)
        {
            const string updateSql = """
                UPDATE Characters
                SET HeroId = @HeroId
                WHERE Username = @Username
                """;

            _db.Execute(updateSql, model);

            return model;
        }

        const string insertSql = """
            INSERT INTO Characters
            (
                Username,
                HeroId
            )
            VALUES
            (
                @Username,
                @HeroId
            );

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        var id = _db.Query<int>(insertSql, model).Single();

        model.Id = id;

        return model;
    }

    /// <summary>
    /// 특정 사용자 ID에 해당하는 캐릭터 설정이 있는지 확인합니다.
    /// </summary>
    public int GetRecordCounts(string username)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        const string sql = """
            SELECT COUNT(*)
            FROM Characters
            WHERE Username = @Username
            """;

        return _db.Query<int>(
            sql,
            new { Username = username })
            .Single();
    }

    /// <summary>
    /// 특정 사용자의 캐릭터 정보를 반환합니다.
    /// 캐릭터가 설정되지 않은 경우 null을 반환합니다.
    /// </summary>
    public CharacterModel? GetCharacterByUsername(string username)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        const string sql = """
            SELECT Id, Username, HeroId
            FROM Characters
            WHERE Username = @Username
            """;

        return _db.Query<CharacterModel>(
            sql,
            new { Username = username })
            .SingleOrDefault();
    }
}