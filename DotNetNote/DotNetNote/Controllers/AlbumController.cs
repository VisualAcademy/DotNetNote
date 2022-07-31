using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

public class AlbumController : Controller
{
    public IActionResult Index()
    {
        //return View();
        return View("Default");
    }
}
