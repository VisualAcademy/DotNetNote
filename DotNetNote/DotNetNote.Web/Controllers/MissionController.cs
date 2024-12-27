using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Web.Controllers
{
    public class MissionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
