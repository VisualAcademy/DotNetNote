using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers
{
    public class SessionsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
