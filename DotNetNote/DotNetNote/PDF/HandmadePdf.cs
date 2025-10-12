// File: DotNetNote/PDF/HandmadePdf.cs
using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

namespace DotNetNote.PDF
{
    // ====== Invoice 모델 ======
    public record Invoice(
        int Id,
        string SellerName,
        string BuyerName,
        DateTime IssueDate,
        List<InvoiceLine> Lines
    );

    public record InvoiceLine(string Name, int Quantity, decimal UnitPrice)
    {
        public decimal LineTotal => Quantity * UnitPrice;
    }

    // ====== 순수 PDF 생성기 ======
    public static class HandmadePdf
    {
        // PDF 문자열 내 이스케이프: (), \ 문자 처리
        private static string PdfEscape(string s) =>
            s.Replace(@"\", @"\\").Replace("(", @"\(").Replace(")", @"\)");

        // 통화 표기 (ASCII 안전하게)
        private static string FormatMoney(decimal v) =>
            "KRW " + string.Format(CultureInfo.InvariantCulture, "{0:N0}", v);

        /// <summary>
        /// 인보이스 1페이지 PDF 생성 (A4, 단순 테이블)
        /// </summary>
        public static byte[] BuildInvoicePdf(Invoice vm)
        {
            // A4 (pt, 72dpi)
            const int pageW = 595, pageH = 842;
            const int left = 50, right = 545;

            int y = 800;               // 현재 텍스트 Y
            int rowH = 20;             // 테이블 행 높이
            int xNo = left;            // 컬럼 X 좌표들
            int xItem = left + 40;
            int xQty = left + 360;
            int xPrice = left + 420;
            int xTotal = left + 500;

            // ===== 1) 콘텐츠 스트림 조립 =====
            var sb = new StringBuilder();

            // 선 두께
            sb.AppendLine("1 w");

            // 제목
            sb.AppendLine($"BT /F1 18 Tf 50 800 Td ({PdfEscape($"Invoice #{vm.Id}")}) Tj ET");

            // 메타 정보
            y -= 30;
            sb.AppendLine($"BT /F1 12 Tf 50 {y} Td ({PdfEscape("Seller: " + vm.SellerName)}) Tj ET");
            y -= 16;
            sb.AppendLine($"BT /F1 12 Tf 50 {y} Td ({PdfEscape("Buyer:  " + vm.BuyerName)}) Tj ET");
            y -= 16;

            // *** 여기서 날짜 포맷을 ToString("yyyy-MM-dd")로 명확히 지정 ***
            var dateText = "Date:   " + vm.IssueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            sb.AppendLine($"BT /F1 12 Tf 50 {y} Td ({PdfEscape(dateText)}) Tj ET");

            // 테이블 헤더
            y -= 30;
            int tableTop = y;

            // 헤더 외곽선 (가로)
            sb.AppendLine($"{left} {tableTop} m {right} {tableTop} l S");
            sb.AppendLine($"{left} {tableTop - rowH} m {right} {tableTop - rowH} l S");
            // 헤더 세로선
            sb.AppendLine($"{left} {tableTop} m {left} {tableTop - rowH} l S");
            sb.AppendLine($"{xItem - 5} {tableTop} m {xItem - 5} {tableTop - rowH} l S");
            sb.AppendLine($"{xQty - 5} {tableTop} m {xQty - 5} {tableTop - rowH} l S");
            sb.AppendLine($"{xPrice - 5} {tableTop} m {xPrice - 5} {tableTop - rowH} l S");
            sb.AppendLine($"{xTotal - 5} {tableTop} m {xTotal - 5} {tableTop - rowH} l S");
            sb.AppendLine($"{right} {tableTop} m {right} {tableTop - rowH} l S");

            // 헤더 텍스트
            sb.AppendLine($"BT /F1 12 Tf {xNo + 5} {tableTop - 15} Td (#) Tj ET");
            sb.AppendLine($"BT /F1 12 Tf {xItem} {tableTop - 15} Td (Item) Tj ET");
            sb.AppendLine($"BT /F1 12 Tf {xQty} {tableTop - 15} Td (Qty) Tj ET");
            sb.AppendLine($"BT /F1 12 Tf {xPrice} {tableTop - 15} Td (Price) Tj ET");
            sb.AppendLine($"BT /F1 12 Tf {xTotal} {tableTop - 15} Td (Total) Tj ET");

            // 바디 행들
            int i = 1;
            decimal grand = 0m;
            int currentY = tableTop - rowH;

            foreach (var line in vm.Lines)
            {
                currentY -= rowH;

                // 가로줄
                sb.AppendLine($"{left} {currentY} m {right} {currentY} l S");

                // 세로줄(각 컬럼 경계)
                sb.AppendLine($"{left} {currentY + rowH} m {left} {currentY} l S");
                sb.AppendLine($"{xItem - 5} {currentY + rowH} m {xItem - 5} {currentY} l S");
                sb.AppendLine($"{xQty - 5} {currentY + rowH} m {xQty - 5} {currentY} l S");
                sb.AppendLine($"{xPrice - 5} {currentY + rowH} m {xPrice - 5} {currentY} l S");
                sb.AppendLine($"{xTotal - 5} {currentY + rowH} m {xTotal - 5} {currentY} l S");
                sb.AppendLine($"{right} {currentY + rowH} m {right} {currentY} l S");

                // 셀 텍스트
                sb.AppendLine($"BT /F1 12 Tf {xNo + 5} {currentY + 5} Td ({i}) Tj ET");
                sb.AppendLine($"BT /F1 12 Tf {xItem} {currentY + 5} Td ({PdfEscape(line.Name)}) Tj ET");
                sb.AppendLine($"BT /F1 12 Tf {xQty} {currentY + 5} Td ({line.Quantity}) Tj ET");
                sb.AppendLine($"BT /F1 12 Tf {xPrice} {currentY + 5} Td ({PdfEscape(FormatMoney(line.UnitPrice))}) Tj ET");

                var total = line.LineTotal;
                grand += total;
                sb.AppendLine($"BT /F1 12 Tf {xTotal} {currentY + 5} Td ({PdfEscape(FormatMoney(total))}) Tj ET");

                i++;
            }

            // 바디 마지막 하단 가로줄
            sb.AppendLine($"{left} {currentY} m {right} {currentY} l S");

            // 합계 표시
            int sumY = currentY - 30;
            sb.AppendLine($"BT /F1 12 Tf 400 {sumY} Td (Grand Total:) Tj ET");
            sb.AppendLine($"BT /F1 12 Tf 500 {sumY} Td ({PdfEscape(FormatMoney(grand))}) Tj ET");

            // 페이지 번호
            sb.AppendLine("BT /F1 10 Tf 290 20 Td (Page 1) Tj ET");

            // 스트림 바이트
            var content = sb.ToString();
            var contentBytes = Encoding.ASCII.GetBytes(content);

            // ===== 2) PDF 객체/오프셋/xref 작성 =====
            using var ms = new MemoryStream();
            using var w = new StreamWriter(ms, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), 1024, leaveOpen: true);

            // Header
            w.WriteLine("%PDF-1.4");

            // 1 0 obj : Catalog
            long o1 = ms.Length;
            w.WriteLine("1 0 obj << /Type /Catalog /Pages 2 0 R >> endobj");

            // 2 0 obj : Pages
            long o2 = ms.Length;
            w.WriteLine("2 0 obj << /Type /Pages /Kids [3 0 R] /Count 1 >> endobj");

            // 3 0 obj : Page
            long o3 = ms.Length;
            w.WriteLine($"3 0 obj << /Type /Page /Parent 2 0 R /MediaBox [0 0 {pageW} {pageH}] /Resources << /Font << /F1 5 0 R >> >> /Contents 4 0 R >> endobj");

            // 4 0 obj : Contents (stream)
            long o4 = ms.Length;
            w.WriteLine($"4 0 obj << /Length {contentBytes.Length} >> stream");
            w.Flush();

            ms.Write(contentBytes, 0, contentBytes.Length);

            w.WriteLine();
            w.WriteLine("endstream endobj");

            // 5 0 obj : Font (표준 14 폰트 - Courier)
            long o5 = ms.Length;
            w.WriteLine("5 0 obj << /Type /Font /Subtype /Type1 /BaseFont /Courier >> endobj");

            // xref
            long xrefPos = ms.Length;
            w.WriteLine("xref");
            w.WriteLine("0 6");
            w.WriteLine("0000000000 65535 f ");
            w.WriteLine($"{o1:0000000000} 00000 n ");
            w.WriteLine($"{o2:0000000000} 00000 n ");
            w.WriteLine($"{o3:0000000000} 00000 n ");
            w.WriteLine($"{o4:0000000000} 00000 n ");
            w.WriteLine($"{o5:0000000000} 00000 n ");

            // trailer
            w.WriteLine("trailer << /Size 6 /Root 1 0 R >>");
            w.WriteLine("startxref");
            w.WriteLine(xrefPos);
            w.WriteLine("%%EOF");
            w.Flush();

            return ms.ToArray();
        }
    }

    public class InvoicesController : Controller
    {
        [HttpGet("/invoices/{id}/pdf-handmade")]
        public IActionResult ExportHandmade(int id)
        {
            // 데모 데이터
            var vm = new Invoice(
                Id: id,
                SellerName: "VisualAcademy Inc.",
                BuyerName: "Contoso Ltd.",
                IssueDate: DateTime.UtcNow.Date,
                Lines: new()
                {
                new InvoiceLine("Service A", 2, 150000),
                new InvoiceLine("Service B", 1, 320000),
                new InvoiceLine("Maintenance", 3, 80000),
                }
            );

            var bytes = HandmadePdf.BuildInvoicePdf(vm);
            return File(bytes, "application/pdf", $"Invoice-{id}.pdf");
        }
    }
}
