using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

public class UrlsController(IUrlRepository repository) : Controller
{
    public IActionResult Index() => View("~/Views/_MiniProjects/Urls/Index.cshtml");

    public IActionResult IsExistsMethodTest()
    {
        string dnk = "test@visualacademy.com";

        bool r = repository.IsExists(dnk);

        ViewBag.IsExists = r;

        return View("~/Views/_MiniProjects/Urls/IsExistsMethodTest.cshtml");
    }
}
