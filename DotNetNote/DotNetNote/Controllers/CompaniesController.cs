using DotNetNote.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers
{
    public class CompaniesController : Controller
    {
        private ICompanyRepository _repository;

        public CompaniesController(ICompanyRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string name)
        {
            ViewBag.Message = $"{name}을 입력했습니다.";

            return View();
        }

        [HttpGet]
        public IActionResult Manage()
        {
            var companies = _repository.Read();

            return View(companies);
        }

        [HttpPost]
        public IActionResult Manage(string name)
        {
            _repository.Add(new CompanyModel() { Name = name });
            return RedirectToAction(nameof(Manage));
        }
    }
}
