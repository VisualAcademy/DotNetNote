using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace DotNetNote.Endpoints;

public static class ApplicantsEndpoint
{
    // DemoStore에 있는 토큰/요청자/데모 클라이언트를 사용
    private const string FixedJwtToken = DemoStore.FixedJwtToken;

    // GET 응답용 요약 모델 (출력 형태 유지)
    private record ApplicantItem(
        string applicantGuid,
        string firstName,
        string lastName,
        string gender,
        string dateOfBirth,
        string email,
        string phoneNumber,
        bool textingEnabled,
        long createdDate,
        string createdBy,
        long modifiedDate,
        string modifiedBy,
        int version
    );

    // POST 요청용 입력 모델
    private record CreateApplicantRequest(
        string firstName,
        string lastName,
        string ssn,
        string gender,
        string dateOfBirth, // "YYYY-MM-DD"
        string email,
        string phoneNumber,
        bool? noMiddleName // 선택값, 미제공시 false 처리
    );

    // POST 응답용 상세 모델
    private record ApplicantDetailResponse(
        string applicantGuid,
        bool textingEnabled,
        string firstName,
        bool noMiddleName,
        string lastName,
        string gender,
        string ssn,          // 마스킹된 값
        string dateOfBirth,
        string email,
        string phoneNumber,
        long createdDate,
        string createdBy,
        long modifiedDate,
        string modifiedBy,
        int version,
        List<object> addresses,
        List<object> employment,
        List<object> education,
        List<object> aliases,
        List<object> professionalLicenses,
        List<object> references
    );

    public static void MapApplicantsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        // GET /v1/clients/{clientGuid}/applicants?page=0&size=30
        endpoints.MapGet("/v1/clients/{clientGuid}/applicants", (
            HttpRequest request,
            string clientGuid,
            int? page,
            int? size
        ) =>
        {
            if (!IsAuthorized(request)) return Results.Unauthorized();

            // 데모 시나리오: 데모 클라이언트 GUID일 때만 반환
            if (!string.Equals(clientGuid, DemoStore.DemoClientGuid, StringComparison.OrdinalIgnoreCase))
            {
                return Results.Ok(new List<ApplicantItem>());
            }

            var items = DemoStore.ApplicantsByGuid.Values
                .Select(s => new ApplicantItem(
                    applicantGuid: s.applicantGuid,
                    firstName: s.firstName,
                    lastName: s.lastName,
                    gender: s.gender,
                    dateOfBirth: s.dateOfBirth,
                    email: s.email,
                    phoneNumber: s.phoneNumber,
                    textingEnabled: s.textingEnabled,
                    createdDate: s.createdDate,
                    createdBy: s.createdBy,
                    modifiedDate: s.modifiedDate,
                    modifiedBy: s.modifiedBy,
                    version: s.version
                ))
                .ToList();

            if (page is null || size is null) return Results.Ok(items);

            var safePage = Math.Max(0, page.Value);
            var safeSize = Math.Max(1, size.Value);
            var paged = items.Skip(safePage * safeSize).Take(safeSize).ToList();

            return Results.Ok(paged);
        });

        // POST /v1/clients/{clientGuid}/applicants (항상 성공 저장)
        endpoints.MapPost("/v1/clients/{clientGuid}/applicants", async (
            HttpRequest request,
            string clientGuid
        ) =>
        {
            if (!IsAuthorized(request)) return Results.Unauthorized();

            // 데모 시나리오에서는 데모 클라이언트만 허용(원치 않으면 이 체크 제거)
            if (!string.Equals(clientGuid, DemoStore.DemoClientGuid, StringComparison.OrdinalIgnoreCase))
            {
                return Results.BadRequest(new { message = "Unsupported clientGuid for demo." });
            }

            var payload = await request.ReadFromJsonAsync<CreateApplicantRequest>();
            if (payload is null)
            {
                return Results.BadRequest(new { message = "Invalid JSON body." });
            }

            // 필수값 체크
            if (string.IsNullOrWhiteSpace(payload.firstName) ||
                string.IsNullOrWhiteSpace(payload.lastName) ||
                string.IsNullOrWhiteSpace(payload.ssn) ||
                string.IsNullOrWhiteSpace(payload.gender) ||
                string.IsNullOrWhiteSpace(payload.dateOfBirth) ||
                string.IsNullOrWhiteSpace(payload.email) ||
                string.IsNullOrWhiteSpace(payload.phoneNumber))
            {
                return Results.BadRequest(new { message = "Missing required fields." });
            }

            // 생성
            var nowMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var guid = Guid.NewGuid().ToString();
            var createdBy = DemoStore.RequestedBy;
            var modifiedBy = createdBy;

            // DemoStore에 저장(요약)
            var summary = new DemoStore.ApplicantSummary(
                applicantGuid: guid,
                firstName: payload.firstName,
                lastName: payload.lastName,
                gender: payload.gender,
                dateOfBirth: payload.dateOfBirth,
                email: payload.email,
                phoneNumber: payload.phoneNumber,
                textingEnabled: false,
                createdDate: nowMs,
                createdBy: createdBy,
                modifiedDate: nowMs,
                modifiedBy: modifiedBy,
                version: 1
            );

            DemoStore.ApplicantsByGuid[guid] = summary;

            // 상세 응답
            var response = new ApplicantDetailResponse(
                applicantGuid: guid,
                textingEnabled: false,
                firstName: payload.firstName,
                noMiddleName: payload.noMiddleName ?? false,
                lastName: payload.lastName,
                gender: payload.gender,
                ssn: MaskSsn(payload.ssn), // 마지막 4자리만 노출
                dateOfBirth: payload.dateOfBirth,
                email: payload.email,
                phoneNumber: payload.phoneNumber,
                createdDate: nowMs,
                createdBy: createdBy,
                modifiedDate: nowMs + 11,
                modifiedBy: modifiedBy,
                version: 1,
                addresses: new(),
                employment: new(),
                education: new(),
                aliases: new(),
                professionalLicenses: new(),
                references: new()
            );

            return Results.Created($"/v1/clients/{clientGuid}/applicants/{guid}", response);
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

    private static string MaskSsn(string ssn)
    {
        // 마지막 4자리만 노출
        var last4 = new string(ssn.Where(char.IsDigit).TakeLast(4).ToArray());
        return $"XXX-XX-{last4.PadLeft(4, '0')}";
    }
}
