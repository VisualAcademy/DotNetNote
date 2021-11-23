using DotNetNote.Models.Ideas;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace DotNetNote.Controllers;

/// <summary>
/// [7] ASP.NET Core Web API
/// </summary>
[Route("api/[controller]")]
public class IdeaServiceController : Controller
{
    private IIdeaRepository _repository;
    public IdeaServiceController(IIdeaRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public IEnumerable<Idea> Get()
    {
        // cRud
        return _repository.GetAll().AsEnumerable();
    }

    [HttpGet("{id}")]
    public Idea Get(int id)
    {
        var idea = _repository.GetAll().Where(m => m.Id == id).SingleOrDefault();
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
            var m = _repository.Add(model);

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
