using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace DotNetNote.Controllers
{
    public class LoggingDemoController : Controller
    {
        private readonly ILogger<LoggingDemoController> _logger;

        public LoggingDemoController(ILogger<LoggingDemoController> logger)
        {
            _logger = logger; // 생성자 주입 방식으로 ILogger 개체 사용
        }

        public IActionResult Index()
        {
            // Index 페이지 실행시 로그의 Info 범주에 문자열과 시간 출력
            _logger.LogInformation("Index View {time}", DateTime.Now);
            return View();
        }

        public IActionResult About()
        {
            // About 페이지 실행시 로그의 Info 범주에 문자열과 시간 출력
            _logger.LogInformation("About View {time}", DateTime.Now);

            return View();
        }

    }
}
