using Azunt.Web.Infrastructure.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Azunt.Web.Controllers.Api;

[ApiController]
[Route("api/demo/submit")]
public class SubmitDemoController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITenantPolicyService _tenantPolicy;

    public SubmitDemoController(UserManager<ApplicationUser> userManager, ITenantPolicyService tenantPolicy)
    {
        _userManager = userManager;
        _tenantPolicy = tenantPolicy;
    }

    private Task<bool> HasAllRequiredUploadsAsync(int docId) => Task.FromResult(false);

    [HttpPut("{docId}")]
    public async Task<IActionResult> SubmitAsync(int docId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Unauthorized();

        var userTenant = HttpContext.Request.Headers["X-Demo-UserTenant"].FirstOrDefault() ?? "Hawaso";
        var bypass = _tenantPolicy.IsBypassUploadCheckTenant(userTenant);

        if (!bypass)
        {
            var ok = await HasAllRequiredUploadsAsync(docId);
            if (!ok) return UnprocessableEntity(new { message = "Required uploads are missing.", tenant = userTenant });
        }

        return NoContent();
    }
}
