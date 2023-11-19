using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

public class UrlsController : Controller
{
    private readonly IUrlRepository _repository;

    public UrlsController(IUrlRepository repository) => _repository = repository;

    public IActionResult Index() => View("~/Views/_MiniProjects/Urls/Index.cshtml");

    public IActionResult IsExistsMethodTest()
    {
        string dnk = "test@visualacademy.com";

        bool r = _repository.IsExists(dnk);

        ViewBag.IsExists = r;

        return View("~/Views/_MiniProjects/Urls/IsExistsMethodTest.cshtml");
    }
}
