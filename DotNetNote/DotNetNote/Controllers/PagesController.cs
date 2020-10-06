using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers
{
    public class PagesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Welcome()
        {
            return View();             
        }

        public IActionResult NotFoundPage()
        {
            return View();             
        }

        public IActionResult NotAssigned() 
        {
            return View();             
        }

        public IActionResult IntroDemo()
        {
            return View();
        }
    }
}
