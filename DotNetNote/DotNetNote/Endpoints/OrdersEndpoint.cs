using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace DotNetNote.Endpoints;

public static class OrdersEndpoint
{
    private const string FixedJwtToken = DemoStore.FixedJwtToken;

    // 요청 바디 모델
    private record CreateOrderRequest(
        string applicantGuid,
        string clientProductGuid,
        bool useQuickApp,
        bool certifyPermissiblePurpose,
        string generalReportReference,
        string externalIdentifier,
        string orderNotes,
        bool quickappNotifyApplicants,
        bool queueConsumerDisclosure
    );

    // 목록/생성 응답(요약)
    private record OrderSummaryResponse(
        string orderGuid,
        int fileNumber,
        string orderStatus,
        string orderType,
        long orderedDate,
        string generalReportReference,
        string externalIdentifier,
        string applicantName,
        string clientName,
        string clientCode,
        string productName,
        string requestedBy,
        bool searchFlagged,
        long createdDate,
        string createdBy,
        long modifiedDate,
        string modifiedBy,
        long? completedDate // 완료 전엔 null
    );

    // 단건 상세 응답
    private record OrderDetailResponse(
        string orderGuid,
        int fileNumber,
        string orderStatus,
        string orderType,
        long orderedDate,
        string generalReportReference,
        string externalIdentifier,
        long? completedDate,
        string applicantName,
        string clientName,
        string clientCode,
        string productName,
        string requestedBy,
        bool searchFlagged,
        long createdDate,
        string createdBy,
        long modifiedDate,
        string modifiedBy,
        string clientProductGuid,
        string applicantEmail,
        string applicantDateOfBirth,
        string applicantPhone,
        string applicantGuid,
        double totalCharges,
        string orderNotes,
        List<string> includedSearches,
        List<object> customFields
    );

    public static void MapOrdersEndpoint(this IEndpointRouteBuilder endpoints)
    {
        // POST /v1/clients/{clientGuid}/orders
        endpoints.MapPost("/v1/clients/{clientGuid}/orders", async (
            HttpRequest request,
            string clientGuid
        ) =>
        {
            if (!IsAuthorized(request)) return Results.Unauthorized();

            var payload = await request.ReadFromJsonAsync<CreateOrderRequest>();
            if (payload is null)
                return Results.BadRequest(new { message = "Invalid JSON body." });

            // 간단 필수값 검증
            if (string.IsNullOrWhiteSpace(payload.applicantGuid) ||
                string.IsNullOrWhiteSpace(payload.clientProductGuid) ||
                string.IsNullOrWhiteSpace(payload.generalReportReference) ||
                string.IsNullOrWhiteSpace(payload.externalIdentifier))
            {
                return Results.BadRequest(new { message = "Missing required fields." });
            }

            // 지원자 확인
            if (!DemoStore.ApplicantsByGuid.TryGetValue(payload.applicantGuid, out var applicant))
            {
                return Results.BadRequest(new { message = "Applicant not found." });
            }

            // 제품 확인
            var product = DemoStore.FindProductByClientProductGuid(clientGuid, payload.clientProductGuid);
            if (product is null)
            {
                return Results.BadRequest(new { message = "Client product not found." });
            }

            // 파일번호/주문ID/시간
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var fileNumber = DemoStore.NextFileNumber();
            var orderGuid = SelectDeterministicGuidIfKnown(payload.externalIdentifier) ?? Guid.NewGuid().ToString();

            var order = new DemoStore.OrderRecord(
                orderGuid: orderGuid,
                clientGuid: clientGuid,
                fileNumber: fileNumber,
                orderStatus: "pending",
                orderType: DemoStore.OrderTypeEmployment,
                orderedDate: now,
                generalReportReference: payload.generalReportReference,
                externalIdentifier: payload.externalIdentifier,
                applicantGuid: applicant.applicantGuid,
                applicantName: DemoStore.ToApplicantNameUpper(applicant.firstName, applicant.lastName),
                clientName: DemoStore.ClientName,
                clientCode: DemoStore.ClientCode,
                productName: product.productName,
                requestedBy: DemoStore.RequestedBy,
                searchFlagged: false,
                createdDate: now,
                createdBy: DemoStore.RequestedBy,
                modifiedDate: now,
                modifiedBy: DemoStore.RequestedBy,
                clientProductGuid: product.clientProductGuid,
                applicantEmail: applicant.email.ToUpperInvariant(),
                applicantDateOfBirth: applicant.dateOfBirth,
                applicantPhone: applicant.phoneNumber,
                totalCharges: 0.0,
                orderNotes: payload.orderNotes,
                includedSearches: new() { "Person Search" },
                customFields: new(),
                completedDate: null
            );

            DemoStore.OrdersByGuid[orderGuid] = order;

            // 생성 응답(요약)
            var summary = new OrderSummaryResponse(
                orderGuid: order.orderGuid,
                fileNumber: order.fileNumber,
                orderStatus: order.orderStatus,
                orderType: order.orderType,
                orderedDate: order.orderedDate,
                generalReportReference: order.generalReportReference,
                externalIdentifier: order.externalIdentifier,
                applicantName: order.applicantName,
                clientName: order.clientName,
                clientCode: order.clientCode,
                productName: order.productName,
                requestedBy: order.requestedBy,
                searchFlagged: order.searchFlagged,
                createdDate: order.createdDate,
                createdBy: order.createdBy,
                modifiedDate: order.modifiedDate,
                modifiedBy: order.modifiedBy,
                completedDate: order.completedDate
            );

            return Results.Created($"/v1/clients/{clientGuid}/orders/{orderGuid}", summary);
        });

        // GET /v1/clients/{clientGuid}/orders?page=&size=
        endpoints.MapGet("/v1/clients/{clientGuid}/orders", (
            HttpRequest request,
            string clientGuid,
            int? page,
            int? size
        ) =>
        {
            if (!IsAuthorized(request)) return Results.Unauthorized();

            // 해당 클라이언트 주문만 필터
            var orders = DemoStore.OrdersByGuid.Values
                .Where(o => string.Equals(o.clientGuid, clientGuid, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(o => o.createdDate)
                .ToList();

            // 자동 완료 시뮬레이션(조회 시점에 상태 전환)
            for (int i = 0; i < orders.Count; i++)
            {
                var current = orders[i];
                DemoStore.TryAutoComplete(ref current);
                orders[i] = current;
            }

            var mapped = orders.Select(o => new OrderSummaryResponse(
                orderGuid: o.orderGuid,
                fileNumber: o.fileNumber,
                orderStatus: o.orderStatus,
                orderType: o.orderType,
                orderedDate: o.orderedDate,
                generalReportReference: o.generalReportReference,
                externalIdentifier: o.externalIdentifier,
                applicantName: o.applicantName,
                clientName: o.clientName,
                clientCode: o.clientCode,
                productName: o.productName,
                requestedBy: o.requestedBy,
                searchFlagged: o.searchFlagged,
                createdDate: o.createdDate,
                createdBy: o.createdBy,
                modifiedDate: o.modifiedDate,
                modifiedBy: o.modifiedBy,
                completedDate: o.completedDate
            )).ToList();

            if (page is null || size is null)
                return Results.Ok(mapped);

            var p = Math.Max(0, page.Value);
            var s = Math.Max(1, size.Value);

            return Results.Ok(mapped.Skip(p * s).Take(s).ToList());
        });

        // GET /v1/clients/{clientGuid}/orders/{orderGuid}
        endpoints.MapGet("/v1/clients/{clientGuid}/orders/{orderGuid}", (
            HttpRequest request,
            string clientGuid,
            string orderGuid
        ) =>
        {
            if (!IsAuthorized(request)) return Results.Unauthorized();

            if (!DemoStore.OrdersByGuid.TryGetValue(orderGuid, out var order) ||
                !string.Equals(order.clientGuid, clientGuid, StringComparison.OrdinalIgnoreCase))
            {
                return Results.NotFound(new { message = "Order not found." });
            }

            // 자동 완료 시뮬레이션
            DemoStore.TryAutoComplete(ref order);

            var detail = new OrderDetailResponse(
                orderGuid: order.orderGuid,
                fileNumber: order.fileNumber,
                orderStatus: order.orderStatus,
                orderType: order.orderType,
                orderedDate: order.orderedDate,
                generalReportReference: order.generalReportReference,
                externalIdentifier: order.externalIdentifier,
                completedDate: order.completedDate,
                applicantName: order.applicantName,
                clientName: order.clientName,
                clientCode: order.clientCode,
                productName: order.productName,
                requestedBy: order.requestedBy,
                searchFlagged: order.searchFlagged,
                createdDate: order.createdDate,
                createdBy: order.createdBy,
                modifiedDate: order.modifiedDate,
                modifiedBy: order.modifiedBy,
                clientProductGuid: order.clientProductGuid,
                applicantEmail: order.applicantEmail,
                applicantDateOfBirth: order.applicantDateOfBirth,
                applicantPhone: order.applicantPhone,
                applicantGuid: order.applicantGuid,
                totalCharges: order.totalCharges,
                orderNotes: order.orderNotes,
                includedSearches: order.includedSearches,
                customFields: order.customFields
            );

            return Results.Ok(detail);
        });
    }

    private static bool IsAuthorized(HttpRequest request)
    {
        if (!request.Headers.TryGetValue("Authorization", out var auth) || !auth.ToString().StartsWith("Bearer "))
            return false;
        var token = auth.ToString()["Bearer ".Length..];
        return string.Equals(token, FixedJwtToken, StringComparison.Ordinal);
    }

    // 샘플 시나리오를 쉽게 재현하려면 외부 식별자 기준으로 고정 GUID를 부여
    private static string? SelectDeterministicGuidIfKnown(string externalIdentifier) => externalIdentifier switch
    {
        "JOE-CLEAR-001" => "b84409e4-a9a8-4d0d-9d6f-2dc88bb00277",
        "HANK-BAD-002" => "6ab88112-d98f-422e-9395-17b15a64ce8d",
        _ => null
    };
}
