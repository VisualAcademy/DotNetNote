using DotNetNote.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers
{
    public class AttendeeController : Controller
    {
        private IAttendeeRepository _repository;

        // 인터페이스를 통한 생성자 주입 방식 사용: 
        //     Startup.cs에서 services.AddTransient로 등록됨
        public AttendeeController(IAttendeeRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            // 모든 데이터 출력
            var list = _repository.GetAll();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Attendee model)
        {
            // 서버 측 유효성 검사 진행
            if (string.IsNullOrEmpty(model.Name) 
                || string.IsNullOrEmpty(model.UserId))
            {
                ModelState.AddModelError("", "잘못된 데이터입니다.");
            }

            if (ModelState.IsValid)
            {
                // 모델 개체의 값이 정상이면 저장
                _repository.Add(model);

                // Index 페이지로 이동
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
