//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.StaticFiles;
//using System.IO;
//using System.Threading.Tasks;

//namespace DotNetNote.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class WebApiFileUploadDownloadTestController : ControllerBase
//    {
//        private readonly string _tempFilesPath;

//        public WebApiFileUploadDownloadTestController()
//        {
//            _tempFilesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tempfiles");

//            // Ensure the directory exists
//            if (!Directory.Exists(_tempFilesPath))
//            {
//                Directory.CreateDirectory(_tempFilesPath);
//            }
//        }

//        // 파일 업로드
//        [HttpPost("upload")]
//        public async Task<IActionResult> UploadFile(IFormFile file)
//        {
//            if (file == null || file.Length == 0)
//                return BadRequest("No file uploaded");

//            var filePath = Path.Combine(_tempFilesPath, file.FileName);

//            using (var stream = new FileStream(filePath, FileMode.Create))
//            {
//                await file.CopyToAsync(stream);
//            }

//            return Ok(new { FileName = file.FileName, FilePath = filePath });
//        }

//        // 파일 다운로드
//        [HttpGet("download/{fileName}")]
//        public IActionResult DownloadFile(string fileName)
//        {
//            var filePath = Path.Combine(_tempFilesPath, fileName);

//            if (!System.IO.File.Exists(filePath))
//                return NotFound("File not found");

//            var provider = new FileExtensionContentTypeProvider();
//            if (!provider.TryGetContentType(filePath, out var contentType))
//            {
//                contentType = "application/octet-stream";
//            }

//            var fileBytes = System.IO.File.ReadAllBytes(filePath);
//            return File(fileBytes, contentType, fileName);
//        }
//    }
//}
