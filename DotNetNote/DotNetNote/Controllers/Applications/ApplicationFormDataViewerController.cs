using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers.Applications;

public class ApplicationFormDataViewerController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
