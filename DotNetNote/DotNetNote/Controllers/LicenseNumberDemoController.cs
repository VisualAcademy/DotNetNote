using DotNetNote.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers
{
    public class LicenseNumberDemoController : Controller
    {
        private readonly IEmployeeLicenseNumberService employeeService;
        private readonly IVendorEmployeeLicenseNumberService vendorEmployeeService;
        private readonly IVendorLicenseNumberService vendorService;

        public LicenseNumberDemoController(
            IEmployeeLicenseNumberService employeeService,
            IVendorEmployeeLicenseNumberService vendorEmployeeService,
            IVendorLicenseNumberService vendorService)
        {
            this.employeeService = employeeService;
            this.vendorEmployeeService = vendorEmployeeService;
            this.vendorService = vendorService;
        }

        public IActionResult Index()
        {
            ViewBag.EmployeeSuggestion = employeeService.GetRecentLicenseNumberSuggestions();
            ViewBag.VendorEmployeeSuggestion = vendorEmployeeService.GetRecentLicenseNumberSuggestions();
            ViewBag.VendorSuggestion = vendorService.GetRecentLicenseNumberSuggestions();

            return View();
        }
    }
}