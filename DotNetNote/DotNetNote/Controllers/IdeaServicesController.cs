using DotNetNote.Models.Ideas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        // return _repository.GetAll().AsEnumerable();
        repository.GetAll().ToList();

    /// <summary>
    /// /api/IdeaServices/1234
    /// </summary>
    [HttpGet("{id}")]
    public ActionResult<Idea> Get(int id)
    {
        var idea = repository.GetAll().SingleOrDefault(m => m.Id == id);

        if (idea == null)
        {
            return NotFound();
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

            Response.StatusCode = StatusCodes.Status201Created;
            return Json(m);
        }
        else
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return Json(new { Message = "실패", ModelState });
        }
    }
}