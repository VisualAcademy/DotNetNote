using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

/// <summary>
/// WebApiCampsController 대신에 WebApiCamps만 지정해도 사용 가능하다. 
/// </summary>
[Route("api/[controller]")]
public class WebApiCamps : Controller
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { Id = 1, Name = "Web API" }); 
    }

    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }

    [HttpPost]
    public void Post([FromBody]string value)
    {
    }

    [HttpPut("{id}")]
    public void Put(int id, [FromBody]string value)
    {
    }

    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
