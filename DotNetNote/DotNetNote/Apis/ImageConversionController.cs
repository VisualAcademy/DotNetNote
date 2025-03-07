using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Apis
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageConversionController : ControllerBase
    {
        [HttpGet("{fileName}")]
        public IActionResult GetBase64Image(string fileName)
        {
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logos", fileName);
            if (!System.IO.File.Exists(imagePath))
            {
                return NotFound("이미지를 찾을 수 없습니다.");
            }

            byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);
            string base64String = Convert.ToBase64String(imageBytes);
            string dataUrl = $"data:image/png;base64,{base64String}";

            return Ok(dataUrl);
        }
    }
}
