using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

public sealed class ScreeningDemoController : Controller
{
    public IActionResult Index(string? tenant, string? partner, bool? admin, bool? global)
    {
        var vm = new ScreeningDemoVm
        {
            TenantName = string.IsNullOrWhiteSpace(tenant) ? "VisualAcademy" : tenant,
            PartnerName = string.IsNullOrWhiteSpace(partner) ? "Azunt" : partner,
            IsAdmin = admin ?? false,
            IsGlobalAdmin = global ?? false
        };

        return View(vm);
    }
}
