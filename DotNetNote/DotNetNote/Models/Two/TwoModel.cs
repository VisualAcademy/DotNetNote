namespace DotNetNote.Models;

/// <summary>
/// [1] 모델 클래스
/// </summary>
public class TwoModel
{
    public int Id { get; set; }

    public string Note { get; set; }
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
    private SqlConnection db;
    private IConfiguration _config;

    public TwoRepository(IConfiguration config)
    {
        _config = config;
        db = new SqlConnection(
                _config
                    .GetSection("ConnectionStrings")
                        .GetSection("DefaultConnection").Value);
    }

    public List<TwoModel> GetAll()
    {
        string sql = "Select * From Twos Order By Id Asc";
        return db.Query<TwoModel>(sql).ToList();
    }

    public TwoModel Add(TwoModel model)
    {
        string sql = @"
                Insert Into Twos (Note) Values (@Note);
                Select Cast(SCOPE_IDENTITY() As Int);
            ";
        var id = db.Query<int>(sql, model).Single();
        model.Id = id;
        return model;
    }
}
