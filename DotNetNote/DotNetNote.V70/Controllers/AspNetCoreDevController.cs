using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

public class AspNetCoreDevController : Controller
{
    public IActionResult Index() => View();
}
