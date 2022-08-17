using DotNetNoteCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DotNetNote.Controllers;

public class AspNetCoreIdentityTestController : Controller
{
    private readonly UserManager<DotNetNoteUser> userManager;

    public AspNetCoreIdentityTestController(UserManager<DotNetNoteUser> userManager)
    {
        this.userManager = userManager;
    }

    // GET: /<controller>/
    public IActionResult Index() => View();

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(DotNetNoteUserRegisterModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await userManager.FindByNameAsync(model.UserName);

            if (user == null)
            {
                user = new DotNetNoteUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = model.UserName
                };

                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return View("Success");
                }
            }

            return View();
        }

        return View();
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(DotNetNoteLoginModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await userManager.FindByNameAsync(model.UserName);

            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                var identity = new ClaimsIdentity("Cookies");
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
                identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));

                await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(identity));

                //return Redirect("/Home/Index");
                return RedirectToAction(nameof(Greetings));
            }

            ModelState.AddModelError("", "아이디 또는 암호가 틀립니다.");
        }

        return View();
    }

    [HttpGet]
    public IActionResult Greetings() => View();
}
