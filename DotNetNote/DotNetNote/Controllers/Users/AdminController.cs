namespace DotNetNote.Controllers;

//[4] Administrators 이름으로 관리자 권한(Policy) 설정 관련
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
    public IActionResult NoteManager() => View();

    /// <summary>
    /// TODO: 사용자 관리자 페이지
    /// </summary>
    public IActionResult UserManager() => View();
}
