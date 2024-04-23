namespace DotNetNote.Controllers;

public class TestsController : Controller
{
    // GET: /<controller>/
    public IActionResult Index() => View();

    public IActionResult DulGetNameTest() => View();
}
