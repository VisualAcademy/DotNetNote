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
        // ViewData를 사용하여 뷰로 데이터 전달
        ViewData["Url"] = "www.gilbut.co.kr";
        return View();
    }

    public IActionResult InfoServiceDemo()
    {
        InfoService svc = new();
        ViewData["Url"] = svc.GetUrl();
        return View("Index");
    }
}
