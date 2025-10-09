using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace DotNetNote.Endpoints;

public static class ProductFieldsEndpoint
{
    private const string FixedJwtToken = DemoStore.FixedJwtToken;

    // 공통 필드 모델
    private record FieldItem(string name, List<string>? predefinedOptions = null);

    // 검색 필드 묶음
    private record SearchField(
        string search,
        string orderingOption, // "default" | "required" | "optional"
        List<FieldItem> personalIdentifierRequiredFields,
        List<FieldItem> personalIdentifierOptionalFields,
        List<FieldItem> searchRequiredFields,
        List<FieldItem> searchOptionalFields
    );

    // 최상위 응답
    private record ProductFieldsResponse(
        string clientProductGuid,
        string productName,
        List<FieldItem> optionalFields,
        List<FieldItem> requiredFields,
        List<SearchField> searchFields
    );

    public static void MapProductFieldsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        // GET /v1/clients/{clientGuid}/products/{clientProductGuid}/fields
        endpoints.MapGet("/v1/clients/{clientGuid}/products/{clientProductGuid}/fields", (
            HttpRequest request,
            string clientGuid,
            string clientProductGuid
        ) =>
        {
            if (!IsAuthorized(request)) return Results.Unauthorized();

            // 데모 스토어에서 제품 찾기(클라이언트-제품 매칭)
            var product = DemoStore.FindProductByClientProductGuid(clientGuid, clientProductGuid);
            if (product is null)
            {
                return Results.NotFound(new { message = "Client product not found." });
            }

            // ====== 샘플 콘텐츠 ======
            var optionalFields = new List<FieldItem>
            {
                new("Employee ID"),
                new("Union Member", new() { "Yes", "No" }),
                new("Work Shift",  new() { "DAY", "SWING", "GRAVEYARD" })
            };

            var requiredFields = new List<FieldItem>
            {
                new("Reference", new() { "OPERATIONS", "FINANCE", "SECURITY", "HUMAN RESOURCES", "INFORMATION TECHNOLOGY" }), // 무조건 이 값으로 테스트
                new("Employment Type", new() { "FULL_TIME", "PART_TIME", "TEMPORARY", "CONTRACTOR" })
            };

            var searchFields = new List<SearchField>
            {
                new(
                    search: "PERSON_SEARCH",
                    orderingOption: "default",
                    personalIdentifierRequiredFields: new()
                    {
                        new("First name"),
                        new("Last name")
                    },
                    personalIdentifierOptionalFields: new()
                    {
                        new("Middle name"),
                        new("Birth date"),
                        new("Social Security number")
                    },
                    searchRequiredFields: new(),
                    searchOptionalFields: new()
                    {
                        // 검색 단계에서 선택 가능한 프리셋(예: 커버리지 관할)
                        new("Jurisdictions", new() { "CA", "NV", "AZ", "WA" })
                    }
                ),
                new(
                    search: "STATE_CRIMINAL_RECORD",
                    orderingOption: "required",
                    personalIdentifierRequiredFields: new()
                    {
                        new("First name"),
                        new("Last name"),
                        new("Birth date")
                    },
                    personalIdentifierOptionalFields: new()
                    {
                        new("Middle name"),
                        new("Generation"),
                        new("Social Security number")
                    },
                    searchRequiredFields: new()
                    {
                        new("State", new() { "CA", "NV", "AZ", "OR", "UT" })
                    },
                    searchOptionalFields: new()
                    {
                        new("Include Aliases", new() { "True", "False" })
                    }
                )
            };
            // =====================================================

            var response = new ProductFieldsResponse(
                clientProductGuid: product.clientProductGuid,
                productName: product.productName,
                optionalFields: optionalFields,
                requiredFields: requiredFields,
                searchFields: searchFields
            );

            return Results.Ok(response);
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
