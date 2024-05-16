using DotNetNote.Models.Ideas;
using System.Net;

namespace DotNetNote.Controllers;

/// <summary>
/// [7] ASP.NET Core Web API
/// </summary>
[Route("api/[controller]")]
public class IdeaServiceController(IIdeaRepository repository) : Controller
{
    [HttpGet]
    public IEnumerable<Idea> Get() =>
        // cRud
        repository.GetAll().AsEnumerable();

    [HttpGet("{id}")]
    public Idea Get(int id)
    {
        var idea = repository.GetAll().Where(m => m.Id == id).SingleOrDefault();
        if (idea == null)
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return null;
        }
        return idea;
    }

    [HttpPost]
    public JsonResult Post([FromBody] Idea model)
    {
        if (ModelState.IsValid)
        {
            // Crud
            var m = repository.Add(model);

            Response.StatusCode = (int)HttpStatusCode.Created;
            return Json(m);
        }
        else
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { Message = "실패", ModelState = ModelState });
        }
    }
}
