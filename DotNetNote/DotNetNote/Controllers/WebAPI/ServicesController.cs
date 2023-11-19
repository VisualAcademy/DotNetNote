using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DotNetNote.Controllers;

[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    [HttpGet]
    [Produces("application/json")]
    public IEnumerable<string> Get() => new string[] { "안녕하세요.", "반갑습니다." };

    //[HttpGet("{id?}")] // 생략 가능
    //[HttpGet("{id=1000}")] // 기본 값
    [HttpGet("{id:int}")] // 제약조건
    //public string Get([FromRoute]int id, [FromQuery]string query)
    public IActionResult Get([FromRoute] int id, [FromQuery] string query) =>
        //return $"넘어온 값: {id}, {query}";
        Ok(new Dto { Id = id, Text = $"값: {id}" });

    [HttpPost]
    //public void Post([FromBody]Dto value)
    public IActionResult Post([FromBody] Dto value)
    {
        if (!ModelState.IsValid)
        {
            // throw new InvalidOperationException("잘못되었습니다.");
            return BadRequest(ModelState); // 400 Bad Request
        }

        // 데이터 저장 후 Identity 값 반환
        value.Id++;

        return CreatedAtAction("Get", new { id = value.Id }, value); // 201 Created
    }

    [HttpPut("{id}")]
    public void Put(int id, [FromBody] Dto value)
    {

    }

    [HttpDelete("{id}")]
    public void Delete(int id)
    {

    }
}

public class Dto
{
    public int Id { get; set; }

    [MinLength(5)]
    public string Text { get; set; }
}
