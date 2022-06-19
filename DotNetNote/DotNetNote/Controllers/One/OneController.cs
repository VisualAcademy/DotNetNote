using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

public class OneController : Controller
{
    private IOneRepository _repository;

    public OneController(IOneRepository repository) => _repository = repository;

    [HttpGet]
    public IActionResult Index()
    {
        var ones = _repository.GetAll();
        return View(ones);
    }

    [HttpPost]
    public IActionResult Index(One model)
    {
        _repository.Add(model);

        return RedirectToAction(nameof(Index));
    }
}
