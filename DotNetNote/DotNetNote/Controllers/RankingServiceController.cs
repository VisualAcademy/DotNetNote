using AngularNote.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace DotNetNote.Controllers;

[Route("api/[controller]")]
public class RankingServiceController : Controller
{
    // GET: api/RankingService
    [HttpGet]
    public IEnumerable<Ranking> Get()
    {
        List<Ranking> lst = new List<Ranking>();
        lst.Add(new Ranking { Id = 1, Name = "홍길동", Rank = 1, RankImage = "One.gif" });
        lst.Add(new Ranking { Id = 2, Name = "백두산", Rank = 2, RankImage = "Two.gif" });
        lst.Add(new Ranking { Id = 3, Name = "백두산3", Rank = 3, RankImage = "" });
        lst.Add(new Ranking { Id = 4, Name = "백두산4", Rank = 4, RankImage = "" });

        return lst.AsEnumerable();
    }

    // GET api/RankingService/1
    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }

    // POST api/RankingService
    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

    // PUT api/RankingService/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/RankingService/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
