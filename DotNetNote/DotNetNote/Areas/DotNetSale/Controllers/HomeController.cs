using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DotNetSale.Controllers
{
    [Area("DotNetSale")]
    public class HomeController : Controller
    {
        private ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("HOME - Index 페이지가 로드되었습니다.");

            return View();
        }

        public IActionResult About()
        {
            _logger.LogInformation("HOME - About 페이지가 로드되었습니다.");

            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            _logger.LogInformation("HOME - Contact 페이지가 로드되었습니다.");

            ViewData["Message"] = "Your contact page.";

            return View();
        }
    }
}
