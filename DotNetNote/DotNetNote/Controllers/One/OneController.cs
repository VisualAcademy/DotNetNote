using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

public class OneController(IOneRepository repository) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        var ones = repository.GetAll();
        return View(ones);
    }

    [HttpPost]
    public IActionResult Index(One model)
    {
        repository.Add(model);

        return RedirectToAction(nameof(Index));
    }
}
