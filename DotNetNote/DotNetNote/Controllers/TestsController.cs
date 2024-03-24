// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DotNetNote.Controllers;

public class TestsController : Controller
{
    // GET: /<controller>/
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult DulGetNameTest()
    {
        return View(); 
    }
}
