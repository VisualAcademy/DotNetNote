using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers
{
    public class SamplesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AnimateCss()
        {
            return View(); 
        }

        /// <summary>
        /// PersonServiceController 테스트 
        /// </summary>
        public IActionResult PersonSerivceTest()
        {
            return View();
        }

        /// <summary>
        /// jQuery UI 테스트
        /// </summary>
        public IActionResult jQueryUITest()
        {
            return View(); 
        }


        public IActionResult BootboxDemo()
        {
            return View();
        }

        public IActionResult BootboxProcess()
        {
            return View(); 
        }

    }
}
