using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers
{
    // MaximController 클래스는 Controller 클래스를 상속받습니다.
    public class MaximController : Controller
    {
        // MaximServiceRepository 타입의 _repo 필드를 선언하고 있습니다.
        // 이는 MaximController 클래스가 MaximServiceRepository를 사용하여 
        // Maxim 관련 데이터를 관리하기 위함입니다.
        private MaximServiceRepository _repo;

        // MaximController 생성자에서 MaximServiceRepository 개체를 받아 _repo 필드에 할당합니다.
        // 이로써, MaximController 개체가 생성될 때 MaximServiceRepository 개체가 함께 주입됩니다.
        public MaximController(MaximServiceRepository maximService) => _repo = maximService;

        // Index 메서드는 HTTP GET 요청에 응답하여 View를 반환합니다.
        // View에서는 _repo.GetMaxims()로 가져온 Maxim 리스트를 표시할 것입니다.
        public IActionResult Index() => View(_repo.GetMaxims());

        // Create 메서드는 HTTP GET 요청에 응답하여 'Create' View를 반환합니다.
        public IActionResult Create() => View();

        // 이 Create 메서드는 HTTP POST 요청에 응답합니다.
        // 사용자가 입력한 Maxim 데이터를 받아 Repository에 저장하고, 
        // 저장이 완료된 후 'Index' View로 리다이렉션합니다.
        [HttpPost]
        public IActionResult Create(Maxim model)
        {
            // Maxim 데이터를 MaximServiceRepository의 AddMaxim 메서드에 전달하여 저장합니다.
            _repo.AddMaxim(model);

            // 'Index' 액션 메서드로 리다이렉션합니다. 이는 사용자에게 Maxim 목록 페이지를 다시 보여주게 됩니다.
            return RedirectToAction("Index", "Maxim");
        }
    }
}
