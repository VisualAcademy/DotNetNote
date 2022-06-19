using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace DotNetNote.Controllers;

[Route("api/[controller]")]
public class OneServiceController : Controller
{
    private IOneRepository _repository;

    public OneServiceController(IOneRepository repository) => _repository = repository;

    [HttpGet]
    public IEnumerable<One> Get()
    {
        return _repository.GetAll().AsEnumerable();
    }

    [HttpGet("{id}")]
    public One Get(int id)
    {
        return Get().Where(o => o.Id == id).Single();
    }

    [HttpPost]
    public One Post([FromBody] One model)
    {
        return _repository.Add(model);
    }
}
