using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers
{
    public class GuideController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
