//using Microsoft.AspNetCore.Mvc;

//namespace DotNetNote.Apis;

//public record OrderItem(
//    string orderGuid,
//    int fileNumber,
//    string orderStatus,
//    string orderType,
//    long orderedDate,
//    string generalReportReference,
//    string externalIdentifier,
//    long completedDate,
//    string applicantName,
//    string clientName,
//    string clientCode,
//    string productName,
//    string requestedBy,
//    bool searchFlagged,
//    long createdDate,
//    string createdBy,
//    long modifiedDate
//);

//[ApiController]
//[Route("v1/clients/{clientGuid}/[controller]")]
//public class OrdersController : ControllerBase
//{
//    private const string FixedJwtToken = "TestToken";

//    // ClientsController.cs 에서 사용한 첫 번째 GUID 값
//    private const string DemoClientGuid = "11111111-1111-1111-1111-111111111111";

//    private static readonly List<(string ClientGuid, OrderItem Item)> Orders = new()
//    {
//        (
//            DemoClientGuid,
//            new OrderItem(
//                orderGuid: "6ab88112-d98f-422e-9395-17b15a64ce8d",
//                fileNumber: 670771,
//                orderStatus: "complete",
//                orderType: "Employment",
//                orderedDate: 1753606096000,
//                generalReportReference: "HANK-BAD-REF-002",
//                externalIdentifier: "HANK-BAD-002",
//                completedDate: 1753606098000,
//                applicantName: "MESS, HANK",
//                clientName: "Sandbox API account for DemoClient",
//                clientCode: DemoClientGuid,
//                productName: "API Demo",
//                requestedBy: "Auto Setup Instance - DemoClient",
//                searchFlagged: false,
//                createdDate: 1753606096000,
//                createdBy: "Auto Setup Instance - DemoClient",
//                modifiedDate: 1753606098000
//            )
//        ),
//        (
//            DemoClientGuid,
//            new OrderItem(
//                orderGuid: "b84409e4-a9a8-4d0d-9d6f-2dc88bb00277",
//                fileNumber: 670770,
//                orderStatus: "complete",
//                orderType: "Employment",
//                orderedDate: 1753606056000,
//                generalReportReference: "JOE-CLEAR-REF-001",
//                externalIdentifier: "JOE-CLEAR-001",
//                completedDate: 1753606058000,
//                applicantName: "CLEAN, JOE",
//                clientName: "Sandbox API account for DemoClient",
//                clientCode: DemoClientGuid,
//                productName: "API Demo",
//                requestedBy: "Auto Setup Instance - DemoClient",
//                searchFlagged: false,
//                createdDate: 1753606056000,
//                createdBy: "Auto Setup Instance - DemoClient",
//                modifiedDate: 1753606058000
//            )
//        )
//    };

//    [HttpGet]
//    public IActionResult GetOrders(
//        [FromHeader(Name = "Authorization")] string? authorization,
//        [FromRoute] string clientGuid,
//        [FromQuery] int? page,
//        [FromQuery] int? size)
//    {
//        if (authorization is null || !authorization.StartsWith("Bearer "))
//            return Unauthorized(new { message = "Authorization header missing" });

//        var token = authorization["Bearer ".Length..];
//        if (token != FixedJwtToken)
//            return Unauthorized(new { message = "Invalid token" });

//        var items = Orders
//            .Where(x => string.Equals(x.ClientGuid, clientGuid, StringComparison.OrdinalIgnoreCase))
//            .Select(x => x.Item)
//            .ToList();

//        if (page is null || size is null)
//            return Ok(items);

//        var safePage = Math.Max(0, page.Value);
//        var safeSize = Math.Max(1, size.Value);

//        var paged = items.Skip(safePage * safeSize).Take(safeSize).ToList();

//        return Ok(paged);
//    }
//}
