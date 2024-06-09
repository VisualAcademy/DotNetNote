namespace DotNetNote.Controllers;

[Route("api/[controller]")]
public class OneServiceController(IOneRepository repository) : Controller
{
    [HttpGet]
    public IEnumerable<One> Get() => repository.GetAll().AsEnumerable();

    [HttpGet("{id}")]
    public One Get(int id) => Get().Where(o => o.Id == id).Single();

    [HttpPost]
    public One Post([FromBody] One model) => repository.Add(model);
}
