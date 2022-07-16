using DotNetNote.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers
{
    /// <summary>
    /// 종속성 주입(의존성 주입) 연습 
    /// </summary>
    public class DependencyInjectionDemoController : Controller
    {
        //private CopyrightService _service;
        //public DependencyInjectionDemoController(CopyrightService service)
        //{
        //    _service = service;
        //}
        private ICopyrightService _service;
        private ICopyrightService _service2;

        public DependencyInjectionDemoController(
            ICopyrightService service, ICopyrightService service2)
        {
            _service = service;
            _service2 = service2;
        }

        public IActionResult Index()
        {
            //ViewBag.Copyright =
            //    $"Copyright {DateTime.Now.Year} all right reserved.";

            //CopyrightService _svc = new CopyrightService();
            //ViewBag.Copyright = _svc.GetCopyrightString();

            //ViewBag.Copyright = _service.GetCopyrightString();

            ViewBag.Copyright = 
                _service.GetCopyrightString() + ", " + 
                _service2.GetCopyrightString();
            return View();
        }

        public IActionResult About()
        {
            //CopyrightService _svc = new CopyrightService();
            //ViewBag.Copyright = _svc.GetCopyrightString();
            ViewBag.Copyright = _service.GetCopyrightString();
            return View();
        }

        public IActionResult AtInjectDemo()
        {
            return View(); 
        }
    }
}
