using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Web.Controllers
{
    public class ExamDemoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
