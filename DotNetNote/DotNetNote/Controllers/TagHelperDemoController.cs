using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

public class TagHelperDemoController : Controller
{
    public IActionResult Index() => View();

    /// <summary>
    /// environment 태그 헬퍼 사용하기
    /// </summary>
    public IActionResult EnvironmentDemo() => View();

    /// <summary>
    /// 내장 태그 헬퍼에 접두사 붙이기
    /// </summary>
    public IActionResult PrefixDemo()
    {
        return View();
    }

    /// <summary>
    /// 사용자 정의 태그 헬퍼 테스트
    /// </summary>
    public IActionResult MyTagHelperDemo()
    {
        return View();
    }

    /// <summary>
    /// 커스텀 태그 헬퍼
    /// </summary>
    public IActionResult EmailLinkDemo()
    {
        return View();
    }

    /// <summary>
    /// 유닉스 시간 변경기 태그 헬퍼 사용 테스트
    /// </summary>
    public IActionResult TagHelperDemo()
    {
        return View();
    }

    /// <summary>
    /// 페이징 헬퍼
    /// </summary>
    public IActionResult PagingHelperDemo()
    {
        return View();
    }

    /// <summary>
    /// Cache 태그 헬퍼 
    /// </summary>
    public IActionResult CacheDemo()
    {
        return View();
    }


    /// <summary>
    /// 마크다운 뷰어
    /// </summary>
    public IActionResult MarkdownViewerDemo()
    {
        return View();
    }
}
