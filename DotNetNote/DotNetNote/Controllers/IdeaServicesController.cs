using DotNetNote.Models.Ideas;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net;

namespace DotNetNote.Controllers;

/// <summary>
/// [5] Web API 컨트롤러 클래스 
/// </summary>
[Route("api/[controller]")]
public class IdeaServicesController(IIdeaRepository repository) : Controller
{
    /// <summary>
    /// /api/IdeaServices 
    /// </summary>
    [HttpGet]
    public IEnumerable<Idea> Get() =>
        // cRud
        //return _repository.GetAll().AsEnumerable();
        repository.GetAll().ToList();

    /// <summary>
    /// /api/IdeaServices/1234
    /// </summary>
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

    /// <summary>
    /// POST: /api/IdeaServices - JSON - T
    /// </summary>
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
