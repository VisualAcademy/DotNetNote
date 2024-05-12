using DotNetNote.Models.Companies;

namespace DotNetNote.Controllers;

public class CompaniesController(ICompanyRepository repository) : Controller
{
    public IActionResult Index() => View();

    [HttpPost]
    public IActionResult Index(string name)
    {
        ViewBag.Message = $"{name}을 입력했습니다.";

        return View();
    }

    [HttpGet]
    public IActionResult Manage()
    {
        var companies = repository.Read();

        return View(companies);
    }

    [HttpPost]
    public IActionResult Manage(string name)
    {
        repository.Add(new CompanyModel() { Name = name });
        return RedirectToAction(nameof(Manage));
    }
}
