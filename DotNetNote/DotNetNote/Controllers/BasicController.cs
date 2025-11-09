// https://youtu.be/T4ybB7fepJs
// Visual Studio 2026에서 Basic 이름으로 MVC 컨트롤러 만들고 실행하기
// (실시간) ASP.NET Core MVC 기초 컨트롤러 클래스, 액션 메서드, 뷰 페이지

namespace DotNetNote.Controllers;

/// <summary>
/// Basic 컨트롤러 클래스
/// </summary>
public class BasicController : Controller
{
    /// <summary>
    /// 액션 메서드
    /// </summary>
    public IActionResult Index() => View();

    public IActionResult Unit() => View();

    [HttpGet]
    public IActionResult Quiz(int id, int page)
    {
        // 액션 메서드의 값을 뷰 페이지에 전송
        ViewBag.Page = page;
        ViewData["Id"] = id;

        return View();
    }

    [HttpPost]
    public IActionResult Quiz(string answer)
    {
        ViewBag.Answer = answer;
        return View();
    }

    /// <summary>
    /// 강력한 형식(Strongly Typed)의 뷰 페이지
    /// </summary>
    public IActionResult Analysis()
    {
        int score = 100;
        return View(score);
    }

    public IActionResult Review() => View();
}
