using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

public class ViewWithModelDemoController : Controller
{
    /// <summary>
    /// 컨트롤러에서 모델 개체에 데이터를 담아서 뷰로 전송하기
    /// </summary>
    public IActionResult Index()
    {
        DemoModel dm = new DemoModel();
        dm.Id = 1;
        dm.Name = "홍길동";
        return View(dm);
    }
}
