using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Areas.Employees.Controllers
{
    public class TemplatesController : Controller
    {
        public IActionResult Index()
        {
            return Content("Employee Area");
        }
    }
}
