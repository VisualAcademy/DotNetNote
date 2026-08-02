namespace DotNetNote.Models;

/// <summary>
/// [1] 모델 클래스
/// </summary>
public class ThreeViewModel
{
    public int Id { get; set; }

    public string Note { get; set; } = string.Empty;
}

/// <summary>
/// [2] 리포지토리 인터페이스
/// </summary>
public interface IThreeRepository
{
    ThreeViewModel Add(ThreeViewModel model);

    List<ThreeViewModel> GetAll();

    ThreeViewModel GetById(int id);
}

/// <summary>
/// [3] 리포지토리 클래스
/// </summary>
public class ThreeRepository : IThreeRepository
{
    private readonly SqlConnection _db;

    public ThreeRepository(IConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(config);

        var connectionString =
            config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "DefaultConnection 연결 문자열이 설정되지 않았습니다.");

        _db = new SqlConnection(connectionString);
    }

    public List<ThreeViewModel> GetAll()
    {
        const string sql = """
            SELECT Id, Note
            FROM Threes
            ORDER BY Id ASC
            """;

        return _db.Query<ThreeViewModel>(sql).ToList();
    }

    public ThreeViewModel Add(ThreeViewModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        const string sql = """
            INSERT INTO Threes (Note)
            VALUES (@Note);

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        var id = _db.Query<int>(sql, model).Single();

        model.Id = id;

        return model;
    }

    public ThreeViewModel GetById(int id)
    {
        throw new NotImplementedException();
    }
}