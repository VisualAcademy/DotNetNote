using DotNetNote.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

public class SingletonDemoController(IInfoService svc) : Controller
{
    public IActionResult ConstructorInjectionDemo()
    {
        ViewData["Url"] = svc.GetUrl();
        return View("Index");
    }

    public IActionResult Index()
    {
        ViewData["Url"] = "www.gilbut.co.kr";
        return View();
    }

    public IActionResult InfoServiceDemo()
    {
        InfoService svc = new InfoService();
        ViewData["Url"] = svc.GetUrl();
        return View("Index");
    }
}
