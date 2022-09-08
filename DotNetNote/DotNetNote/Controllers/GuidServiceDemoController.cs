using DotNetNote.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

public class GuidServiceDemoController : Controller
{
    //[!] 생성자
    public GuidServiceDemoController(
        GuidService guidService, IGuidService iguidService)
    {
        _gs = new GuidService();
        _guidService = guidService;
        _iguidService = iguidService;
    }

    //[1] 액션 메서드에서 직접 GuidService의 인스턴스 생성
    public IActionResult Index()
    {
        GuidService gs = new GuidService();
        ViewBag.GuidString = gs.GetGuid();

        return View();
    }

    //[2] 생성자에서 직접 GuidService의 인스턴스 생성
    private GuidService _gs;
    public IActionResult CtorInstance()
    {
        ViewBag.GuidString = _gs.GetGuid();
        return View();
    }

    //[3] 생성자에 매개변수 주입: Startup-AddTransient<GuidService>();
    private readonly GuidService _guidService;
    public IActionResult CtorParameter()
    {
        ViewBag.GuidString = _guidService.GetGuid();
        return View();
    }

    //[4] 인터페이스를 사용한 생성자 주입: 
    private readonly IGuidService _iguidService;
    public IActionResult CtorInterface()
    {
        ViewBag.GuidString = _iguidService.GetGuid();
        return View();
    }

    //[5] @inject 키워드로 뷰에 직접 주입
    public IActionResult AtInject() => View();

    //[6] 3가지 컨테이너 비교
    public IActionResult AtInjectCompare()
    {
        return View();
    }
}
