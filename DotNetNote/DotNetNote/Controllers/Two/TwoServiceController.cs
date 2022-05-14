using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

[Route("api/[controller]")]
public class TwoServiceController : Controller
{
    private ITwoRepository _repository;

    public TwoServiceController(ITwoRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            var twos = _repository.GetAll();
            if (twos == null)
            {
                return NotFound($"아무런 데이터가 없습니다.");
            }
            return Ok(twos);
        }
        catch
        {

        }
        return BadRequest();
    }

    [HttpPost]
    public TwoModel Post([FromBody] TwoModel model)
    {
        return _repository.Add(model);
    }
}
