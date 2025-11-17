using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace DotNetNote.Controllers
{
    [IgnoreAntiforgeryToken]
    public class LogoutRedirectController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LogoutRedirectController(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpGet("/LogoutRedirect")]
        public async Task<IActionResult> Index(string returnUrl = "/")
        {
            if (_signInManager.IsSignedIn(User))
            {
                await _signInManager.SignOutAsync();
            }

            // DotNetNote Identity의 로그인 URL 구조 그대로 사용
            var loginUrl = $"/Identity/Account/Login?returnUrl={Uri.EscapeDataString(returnUrl)}";

            return Redirect(loginUrl);
        }
    }
}
