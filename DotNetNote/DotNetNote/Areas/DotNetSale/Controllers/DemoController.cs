using Microsoft.AspNetCore.Mvc;
using CategoryRepositoryInMemory = DotNetSale.Models.CategoryRepositoryInMemory;

namespace DotNetSale.Controllers
{
    [Area("DotNetSale")]
    public class DemoController : Controller
    {
        public IActionResult Index() => View();

        public IActionResult Category()
        {
            CategoryRepositoryInMemory repository = new CategoryRepositoryInMemory();

            var categories = repository.GetAll();

            return View(categories);
            //return View();
        }
    }
}
