﻿namespace DotNetNote.Controllers;

public class SamplesController : Controller
{
    public IActionResult Index() => View();

    public IActionResult AnimateCss() => View();

    /// <summary>
    /// PersonServiceController 테스트 
    /// </summary>
    public IActionResult PersonSerivceTest() => View();

    /// <summary>
    /// jQuery UI 테스트
    /// </summary>
    public IActionResult jQueryUITest() => View();

    public IActionResult BootboxDemo() => View();

    public IActionResult BootboxProcess() => View();
}
