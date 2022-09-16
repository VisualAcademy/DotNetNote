using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

/// <summary>
/// HttpMethodTestController_닷넷 6 Web API 기본 템플릿을 사용하여 Get, Post, Put, Delete 메서드 테스트
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class HttpMethodTestController : ControllerBase
{
    // GET: api/<HttpMethodTestController>
    [HttpGet]
    public IEnumerable<string> Get() => new string[] { "value1", "value2" };

    // GET api/<HttpMethodTestController>/5
    [HttpGet("{id}")]
    public string Get(int id) => $"value {id}";

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
