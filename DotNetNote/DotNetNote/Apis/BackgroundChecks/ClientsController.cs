using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Apis;

public record ClientItem(
    string clientGuid,
    string name,
    string code,
    string status,
    string contactName,
    string contactEmail,
    long createdDate,
    string createdBy,
    long modifiedDate,
    string modifiedBy
);

[ApiController]
[Route("v1/[controller]")]
public class ClientsController : ControllerBase
{
    private const string FixedJwtToken = "TestToken";

    // 더미 데이터 (가공된 값들)
    private static readonly List<ClientItem> Clients = new()
    {
        new ClientItem(
            clientGuid: "11111111-1111-1111-1111-111111111111",
            name: "Sandbox API account for VisualAcademy",
            code: "ACCT-0001",
            status: "ACTIVE",
            contactName: "Alice",
            contactEmail: "alice@example.com",
            createdDate: 1751000000000,
            createdBy: "System Admin",
            modifiedDate: 1751100000000,
            modifiedBy: "System Admin"
        ),
        new ClientItem(
            clientGuid: "22222222-2222-2222-2222-222222222222",
            name: "Sandbox API account for Hawaso",
            code: "ACCT-0002",
            status: "ACTIVE",
            contactName: "David",
            contactEmail: "david@example.com",
            createdDate: 1751200000000,
            createdBy: "System Admin",
            modifiedDate: 1751300000000,
            modifiedBy: "System Admin"
        )
    };

    [HttpGet]
    public IActionResult GetClients(
        [FromHeader(Name = "Authorization")] string? authorization,
        [FromQuery] int? page,
        [FromQuery] int? size)
    {
        if (authorization is null || !authorization.StartsWith("Bearer "))
            return Unauthorized(new { message = "Authorization header missing" });

        var token = authorization["Bearer ".Length..];
        if (token != FixedJwtToken)
            return Unauthorized(new { message = "Invalid token" });

        // 전체 목록
        if (page is null || size is null)
        {
            return Ok(Clients);
        }

        // 간단 페이징
        var skip = Math.Max(0, page.Value) * Math.Max(1, size.Value);
        var take = Math.Max(1, size.Value);
        var paged = Clients.Skip(skip).Take(take).ToList();

        return Ok(paged);
    }
}
