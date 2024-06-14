namespace DotNetNote.Tests;

[Authorize(Roles = "Administrators")]
public class LoginController : Controller
{
    public LoginController()
    {
    }

    public string Name { get; set; }
    public string Password { get; set; }

    public IActionResult Login()
    {
        ViewData["Title"] = "Login Page";
        return View();
    }

    public IActionResult FormSubmit(string name, string password)
    {
        // TODO: Authenticate user.
        ViewData["Name"] = name;
        ViewData["Title"] = "Welcome";
        return View("Welcome");
    }
}
