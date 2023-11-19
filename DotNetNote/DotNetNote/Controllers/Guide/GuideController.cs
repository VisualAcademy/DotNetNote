using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

/// <summary>
/// ASP.NET Core 책 학습 가이드 
/// </summary>
public class GuideController : Controller
{
    public IActionResult Index() => View();
}
