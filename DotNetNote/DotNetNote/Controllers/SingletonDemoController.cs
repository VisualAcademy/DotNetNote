using DotNetNote.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

public class SingletonDemoController : Controller
{
    //[1] 생성자에 클래스 주입
    //private readonly InfoService _svc;

    //public SingletonDemoController(InfoService svc)
    //{
    //    _svc = svc;
    //}

    //[2] 생성자에 인터페이스 주입
    private readonly IInfoService _svc;

    public SingletonDemoController(IInfoService svc)
    {
        _svc = svc;
    }

    public IActionResult ConstructorInjectionDemo()
    {
        ViewData["Url"] = _svc.GetUrl();
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
