namespace DotNetNote.Controllers;

public class ViewBagDemoController : Controller
{
    /// <summary>
    /// ViewBag 개체로 컨트롤러에서 뷰로 데이터 전달
    /// </summary>
    public IActionResult Index()
    {
        ViewBag.Name = "박용준";
        ViewBag.Age = 21;
        ViewBag.원하는모든단어 = "모든 값...";

        // ViewBag.Address와 ViewData["Address"]는 동일 표현 
        //ViewBag.Address = "세종시...";
        ViewData["Address"] = "세종시...";

        return View();
    }

    public IActionResult JavaScriptStringTest()
    {
        ViewBag.AlertMessage = "안녕하세요."; 
        return View(); 
    }
}
