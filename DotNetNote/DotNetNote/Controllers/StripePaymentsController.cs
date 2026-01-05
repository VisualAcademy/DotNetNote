using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers
{
    public class StripePaymentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
