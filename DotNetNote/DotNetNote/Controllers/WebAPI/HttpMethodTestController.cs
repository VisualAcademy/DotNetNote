using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HttpMethodTestController : ControllerBase
{
    // GET: api/<HttpMethodTestController>
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }

    // GET api/<HttpMethodTestController>/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
        return $"value {id}";
    }

    // POST api/<HttpMethodTestController>
    [HttpPost]
    public void Post([FromBody] string value)
    {

    }

    // PUT api/<HttpMethodTestController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {

    }

    // DELETE api/<HttpMethodTestController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {

    }
}
