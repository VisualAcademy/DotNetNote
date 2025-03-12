using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Areas.Conferences.Controllers
{
    public class TicketsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
