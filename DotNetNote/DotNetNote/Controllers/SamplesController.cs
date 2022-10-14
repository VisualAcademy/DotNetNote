using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

public class SamplesController : Controller
{
    public IActionResult Index() => View();

    public IActionResult AnimateCss()
    {
        return View();
    }

    /// <summary>
    /// PersonServiceController 테스트 
    /// </summary>
    public IActionResult PersonSerivceTest()
    {
        return View();
    }

    /// <summary>
    /// jQuery UI 테스트
    /// </summary>
    public IActionResult jQueryUITest() => View();

    public IActionResult BootboxDemo() => View();

    public IActionResult BootboxProcess() => View();
}
