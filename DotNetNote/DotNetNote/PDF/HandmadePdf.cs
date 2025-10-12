// File: DotNetNote/PDF/HandmadePdf.cs
using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

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

        // Courier(모노스페이스) 가정 하의 텍스트 폭 계산 (글자당 0.6 * fontSize pt)
        private static double MeasureTextWidth(string text, double fontSize)
            => (text?.Length ?? 0) * fontSize * 0.6;

        /// <summary>
        /// 인보이스 1페이지 PDF 생성 (A4, 단순 테이블)
        /// </summary>
        public static byte[] BuildInvoicePdf(Invoice vm)
        {
            // A4 (pt, 72dpi)
            const int pageW = 595, pageH = 842;
            const int left = 50, right = 545;

            // 테이블 수직 경계(세로선) - 오른쪽에서부터 역산
            // [left][#][Item]........[Qty][Price][Total][right]
            const int totalColWidth = 75; // Total 칸 폭
            const int priceColWidth = 70; // Price 칸 폭
            const int qtyColWidth = 50; // Qty 칸 폭
            const int noColWidth = 35; // # 칸 폭 (왼쪽 고정)
            int vRight = right;                               // 맨 오른쪽 세로선
            int vTotal = vRight - totalColWidth;              // Total 왼쪽 경계
            int vPrice = vTotal - priceColWidth;              // Price 왼쪽 경계
            int vQty = vPrice - qtyColWidth;                // Qty 왼쪽 경계
            int vItem = left + noColWidth;                   // Item 왼쪽 경계
            int vLeft = left;                                // 맨 왼쪽 세로선

            int y = 800;               // 현재 텍스트 Y
            int rowH = 20;             // 테이블 행 높이

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

            var dateText = "Date:   " + vm.IssueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            sb.AppendLine($"BT /F1 12 Tf 50 {y} Td ({PdfEscape(dateText)}) Tj ET");

            // 테이블 헤더
            y -= 30;
            int tableTop = y;

            // 헤더 외곽선 (가로)
            sb.AppendLine($"{vLeft} {tableTop} m {vRight} {tableTop} l S");
            sb.AppendLine($"{vLeft} {tableTop - rowH} m {vRight} {tableTop - rowH} l S");

            // 헤더 세로선
            sb.AppendLine($"{vLeft} {tableTop} m {vLeft} {tableTop - rowH} l S");
            sb.AppendLine($"{vItem} {tableTop} m {vItem} {tableTop - rowH} l S");
            sb.AppendLine($"{vQty} {tableTop} m {vQty} {tableTop - rowH} l S");
            sb.AppendLine($"{vPrice} {tableTop} m {vPrice} {tableTop - rowH} l S");
            sb.AppendLine($"{vTotal} {tableTop} m {vTotal} {tableTop - rowH} l S");
            sb.AppendLine($"{vRight} {tableTop} m {vRight} {tableTop - rowH} l S");

            // 헤더 텍스트 (Item은 좌정렬, 나머지 숫자열은 우정렬 느낌으로 배치)
            double f12 = 12.0;
            int pad = 3;

            // "#": 좌측 칸 시작+패딩
            sb.AppendLine($"BT /F1 12 Tf {vLeft + pad} {tableTop - 15} Td (#) Tj ET");
            // "Item": vItem + pad
            sb.AppendLine($"BT /F1 12 Tf {vItem + pad} {tableTop - 15} Td (Item) Tj ET");
            // "Qty": vQty~vPrice 사이 우측 정렬
            var qtyHeader = "Qty";
            int qtyHeaderX = (int)Math.Round(vPrice - pad - MeasureTextWidth(qtyHeader, f12));
            sb.AppendLine($"BT /F1 12 Tf {qtyHeaderX} {tableTop - 15} Td ({qtyHeader}) Tj ET");
            // "Price"
            var priceHeader = "Price";
            int priceHeaderX = (int)Math.Round(vTotal - pad - MeasureTextWidth(priceHeader, f12));
            sb.AppendLine($"BT /F1 12 Tf {priceHeaderX} {tableTop - 15} Td ({priceHeader}) Tj ET");
            // "Total"
            var totalHeader = "Total";
            int totalHeaderX = (int)Math.Round(vRight - pad - MeasureTextWidth(totalHeader, f12));
            sb.AppendLine($"BT /F1 12 Tf {totalHeaderX} {tableTop - 15} Td ({totalHeader}) Tj ET");

            // 바디 행들
            int i = 1;
            decimal grand = 0m;
            int currentY = tableTop - rowH;

            foreach (var line in vm.Lines)
            {
                currentY -= rowH;

                // 가로줄
                sb.AppendLine($"{vLeft} {currentY} m {vRight} {currentY} l S");

                // 세로줄(각 컬럼 경계)
                sb.AppendLine($"{vLeft} {currentY + rowH} m {vLeft} {currentY} l S");
                sb.AppendLine($"{vItem} {currentY + rowH} m {vItem} {currentY} l S");
                sb.AppendLine($"{vQty} {currentY + rowH} m {vQty} {currentY} l S");
                sb.AppendLine($"{vPrice} {currentY + rowH} m {vPrice} {currentY} l S");
                sb.AppendLine($"{vTotal} {currentY + rowH} m {vTotal} {currentY} l S");
                sb.AppendLine($"{vRight} {currentY + rowH} m {vRight} {currentY} l S");

                // 텍스트: #, Item 좌정렬
                sb.AppendLine($"BT /F1 12 Tf {vLeft + pad} {currentY + 5} Td ({i}) Tj ET");
                sb.AppendLine($"BT /F1 12 Tf {vItem + pad} {currentY + 5} Td ({PdfEscape(line.Name)}) Tj ET");

                // Qty/Price/Total 우정렬
                var qtyText = line.Quantity.ToString(CultureInfo.InvariantCulture);
                int qtyX = (int)Math.Round(vPrice - pad - MeasureTextWidth(qtyText, f12));
                sb.AppendLine($"BT /F1 12 Tf {qtyX} {currentY + 5} Td ({qtyText}) Tj ET");

                var priceText = FormatMoney(line.UnitPrice);
                int priceX = (int)Math.Round(vTotal - pad - MeasureTextWidth(priceText, f12));
                sb.AppendLine($"BT /F1 12 Tf {priceX} {currentY + 5} Td ({PdfEscape(priceText)}) Tj ET");

                var total = line.LineTotal;
                grand += total;
                var totalText = FormatMoney(total);
                int totalX = (int)Math.Round(vRight - pad - MeasureTextWidth(totalText, f12));
                sb.AppendLine($"BT /F1 12 Tf {totalX} {currentY + 5} Td ({PdfEscape(totalText)}) Tj ET");

                i++;
            }

            // 바디 마지막 하단 가로줄
            sb.AppendLine($"{vLeft} {currentY} m {vRight} {currentY} l S");

            // 합계 표시 (우측 정렬로 합계값 넣기)
            int sumY = currentY - 30;
            var grandLabel = "Grand Total:";
            int grandLabelX = vTotal - pad - (int)Math.Round(MeasureTextWidth(grandLabel, f12));
            sb.AppendLine($"BT /F1 12 Tf {grandLabelX} {sumY} Td ({grandLabel}) Tj ET");

            var grandText = FormatMoney(grand);
            int grandTextX = vRight - pad - (int)Math.Round(MeasureTextWidth(grandText, f12));
            sb.AppendLine($"BT /F1 12 Tf {grandTextX} {sumY} Td ({PdfEscape(grandText)}) Tj ET");

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

    // 데모 컨트롤러 (원하시면 별도 파일로 분리)
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
