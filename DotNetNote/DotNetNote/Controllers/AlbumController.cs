using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

public class AlbumController : Controller
{
    public IActionResult Index() => View("Default"); // return View();
}
