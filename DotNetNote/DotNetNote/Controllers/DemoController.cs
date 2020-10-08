using DotNetNote.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace DotNetNote.Controllers
{
    public class DemoController : Controller
    {
        public IActionResult Index()
        {
            //return Content("Demo Page");
            return View();
        }

        /// <summary>
        /// Content() 반환값
        /// </summary>
        /// <returns></returns>
        public IActionResult ContentResultDemo()
        {
            return Content("ContentResult 반환값");
        }

        public IActionResult ObjectResultDemo()
        {
            DemoModel dm = new DemoModel() { Id = 1, Name = "홍길동" };
            return new ObjectResult(dm);
        }

        public IActionResult TempDataDemoStart()
        {
            TempData["Message"] = "TempDataDemoStart에서 만들어진 문자열";

            return View("TempDataDemo");
        }

        public IActionResult TempDataDemo()
        {
            return View();
        }

        public IActionResult JsonResultDemo()
        {
            return Json(new { Foo = "Bar" });
        }

        public string SendMailDemo()
        {
            return "전송 완료";
        }

        public IActionResult EnvironmentAndFramework()
        {
            return View();
        }

        public IActionResult RedirectPermanentDemo()
        {
            return RedirectPermanent("/");
        }

        /// <summary>
        /// 컨트롤러에서 컬렉션 형태의 데이터를 뷰 페이지로 전송하기
        /// </summary>
        public IActionResult ViewWithListOfDemo()
        {
            List<DemoModel> models = new List<DemoModel>() {
                new DemoModel { Id = 1, Name = "홍길동" },
                new DemoModel { Id = 2, Name = "백두산" },
                new DemoModel { Id = 3, Name = "임꺽정" }
            };

            return View(models); // 다중 레코드, 컬렉션, List<T>
        }

        /// <summary>
        /// 컨트롤러에서 모델 개체에 데이터를 담아서 뷰로 전송하기
        /// </summary>
        public IActionResult ViewWithModelDemo()
        {
            DemoModel dm = new DemoModel();
            dm.Id = 1;
            dm.Name = "홍길동";
            return View(dm);
        }
    }
}
