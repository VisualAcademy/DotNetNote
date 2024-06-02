namespace DotNetNote.Controllers;

[Route("api/[controller]")]
public class TwoServiceController(ITwoRepository repository) : Controller
{
    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            var twos = repository.GetAll();
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
    public TwoModel Post([FromBody] TwoModel model) => repository.Add(model);
}
