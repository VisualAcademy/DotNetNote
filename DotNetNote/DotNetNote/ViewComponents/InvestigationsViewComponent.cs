using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.ViewComponents;

/// <summary>
/// Investigations 내용을 출력하는 ViewComponent입니다.
/// </summary>
public class InvestigationsViewComponent : ViewComponent
{
    /// <summary>
    /// Investigations 컴포넌트를 실행합니다.
    /// </summary>
    public IViewComponentResult Invoke()
    {
        string message = "Investigations ViewComponent 데모입니다.";

        return View("Default", message);
    }
}