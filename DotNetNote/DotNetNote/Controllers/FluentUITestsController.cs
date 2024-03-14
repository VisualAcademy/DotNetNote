using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers
{
    public class FluentUITestsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
