namespace DotNetNote.Controllers;

public class StronglyTypedConfigurationController(IOptions<DotNetNoteSettings> options) : Controller
{
    // 강력한 형식의 클래스의 인스턴스 생성
    private readonly DotNetNoteSettings _dnnSettings = options.Value;

    public IActionResult Index()
    {
        // 뷰 페이지로 전송
        ViewData["SiteName"] = _dnnSettings.SiteName;
        ViewBag.SiteUrl = _dnnSettings.SiteUrl;

        return View();
    }
}
