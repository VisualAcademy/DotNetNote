using Microsoft.AspNetCore.Mvc;

namespace DotNetSale.Controllers
{
    [Area("DotNetSale")]
    public class CategoryController : Controller
    {
        public IActionResult Index() =>
            //return RedirectToAction("CategoryList");
            RedirectToAction(nameof(CategoryList));

        public IActionResult CategoryAdd() => View();

        public IActionResult CategoryList()
        {
            return View(); 
        }
    }
}
