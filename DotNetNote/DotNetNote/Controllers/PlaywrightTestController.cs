using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace DotNetNote.Controllers
{
    /// <summary>
    /// Playwright.NET을 사용하여 Chromium에서 HTML을 렌더링하고
    /// 특정 HTML 요소만 PNG 이미지로 캡처하는 데모 컨트롤러입니다.
    /// </summary>
    public class PlaywrightTestController : Controller
    {
        private const string CaptureTargetSelector = "#capture-target";

        private readonly ILogger<PlaywrightTestController> logger;

        public PlaywrightTestController(
            ILogger<PlaywrightTestController> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Playwright Chromium을 실행하여 특정 HTML 영역만
        /// PNG 이미지로 캡처합니다.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                using var playwright = await Playwright.CreateAsync();

                await using var browser = await playwright.Chromium.LaunchAsync(
                    new BrowserTypeLaunchOptions
                    {
                        Headless = true
                    });

                var page = await browser.NewPageAsync(
                    new BrowserNewPageOptions
                    {
                        ViewportSize = new ViewportSize
                        {
                            Width = 1200,
                            Height = 800
                        }
                    });

                await page.SetContentAsync(
                    TestHtml,
                    new PageSetContentOptions
                    {
                        WaitUntil = WaitUntilState.Load
                    });

                var target = page.Locator(CaptureTargetSelector);

                var imageBytes = await target.ScreenshotAsync(
                    new LocatorScreenshotOptions
                    {
                        Type = ScreenshotType.Png
                    });

                ViewBag.Success = true;
                ViewBag.ImageBase64 = Convert.ToBase64String(imageBytes);
                ViewBag.ImageSize = imageBytes.Length;
                ViewBag.ErrorMessage = null;
            }
            catch (Exception ex)
            {
                this.logger.LogError(
                    ex,
                    "Playwright.NET screenshot test failed.");

                ViewBag.Success = false;
                ViewBag.ImageBase64 = null;
                ViewBag.ImageSize = 0;
                ViewBag.ErrorMessage = ex.Message;
            }

            return this.View();
        }

        /*
        /// <summary>
        /// Playwright에서 사용할 Chromium 브라우저를 설치합니다.
        ///
        /// 학습 및 배포 환경 확인을 위한 참고용 코드입니다.
        /// 실제 공개 사이트에서는 이 액션을 활성화하지 않는 것을 권장합니다.
        ///
        /// Azure App Service 등의 서버에 Chromium이 설치되어 있지 않을 때
        /// 일회성 테스트 용도로 사용할 수 있습니다.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult InstallChromium()
        {
            try
            {
                var exitCode = Microsoft.Playwright.Program.Main(
                    new[] { "install", "chromium" });

                if (exitCode != 0)
                {
                    throw new Exception(
                        $"Playwright Chromium installation failed. ExitCode: {exitCode}");
                }

                TempData["PlaywrightInstallMessage"] =
                    "Chromium installed successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                this.logger.LogError(
                    ex,
                    "Playwright Chromium installation failed.");

                TempData["PlaywrightInstallError"] = ex.Message;

                return RedirectToAction(nameof(Index));
            }
        }
        */

        /// <summary>
        /// Playwright 렌더링 및 특정 요소 캡처 테스트에 사용할 HTML입니다.
        /// </summary>
        private const string TestHtml = """
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset="utf-8" />

                <style>
                    body {
                        font-family: Arial, sans-serif;
                        background: #eeeeee;
                        padding: 40px;
                    }

                    #capture-target {
                        width: 700px;
                        background: white;
                        border: 2px solid #444444;
                        padding: 30px;
                    }

                    .title {
                        font-size: 22px;
                        font-weight: bold;
                        text-align: center;
                        margin-bottom: 25px;
                    }

                    .signature {
                        margin-top: 30px;
                        width: 250px;
                        height: 70px;
                        border: 1px solid #888888;
                        display: flex;
                        align-items: center;
                        justify-content: center;
                        font-style: italic;
                        font-size: 24px;
                    }
                </style>
            </head>

            <body>
                <div>
                    이 부분은 캡처 대상이 아닙니다.
                </div>

                <div id="capture-target">
                    <div class="title">
                        RELEASE OF INFORMATION AUTHORIZATION
                    </div>

                    <p>
                        This is a simple Playwright.NET screenshot test.
                    </p>

                    <p>
                        Only this HTML section should appear in the generated image.
                    </p>

                    <div class="signature">
                        Test Signature
                    </div>

                    <p>
                        Date: 2026-08-18
                    </p>
                </div>

                <div>
                    이 부분 역시 캡처 대상이 아닙니다.
                </div>
            </body>
            </html>
            """;
    }
}