// https://youtu.be/idQXLTgb9-k

namespace DotNetNote.Controllers;

public class PagesController : Controller
{
    public IActionResult Index() => View();

    public IActionResult Welcome() => View();

    public IActionResult NotFoundPage() => View();

    public IActionResult NotAssigned() => View();

    public IActionResult IntroDemo() => View();
}
