using Azunt.Web.Infrastructure.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Azunt.Web.Controllers;

public class DocumentsDemoController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITenantPolicyService _tenantPolicy;

    public DocumentsDemoController(UserManager<ApplicationUser> userManager, ITenantPolicyService tenantPolicy)
    {
        _userManager = userManager;
        _tenantPolicy = tenantPolicy;
    }

    private record DemoDoc(int Id, string OwnerEmail, string TenantName, string State, string? Json);
    private static DemoDoc GetFakeDoc(int id) =>
        new(id, "owner@visualacademy.com", "VisualAcademy", "Todo", null);

    public async Task<IActionResult> Index(int id = 1)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return LocalRedirect("/Identity/Account/Login");

        var doc = GetFakeDoc(id);
        var email = (await _userManager.GetEmailAsync(user)) ?? "";
        var isOwner = string.Equals(doc.OwnerEmail, email, StringComparison.OrdinalIgnoreCase);
        ViewBag.CanEdit = isOwner;
        ViewBag.State = doc.State;

        var isAdmin = await _userManager.IsInRoleAsync(user, "Administrators");

        // 학습용: 유저 테넌트는 헤더로 흉내
        var userTenant = HttpContext.Request.Headers["X-Demo-UserTenant"].FirstOrDefault() ?? "DotNetNote";
        var knownTenants = new[] { doc.TenantName, userTenant };

        var (isManager, managerTenant) = await _tenantPolicy.IsTenantManagerAsync(
            user, role => _userManager.IsInRoleAsync(user, role), knownTenants);

        var access =
            isOwner ||
            isAdmin ||
            (isManager && string.Equals(doc.TenantName, managerTenant, StringComparison.OrdinalIgnoreCase));

        if (!access) return LocalRedirect("/");

        // 운영 편의: 특정 테넌트면 강제 편집 허용
        var candidateTenant = managerTenant ?? doc.TenantName ?? userTenant;
        if (_tenantPolicy.IsEditOverrideTenant(candidateTenant))
            ViewBag.CanEdit = true;

        ViewBag.DocTenant = doc.TenantName;
        ViewBag.UserTenant = userTenant;
        return View(); // Views/DocumentsDemo/Index.cshtml (간단한 값 출력)
    }
}
