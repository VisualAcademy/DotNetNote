using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.ViewComponents.UserManagement
{
    /// <summary>
    /// 로그인 IP 제한 UI (강의/데모용: 모델 의존 없음)
    /// </summary>
    public class LoginIpRestrictionPanelViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            // 필요 시 ViewData/TempData 등을 통해 기본값 주입 가능
            return View(); // 모델 없이 반환
        }
    }
}
