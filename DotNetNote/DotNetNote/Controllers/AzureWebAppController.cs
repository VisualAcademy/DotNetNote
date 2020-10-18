using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers
{
    public class AzureWebAppController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
