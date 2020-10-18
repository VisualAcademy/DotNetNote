using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers
{
    public class AngularController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
