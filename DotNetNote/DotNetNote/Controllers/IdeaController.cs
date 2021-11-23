using DotNetNote.Models.Ideas;
using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

/// <summary>
/// [5] ASP.NET 컨트롤러 클래스
/// </summary>
public class IdeaController : Controller
{
    private IIdeaRepository _repository;

    public IdeaController(IIdeaRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var ideas = _repository.GetAll();
        return View(ideas);
    }

    [HttpPost]
    public IActionResult Index(Idea model)
    {
        if (ModelState.IsValid)
        {
            model = _repository.Add(model);
            return RedirectToAction(nameof(Index));
        }
        else
        {
            return View(model);
        }
    }
}
