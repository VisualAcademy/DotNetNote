using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

public class TabManagerController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
