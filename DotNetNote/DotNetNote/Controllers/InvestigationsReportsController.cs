using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers
{
    public class InvestigationsReportsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
