using Microsoft.AspNetCore.Http;

namespace DotNetNote.Controllers;

public class StorageManagerTestController(IStorageManager storageManager) : Controller
{
    public IActionResult Index() => View();

    [HttpPost]
    public async Task<IActionResult> Index(List<IFormFile> files)
    {
        byte[] byteArray;
        foreach (var formFile in files)
        {
            if (formFile.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    await formFile.CopyToAsync(stream);
                    byteArray = stream.ToArray();
                    var folderPath = storageManager.GetFolderPath("Test", "1234", "Files");
                    var newFileName = await storageManager.UploadAsync(byteArray, formFile.FileName, folderPath, false);

                    if (!string.IsNullOrEmpty(newFileName))
                    {
                        // 데이터베이스에 파일 이름 저장 영역 
                    }
                }
            }
        }

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Download(string fileName)
    {
        var folderPath = storageManager.GetFolderPath("Test", "1234", "Files");
        var fileBytes = await storageManager.DownloadAsync(fileName, folderPath);

        if (fileBytes == null)
        {
            return NotFound();
        }

        return File(fileBytes, "application/octet-stream", fileName);
    }
}
