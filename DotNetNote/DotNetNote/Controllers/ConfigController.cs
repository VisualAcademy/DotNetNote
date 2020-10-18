using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DotNetNoteCom.Controllers
{
    public class ConfigController : Controller
    {
        private IConfiguration _config;

        public ConfigController(IConfiguration config)
        {
            _config = config;
        }

        public string Index()
        {
            // 1.X 버전은 GetSecion() 메서드 사용
            //string srv = _config.GetSection("Con").GetSection("Server").Value; 
            //string rdb = "데이터베이스"; // 동적으로 변경 가능
            //string uid= _config["Con:User ID"]; // 2.X 버전
            //string pwd= _config["Con:Password"];

            // 원하는 모양으로 가공해서 사용 가능
            //return $"{srv};{rdb};{uid};{pwd}";
            return "";
        }
    }
}
