namespace DotNetNote.Models;

/// <summary>
/// 리포지토리 클래스 
/// </summary>
public class OneRepository : IOneRepository
{
    private SqlConnection db;
    private IConfiguration _config;

    public OneRepository(IConfiguration config)
    {
        _config = config;
        db = new SqlConnection(
                _config
                    .GetSection("ConnectionStrings")
                        .GetSection("DefaultConnection").Value);
    }

    public List<One> GetAll()
    {
        string sql = "Select * From Ones Order By Id Asc";
        return db.Query<One>(sql).ToList();
    }

    public One Add(One model)
    {
        string sql = @"
                Insert Into Ones (Note) Values (@Note);
                Select Cast(SCOPE_IDENTITY() As Int);
            ";
        var id = db.Query<int>(sql, model).Single();
        model.Id = id;
        return model;
    }
}
