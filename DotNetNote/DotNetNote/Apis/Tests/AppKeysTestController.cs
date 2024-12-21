//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace DotNetNote.Apis.Tests
//{
//    [Authorize(Roles = "Administrators")]
//    [ApiController]
//    [Route("api/[controller]")]
//    public class AppKeysTestController : ControllerBase
//    {
//        private readonly IConfiguration _configuration;

//        public AppKeysTestController(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }

//        [HttpGet("GetAppKeys")]
//        public IActionResult GetAppKeys()
//        {
//            var azureStorageAccount = _configuration["AppKeys:AzureStorageAccount"];
//            var azureStorageAccessKey = _configuration["AppKeys:AzureStorageAccessKey"];

//            if (string.IsNullOrEmpty(azureStorageAccount) || string.IsNullOrEmpty(azureStorageAccessKey))
//            {
//                return NotFound("AppKeys 데이터가 설정되지 않았습니다.");
//            }

//            return Ok(new
//            {
//                AzureStorageAccount = azureStorageAccount,
//                AzureStorageAccessKey = azureStorageAccessKey
//            });
//        }
//    }
//}
