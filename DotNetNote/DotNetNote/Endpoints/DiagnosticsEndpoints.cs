using Microsoft.AspNetCore.Builder;   // MapGet 확장 메서드
using Microsoft.AspNetCore.Http;      // Results.*
using Microsoft.AspNetCore.Routing;   // IEndpointRouteBuilder
using System.Net.Http;
using System.Net.Http.Headers;

namespace Azunt.Endpoints;

public static class DiagnosticsEndpoints
{
    /// <summary>
    /// 운영 진단용 미니멀 API 묶음 등록 (인증 필요)
    /// </summary>
    public static IEndpointRouteBuilder MapDiagnosticsEndpoints(this IEndpointRouteBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        var endpoints = app;

        // 이 그룹에 포함된 모든 엔드포인트는 인증 필요
        var diag = endpoints.MapGroup("/api/diagnostics")
                            .WithTags("Diagnostics")
                            .RequireAuthorization();

        // 그룹에 포함된 엔드포인트 (자동으로 인증 필요)
        diag.MapGet("/egress-ip", async (IHttpClientFactory httpClientFactory, CancellationToken ct) =>
        {
            var client = httpClientFactory.CreateClient("egress-ip");

            static async Task<string?> TryGetAsync(HttpClient http, string url, CancellationToken ct)
            {
                try
                {
                    using var resp = await http.GetAsync(url, ct);
                    if (!resp.IsSuccessStatusCode) return null;

                    var text = (await resp.Content.ReadAsStringAsync(ct)).Trim();
                    return string.IsNullOrWhiteSpace(text) ? null : text;
                }
                catch
                {
                    return null;
                }
            }

            // 여러 공급자 중 되는 것 하나 사용 (순차 폴백)
            var ip =
                await TryGetAsync(client, "https://api.ipify.org", ct) ??
                await TryGetAsync(client, "https://ifconfig.me/ip", ct) ??
                await TryGetAsync(client, "https://checkip.amazonaws.com", ct);

            return ip is not null
                ? Results.Json(new { ip })
                : Results.Problem("Unable to determine outbound IP address right now. Try again shortly.");
        })
        .WithName("GetEgressIp")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status503ServiceUnavailable);

        // health 엔드포인트 (그룹 밖, 개별 RequireAuthorization 적용)
        endpoints.MapGet("/health/egress-ip", async (IHttpClientFactory httpClientFactory, CancellationToken ct) =>
        {
            var client = httpClientFactory.CreateClient("egress-ip");
            try
            {
                var ip = (await client.GetStringAsync("https://api.ipify.org", ct)).Trim();
                return Results.Text(ip, "text/plain");
            }
            catch
            {
                return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
        })
        .WithName("HealthEgressIp")
        .WithTags("Diagnostics")
        .RequireAuthorization();  // 개별 인증 요구

        return endpoints;
    }
}
