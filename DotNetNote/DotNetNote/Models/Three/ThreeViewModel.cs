namespace DotNetNote.Models
{
    /// <summary>
    /// [1] 모델 클래스
    /// </summary>
    public class ThreeViewModel
    {
        public int Id { get; set; }

        public string Note { get; set; }
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
        private SqlConnection db;
        private IConfiguration _config;

        public ThreeRepository(IConfiguration config)
        {
            _config = config;
            db = new SqlConnection(
                    _config
                        .GetSection("ConnectionStrings")
                            .GetSection("DefaultConnection").Value);
        }

        public List<ThreeViewModel> GetAll()
        {
            string sql = "Select * From Threes Order By Id Asc";
            return db.Query<ThreeViewModel>(sql).ToList();
        }

        public ThreeViewModel Add(ThreeViewModel model)
        {
            string sql = @"
                Insert Into Threes (Note) Values (@Note);
                Select Cast(SCOPE_IDENTITY() As Int);
            ";
            var id = db.Query<int>(sql, model).Single();
            model.Id = id;
            return model;
        }

        public ThreeViewModel GetById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
