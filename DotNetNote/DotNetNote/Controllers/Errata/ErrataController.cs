using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

public class ErrataController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
