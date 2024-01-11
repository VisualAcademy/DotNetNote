using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace DotNetNote.Controllers;

[Route("api/[controller]")]
public class ThreeServiceController(IThreeRepository repository) : Controller
{
    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            var threes = repository.GetAll();
            if (threes == null)
            {
                return NotFound($"아무런 데이터가 없습니다.");
            }
            return Ok(threes);
        }
        catch
        {

        }
        return BadRequest();
    }

    [HttpPost]
    [Produces("application/json", Type = typeof(ThreeViewModel))]
    [Consumes("application/json")]
    public IActionResult Post([FromBody] ThreeViewModel model)
    {
        try
        {
            // 모델 유효성 검사
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // 400 에러 출력
            }
            var m = repository.Add(model);
            return CreatedAtAction("Get", new { id = m.Id }, m); // 201
        }
        catch
        {
        }
        return BadRequest();
    }

    [HttpGet("{id:int}")]
    public ThreeViewModel Get(int id) => repository.GetAll().Where(m => m.Id == id).Single();
}
