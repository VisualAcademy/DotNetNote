using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers
{
    public class TemplatesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
