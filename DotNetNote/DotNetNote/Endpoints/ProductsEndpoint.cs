using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace DotNetNote.Endpoints;

public static class ProductsEndpoint
{
    // DemoStore의 공통 상수 사용
    private const string FixedJwtToken = DemoStore.FixedJwtToken;

    // 응답용 아이템(출력 스키마 유지)
    private record ClientProductItem(
        string clientProductGuid,
        string productGuid,
        string productName,
        bool usesQuickapp,
        long? dateModified = null
    );

    public static void MapClientProductsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        // GET /v1/clients/{clientGuid}/products
        endpoints.MapGet("/v1/clients/{clientGuid}/products", (
            HttpRequest request,
            string clientGuid
        ) =>
        {
            if (!IsAuthorized(request)) return Results.Unauthorized();

            // 요청 clientGuid가 없으면 Demo 클라이언트의 제품으로 대체
            var src = DemoStore.ProductsByClient.GetValueOrDefault(clientGuid)
                      ?? DemoStore.ProductsByClient.GetValueOrDefault(DemoStore.DemoClientGuid)
                      ?? new List<DemoStore.ClientProduct>();

            var list = src.Select(p => new ClientProductItem(
                clientProductGuid: p.clientProductGuid,
                productGuid: p.productGuid,
                productName: p.productName,
                usesQuickapp: p.usesQuickapp,
                dateModified: p.dateModified
            )).ToList();

            return Results.Ok(list);
        });
    }

    private static bool IsAuthorized(HttpRequest request)
    {
        if (!request.Headers.TryGetValue("Authorization", out var auth) ||
            !auth.ToString().StartsWith("Bearer "))
        {
            return false;
        }
        var token = auth.ToString()["Bearer ".Length..];
        return string.Equals(token, FixedJwtToken, StringComparison.Ordinal);
    }
}
