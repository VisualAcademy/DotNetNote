using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers
{
    public class AnalyticsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
