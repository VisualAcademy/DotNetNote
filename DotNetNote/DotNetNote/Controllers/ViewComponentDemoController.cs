using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

public class ViewComponentDemoController : Controller
{
    public IActionResult Index() => View();

    /// <summary>
    /// CopyrightViewComponent 출력 데모
    /// </summary>
    public IActionResult CopyrightDemo() => View();

    /// <summary>
    /// TechListViewComponent 사용 데모
    /// </summary>
    public IActionResult TechListDemo() => View();


    /// <summary>
    /// SiteListViewComponent 사용 데모
    /// </summary>
    public IActionResult SiteListDemo()
    {
        return View();
    }


    /// <summary>
    /// 최근 댓글 리스트 출력 데모
    /// </summary>
    public IActionResult RecentlyCommentListDemo()
    {
        return View();
    }
}
