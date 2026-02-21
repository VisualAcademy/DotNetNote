using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers
{
    public class EmployeePhotosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
