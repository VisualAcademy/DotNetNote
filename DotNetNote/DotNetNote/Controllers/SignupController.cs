using Microsoft.AspNetCore.Mvc;

namespace VisualAcademy.Controllers
{
    public class SignupController : Controller
    {
        [HttpGet("/signup")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("/signup")]
        public IActionResult Index(string username, string email, string password)
        {
            // TODO: 회원가입 처리
            // 예: DB 저장, Identity User 생성 등

            return Content($"가입 완료: {username}, {email}");
        }
    }
}
