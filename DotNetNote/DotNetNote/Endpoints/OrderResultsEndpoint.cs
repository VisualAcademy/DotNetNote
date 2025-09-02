using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace DotNetNote.Endpoints;

public static class OrderResultsEndpoint
{
    private const string FixedJwtToken = DemoStore.FixedJwtToken;

    public static void MapOrderResultsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        // JSON: 결과 PDF URL 반환
        // GET /v1/clients/{clientGuid}/orders/{orderGuid}/resultsAsPdf
        endpoints.MapGet("/v1/clients/{clientGuid}/orders/{orderGuid}/resultsAsPdf", (
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

            var scheme = string.IsNullOrWhiteSpace(request.Scheme) ? "https" : request.Scheme;
            var host = request.Host.HasValue ? request.Host.Value : "localhost";
            // 파일 직접 다운로드를 원하면 ?download=1 유지
            var resultsUrl = $"{scheme}://{host}/v1/reports/{order.fileNumber}.pdf?download=1";

            return Results.Ok(new
            {
                resultsUrl,
                externalIdentifier = order.externalIdentifier
            });
        });

        // PDF 스트리밍 엔드포인트
        // GET /v1/reports/{fileNumber}.pdf?download=1
        //  - download=1 이면 다운로드(attachment)
        //  - 없으면 브라우저 내 인라인 보기
        endpoints.MapGet("/v1/reports/{fileNumber}.pdf", (
            HttpRequest request,
            int fileNumber
        ) =>
        {
            var download = string.Equals(request.Query["download"], "1", StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(request.Query["download"], "true", StringComparison.OrdinalIgnoreCase);

            // 주문 조회(있으면 PDF 안의 텍스트에 반영)
            DemoStore.OrderRecord? order = DemoStore.OrdersByGuid.Values.FirstOrDefault(o => o.fileNumber == fileNumber);

            var pdfBytes = BuildSimplePdf(fileNumber, order);

            // download=1 이면 파일명 제공하여 다운로드 유도
            if (download)
            {
                return Results.File(
                    fileContents: pdfBytes,
                    contentType: "application/pdf",
                    fileDownloadName: $"Order-{fileNumber}.pdf",
                    enableRangeProcessing: true
                );
            }

            // 인라인 표시(파일명 없음 → 대부분 브라우저가 뷰어로 표시)
            return Results.File(
                fileContents: pdfBytes,
                contentType: "application/pdf",
                enableRangeProcessing: true
            );
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

    /// <summary>
    /// 외부 라이브러리 없이, 아주 간단한 1페이지 PDF를 동적으로 생성합니다.
    /// (정상 xref를 포함하여 표준 뷰어에서 열립니다)
    /// </summary>
    private static byte[] BuildSimplePdf(int fileNumber, DemoStore.OrderRecord? order)
    {
        // 페이지에 찍을 텍스트
        var title = $"Demo Report #{fileNumber}";
        var sub = order is null
            ? "No order metadata found."
            : $"{order.applicantName}  |  {order.productName}  |  Status: {order.orderStatus}";

        // PDF 오브젝트 구성
        var header = "%PDF-1.4\n";
        var objects = new List<string>();

        // 1: Catalog
        objects.Add("1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n");

        // 2: Pages
        objects.Add("2 0 obj\n<< /Type /Pages /Kids [3 0 R] /Count 1 >>\nendobj\n");

        // 3: Page (Letter 612x792)
        objects.Add("3 0 obj\n<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Contents 4 0 R /Resources << /Font << /F1 5 0 R >> >> >>\nendobj\n");

        // 4: Contents (간단한 텍스트: 제목 + 보조)
        // PDF 텍스트: 72,720 좌표에 제목, 그 아래 30pt 보조 텍스트
        var contentStream = $"BT /F1 24 Tf 72 720 Td ({EscapePdfText(title)}) Tj ET\n" +
                            $"BT /F1 12 Tf 72 690 Td ({EscapePdfText(sub)}) Tj ET\n";

        var contents = $"4 0 obj\n<< /Length {Encoding.ASCII.GetByteCount(contentStream)} >>\nstream\n{contentStream}endstream\nendobj\n";
        objects.Add(contents);

        // 5: Font
        objects.Add("5 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>\nendobj\n");

        // xref 오프셋 계산
        var all = new StringBuilder();
        all.Append(header);
        var offsets = new List<int> { 0 }; // obj index 0 (free) 자리
        foreach (var obj in objects)
        {
            offsets.Add(all.Length);
            all.Append(obj);
        }
        var xrefPos = all.Length;

        // xref 테이블
        var sbXref = new StringBuilder();
        sbXref.Append("xref\n");
        sbXref.Append($"0 {objects.Count + 1}\n");
        sbXref.Append("0000000000 65535 f \n");
        foreach (var off in offsets.Skip(1))
        {
            sbXref.Append(off.ToString("D10")).Append(" 00000 n \n");
        }

        // trailer
        var trailer = $"trailer\n<< /Size {objects.Count + 1} /Root 1 0 R >>\nstartxref\n{xrefPos}\n%%EOF\n";

        all.Append(sbXref.ToString());
        all.Append(trailer);

        return Encoding.ASCII.GetBytes(all.ToString());
    }

    private static string EscapePdfText(string s)
    {
        // PDF 문자열 리터럴 내에서 (), \ 는 이스케이프 필요
        return s.Replace(@"\", @"\\").Replace("(", @"\(").Replace(")", @"\)");
    }
}
