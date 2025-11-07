using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers
{
    public class ApplicationFormBuilderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
