using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Azunt.Web.Infrastructure.Extensions;

/// <summary>
/// WebApplication 확장 메서드를 정의하는 클래스.
/// Minimal API 라우트들을 한 곳에 모아두고
/// Program.cs에서 MapAzuntMinimalApis() 한 줄로 등록할 수 있게 합니다.
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Azunt.Web 프로젝트의 공통 Minimal API 엔드포인트들을 매핑합니다.
    /// - /api/ping : 단순 핑 테스트용 API (현재 UTC 시간 반환)
    /// 추후 이 지점에 다른 Minimal API 엔드포인트들도 추가 가능합니다.
    /// </summary>
    /// <param name="app">WebApplication 인스턴스</param>
    /// <returns>메서드 체이닝을 위해 WebApplication 자체 반환</returns>
    public static WebApplication MapAzuntMinimalApis(this WebApplication app)
    {
        // Minimal API 샘플 엔드포인트
        // GET /api/ping → { pong = <현재 UTC 시각> }
        app.MapGet("/api/ping", () => Results.Ok(new { pong = DateTime.UtcNow }));

        return app;
    }
}
