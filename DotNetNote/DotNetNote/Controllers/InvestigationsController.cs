using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

/// <summary>
/// Investigations 페이지를 제공하는 MVC 컨트롤러입니다.
/// </summary>
public class InvestigationsController : Controller
{
    /// <summary>
    /// /Investigations 경로를 처리합니다.
    /// </summary>
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}