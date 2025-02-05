using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers.Administrations.UserRoleManagement
{
    public class UserRoleManagementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
