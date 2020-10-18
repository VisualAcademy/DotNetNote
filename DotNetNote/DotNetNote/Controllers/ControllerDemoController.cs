using Microsoft.AspNetCore.Mvc;
using System;

namespace DotNetNote.Controllers
{
    // 컨트롤러: 클래스
    // /ControllerDemo/
    public class ControllerDemoController : Controller
    {
        // 액션: 메서드
        // /ControllerDemo/Index
        public void Index()
        {
            // 아무런 값도 출력하지 않음
        }

        public string StringAction()
        {
            return "String을 반환하는 액션 메서드";
        }

        public DateTime DateTimeAction()
        {
            return DateTime.Now;
        }

        public IActionResult DefaultAction()
        {
            return View(); // 컨트롤러/액션메서드
        }
    }
}
