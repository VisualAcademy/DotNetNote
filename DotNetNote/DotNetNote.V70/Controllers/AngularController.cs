using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

public class AngularController : Controller
{
    public IActionResult Index() => View();

    public IActionResult Error() => View();
}
