using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers
{
    public class PublicAttachmentUploadsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
