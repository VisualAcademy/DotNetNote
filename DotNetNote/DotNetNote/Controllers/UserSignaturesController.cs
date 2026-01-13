using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers
{
    public class UserSignaturesController : Controller
    {
        public IActionResult Index()
        {
            return Content("UsersSignature Controller");
        }
    }
}
