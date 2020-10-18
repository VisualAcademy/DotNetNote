using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers
{
    public class ChartControlController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
