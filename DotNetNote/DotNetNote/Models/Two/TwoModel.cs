namespace DotNetNote.Models;

/// <summary>
/// [1] 모델 클래스
/// </summary>
public class TwoModel
{
    public int Id { get; set; }

    public string Note { get; set; } = string.Empty;
}

/// <summary>
/// [2] 리포지토리 인터페이스
/// </summary>
public interface ITwoRepository
{
    TwoModel Add(TwoModel model);

    List<TwoModel> GetAll();
}

/// <summary>
/// [3] 리포지토리 클래스
/// </summary>
public class TwoRepository : ITwoRepository
{
    private readonly SqlConnection _db;

    public TwoRepository(IConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(config);

        var connectionString =
            config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "DefaultConnection 연결 문자열이 설정되지 않았습니다.");

        _db = new SqlConnection(connectionString);
    }

    public List<TwoModel> GetAll()
    {
        const string sql = """
            SELECT Id, Note
            FROM Twos
            ORDER BY Id ASC
            """;

        return _db.Query<TwoModel>(sql).ToList();
    }

    public TwoModel Add(TwoModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        const string sql = """
            INSERT INTO Twos (Note)
            VALUES (@Note);

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        var id = _db.Query<int>(sql, model).Single();

        model.Id = id;

        return model;
    }
}