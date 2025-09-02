using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Apis;

[ApiController]
[Route("v1/[controller]")]
public class UsageController : ControllerBase
{
    private const string FixedJwtToken = "TestToken";

    [HttpGet]
    public IActionResult GetUsage(
        [FromHeader(Name = "Authorization")] string? authorization,
        [FromQuery] string? startDate,
        [FromQuery] string? endDate)
    {
        if (authorization is null || !authorization.StartsWith("Bearer "))
            return Unauthorized(new { message = "Authorization header missing" });

        var token = authorization["Bearer ".Length..];
        if (token != FixedJwtToken)
            return Unauthorized(new { message = "Invalid token" });

        // 샘플 usageData (몇 개만 가공)
        var usage = new
        {
            startDate = startDate ?? "2025-08-01",
            endDate = endDate ?? "2025-08-30",
            usageData = new List<int[]>
            {
                new[] { 0, 20000 },
                new[] { 1, 19995 },
                new[] { 2, 19990 },
                new[] { 3, 19985 },
                new[] { 17, 19983 }
            }
        };

        return Ok(usage);
    }
}
