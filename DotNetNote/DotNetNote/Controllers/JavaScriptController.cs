using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

public class JavaScriptController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Moment() => View();
}
