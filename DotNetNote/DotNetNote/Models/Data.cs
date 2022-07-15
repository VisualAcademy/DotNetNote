using System.Linq;
using System.Threading.Tasks;

namespace DotNetNote.Models
{
    public class DataModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }
    }

    public class DataService
    {
        private readonly List<DataModel> _data = new List<DataModel>()
        {
            new DataModel { Id = 1, Name = "김태영", Title = "안녕하세요." },
            new DataModel { Id = 2, Name = "박용준", Title = "반갑습니다." },
            new DataModel { Id = 3, Name = "한상훈", Title = "또 만나요." },
        };

        public List<DataModel> GetAll() => _data;

        public DataModel GetDataById(int id)
        {
            return _data.Where(n => n.Id == id).SingleOrDefault();
        }

        public List<DataModel> GetDataByName(string name)
        {
            return _data.Where(
                n => n.Name.ToLower().Equals(name.ToLower())).ToList();
        }
    }

    public class DataFinder
    {
        private DataService _service = new DataService();

        public async Task<DataModel> GetDataById(int id)
        {
            return await Task.FromResult(_service.GetDataById(id));
        }
    }
}
