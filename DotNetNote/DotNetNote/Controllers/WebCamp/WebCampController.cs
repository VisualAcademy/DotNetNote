using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

public class WebCampController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
