using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers.CommunityCamp;

public class CommunityCampController(
    ICommunityCampJoinMemberRepository repository) : Controller
{
    public IActionResult Index()
    {
        // 모든 데이터 출력
        var list = repository.GetAll();

        return View(list);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    public IActionResult Create(CommunityCampJoinMember model)
    {
        // 서버 측 유효성 검사 진행
        if (string.IsNullOrEmpty(model.Email))
        {
            ModelState.AddModelError("Email", "이메일을 입력하시오.");
        }

        if (ModelState.IsValid)
        {
            ViewBag.Result =
                $"커뮤니티: {model.CommunityName}, 이름: {model.Name}";

            // 저장
            repository.AddMember(model);
            TempData["Message"] = "데이터가 저장되었습니다.";

            return RedirectToAction("Index");
        }

        return View();
    }

    [HttpGet]
    public IActionResult Delete() => View();

    [HttpPost]
    public IActionResult Delete(CommunityCampJoinMember model)
    {
        if (ModelState.IsValid)
        {
            // 저장
            repository.DeleteMember(model);
            TempData["Message"] = "데이터가 삭제되었습니다.";

            return RedirectToAction("Index");
        }

        return View();
    }

    [Authorize("Administrators")] // 관리자 아이디로 접근했을 때 볼 수 있음
    public IActionResult ComCampAdmin()
    {
        // 관리자 페이지
        var list = repository.GetAll();

        return View(list);
    }
}
