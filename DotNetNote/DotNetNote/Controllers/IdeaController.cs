using DotNetNote.Models.Ideas;
using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

/// <summary>
/// [5] ASP.NET 컨트롤러 클래스
/// </summary>
public class IdeaController(IIdeaRepository repository) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        var ideas = repository.GetAll();
        return View(ideas);
    }

    [HttpPost]
    public IActionResult Index(Idea model)
    {
        if (ModelState.IsValid)
        {
            model = repository.Add(model);
            return RedirectToAction(nameof(Index));
        }
        else
        {
            return View(model);
        }
    }
}
