using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers
{
    public class DemoController : Controller
    {
        public IActionResult Index()
        {
            //return Content("Demo Page");
            return View();
        }
    }
}
