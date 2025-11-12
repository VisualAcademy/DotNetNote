using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

public class ApplicationFormDataViewerController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
