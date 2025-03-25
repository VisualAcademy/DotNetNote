using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers
{
    public class GoodiesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
