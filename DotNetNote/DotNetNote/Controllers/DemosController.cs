using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers
{
    public class DemosController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult FillDropDownListWithAjax()
        {
            return View(); 
        }
    }
}
