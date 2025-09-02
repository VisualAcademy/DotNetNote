using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Apis;

[ApiController]
[Route("v1/[controller]")]
public class InfoController : ControllerBase
{
    private const string FixedJwtToken = "TestToken";

    [HttpGet]
    public IActionResult GetInfo([FromHeader(Name = "Authorization")] string? authorization)
    {
        if (authorization is null || !authorization.StartsWith("Bearer "))
            return Unauthorized(new { message = "Authorization header missing" });

        var token = authorization["Bearer ".Length..];
        if (token != FixedJwtToken)
            return Unauthorized(new { message = "Invalid token" });

        var info = new
        {
            instanceGuid = "417a0ebd-878e-4cc2-948a-510fbdfb077a",
            instanceName = "Auto Setup Instance - Sandbox API Demo Instance",
            craName = "Sample Background Screening, Inc.",
            application = new
            {
                applicationName = "Sandbox Demo Application",
                apiTypes = new[] { "Order" },
                audience = "CRA"
            }
        };

        return Ok(info);
    }
}
