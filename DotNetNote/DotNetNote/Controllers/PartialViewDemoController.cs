namespace DotNetNote.Controllers;

public class PartialViewDemoController : Controller
{
    public IActionResult Index(int page = 1)
    {
        var pageModel = new Dul.Web.PagerBase
        {
            Url = "PartialViewDemo",
            RecordCount = 140,
            PageSize = 10,
            PageNumber = page,

            SearchMode = true,
            SearchField = "Title",
            SearchQuery = "ASP.NET"
        };

        ViewBag.PageModel = pageModel;

        return View();
    }
}
