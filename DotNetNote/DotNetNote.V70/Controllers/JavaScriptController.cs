using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

public class JavaScriptController : Controller
{
    public IActionResult Index() => View();

    public IActionResult Moment() => View();
}
