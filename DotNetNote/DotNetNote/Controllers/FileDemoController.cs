using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace DotNetNote.Controllers;

public class FileDemoController(IWebHostEnvironment environment) : Controller
{
    /// <summary>
    /// 파일 업로드 폼 
    /// </summary>
    [HttpGet]
    public IActionResult FileUploadDemo() => View();

    /// <summary>
    /// 파일 업로드 처리
    /// 보안상 파일 업로드 처리 코드는 주석 처리합니다.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> FileUploadDemo(
        ICollection<IFormFile> files)
    {
        var uploadFolder = Path.Combine(environment.WebRootPath, "files");

        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                var fileName = Path.GetFileName(
                    ContentDispositionHeaderValue.Parse(
                        file.ContentDisposition).FileName.Trim('"'));

                using (var fileStream = new FileStream(
                    Path.Combine(uploadFolder, fileName), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
        }

        return View();
    }

    /// <summary>
    /// 파일 다운로드 처리 데모
    /// </summary>
    public FileResult FileDownloadDemo(string fileName = "Test.txt")
    {
        byte[] fileBytes = System.IO.File.ReadAllBytes(
            Path.Combine(
                environment.WebRootPath, "files") + "\\" + fileName);

        return File(fileBytes, "application/octet-stream", fileName);
    }

    /// <summary>
    /// 파일 업로드 폼 
    /// </summary>
    [HttpGet]
    public IActionResult FileUploadFileDemo() => View();

    ///// <summary>
    ///// 파일 업로드 처리
    ///// 보안상 파일 업로드 처리 코드는 주석 처리합니다.
    ///// </summary>
    //[HttpPost]
    //public async Task<IActionResult> FileUploadFileDemo(ICollection<IFormFile> files)
    //{
    //    var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\files");

    //    foreach (var file in files)
    //    {
    //        if (file.Length > 0)
    //        {
    //            var fileName = Path.GetFileName(ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"'));

    //            using (var fileStream = new FileStream(Path.Combine(uploadFolder, fileName), FileMode.Create))
    //            {
    //                await file.CopyToAsync(fileStream);
    //            }
    //        }
    //    }

    //    return View();
    //}
}
