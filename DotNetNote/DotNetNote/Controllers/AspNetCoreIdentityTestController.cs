using System.Security.Claims;
using DotNetNote.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

public class AspNetCoreIdentityTestController(UserManager<DotNetNoteUser> userManager) : Controller
{
    // GET: /<controller>/
    public IActionResult Index() => View();

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(DotNetNoteUserRegisterModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (string.IsNullOrWhiteSpace(model.UserName))
        {
            ModelState.AddModelError(nameof(model.UserName), "사용자 이름을 입력하세요.");
            return View(model);
        }

        if (string.IsNullOrWhiteSpace(model.Password))
        {
            ModelState.AddModelError(nameof(model.Password), "암호를 입력하세요.");
            return View(model);
        }

        var userName = model.UserName;
        var password = model.Password;

        var existingUser = await userManager.FindByNameAsync(userName);

        if (existingUser != null)
        {
            ModelState.AddModelError(nameof(model.UserName), "이미 사용 중인 사용자 이름입니다.");
            return View(model);
        }

        var user = new DotNetNoteUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = userName
        };

        var result = await userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            return View("Success");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(DotNetNoteLoginModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (string.IsNullOrWhiteSpace(model.UserName))
        {
            ModelState.AddModelError(nameof(model.UserName), "사용자 이름을 입력하세요.");
            return View(model);
        }

        if (string.IsNullOrWhiteSpace(model.Password))
        {
            ModelState.AddModelError(nameof(model.Password), "암호를 입력하세요.");
            return View(model);
        }

        var userName = model.UserName;
        var password = model.Password;

        var user = await userManager.FindByNameAsync(userName);

        if (user != null && await userManager.CheckPasswordAsync(user, password))
        {
            var identity = new ClaimsIdentity("Cookies");

            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));

            if (!string.IsNullOrWhiteSpace(user.UserName))
            {
                identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            }

            await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(identity));

            return RedirectToAction(nameof(Greetings));
        }

        ModelState.AddModelError(string.Empty, "아이디 또는 암호가 틀립니다.");

        return View(model);
    }

    [HttpGet]
    public IActionResult Greetings() => View();
}