namespace ScaffoldingDemo.Controllers;

[Produces("application/json")]
[Route("api/DefaultApi")]
public class DefaultApiController : Controller
{
    // GET: api/DefaultApi
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }

    // GET: api/DefaultApi/5
    [HttpGet("{id}", Name = "Get")]
    public string Get(int id)
    {
        return "value";
    }
    
    // POST: api/DefaultApi
    [HttpPost]
    public void Post([FromBody]string value)
    {
    }
    
    // PUT: api/DefaultApi/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody]string value)
    {
    }
    
    // DELETE: api/ApiWithActions/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
