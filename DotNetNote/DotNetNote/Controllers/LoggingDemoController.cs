namespace DotNetNote.Controllers;

public class LoggingDemoController(ILogger<LoggingDemoController> logger) : Controller
{
    public IActionResult Index()
    {
        // Index 페이지 실행시 로그의 Info 범주에 문자열과 시간 출력
        logger.LogInformation("Index View {time}", DateTime.Now);
        return View();
    }

    public IActionResult About()
    {
        // About 페이지 실행시 로그의 Info 범주에 문자열과 시간 출력
        logger.LogInformation("About View {time}", DateTime.Now);

        return View();
    }

}
