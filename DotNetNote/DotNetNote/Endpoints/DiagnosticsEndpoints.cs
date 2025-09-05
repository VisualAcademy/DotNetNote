using Microsoft.AspNetCore.Builder;   // MapGet 확장메서드
using Microsoft.AspNetCore.Http;      // Results.*
using Microsoft.AspNetCore.Routing;   // IEndpointRouteBuilder
using System.Net.Http;
using System.Net.Http.Headers;

namespace Azunt.Endpoints;

public static class DiagnosticsEndpoints
{
    /// <summary>
    /// 운영 진단용 미니멀 API 묶음 등록
    /// </summary>
    public static IEndpointRouteBuilder MapDiagnosticsEndpoints(this IEndpointRouteBuilder app)
    {
        // nullable 경고 방지 + 런타임 안전
        ArgumentNullException.ThrowIfNull(app);
        var endpoints = app;

        // JSON 응답: { "ip": "x.x.x.x" }
        endpoints.MapGet("/api/diagnostics/egress-ip", async (IHttpClientFactory httpClientFactory, CancellationToken ct) =>
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
        .WithTags("Diagnostics")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status503ServiceUnavailable);

        // 텍스트 응답(헬스체크/스크립트에서 간단 확인): x.x.x.x
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
        .WithTags("Diagnostics");

        return endpoints;
    }
}
