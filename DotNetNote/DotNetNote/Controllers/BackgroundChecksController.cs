using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

public class BackgroundChecksController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
