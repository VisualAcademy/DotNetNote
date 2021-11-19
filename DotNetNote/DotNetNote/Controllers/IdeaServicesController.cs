using DotNetNote.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace DotNetNote.Controllers;

/// <summary>
/// [5] Web API 컨트롤러 클래스 
/// </summary>
[Route("api/[controller]")]
public class IdeaServicesController : Controller
{
    private IIdeaRepository _repository;
    public IdeaServicesController(IIdeaRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// /api/IdeaServices 
    /// </summary>
    [HttpGet]
    public IEnumerable<Idea> Get()
    {
        // cRud
        //return _repository.GetAll().AsEnumerable();
        return _repository.GetAll().ToList();
    }

    /// <summary>
    /// /api/IdeaServices/1234
    /// </summary>
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

    /// <summary>
    /// POST: /api/IdeaServices - JSON - T
    /// </summary>
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
