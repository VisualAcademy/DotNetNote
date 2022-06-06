using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

[Authorize("Administrators")]
public class AdminController : Controller
{
    //[Authorize(Roles = "Users")]
    //[Authorize("Users")] // Startup.cs AddPolicy() 설정 필요
    //[Authorize("Administrators")]
    //public string Index()
    //{
    //    return "Admin 사용자만 볼 수 있음";
    //}

    public IActionResult Index() => View();

    /// <summary>
    /// TODO: 게시판 관리자 페이지
    /// </summary>
    public IActionResult NoteManager()
    {
        return View();
    }

    /// <summary>
    /// TODO: 사용자 관리자 페이지
    /// </summary>
    public IActionResult UserManager()
    {
        return View();
    }
}
