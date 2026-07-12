using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.ViewComponents.TenantHelpfulGuides;

public class TenantHelpfulGuidesViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}