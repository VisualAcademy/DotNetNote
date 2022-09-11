using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

/// <summary>
/// [실습] Razor 표현식 사용하기 
/// </summary>
public class RazorDemoController : Controller
{
    public IActionResult Index() => View();

    public IActionResult Demo1() => View();

    public IActionResult Demo2() => View();
}
