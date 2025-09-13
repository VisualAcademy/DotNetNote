using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace DotNetNote.Controllers
{
    [Route("excel-demo")]
    public class ExcelDemoController : Controller
    {
        // GET /excel-demo/download
        [HttpGet("download")]
        public IActionResult Download()
        {
            using var ms = new MemoryStream();
            using (var doc = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook, true))
            {
                var wbPart = doc.AddWorkbookPart();
                wbPart.Workbook = new Workbook();

                var wsPart = wbPart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                wsPart.Worksheet = new Worksheet(sheetData);

                var sheets = wbPart.Workbook.AppendChild(new Sheets());
                sheets.Append(new Sheet
                {
                    Id = wbPart.GetIdOfPart(wsPart),
                    SheetId = 1,
                    Name = "Demo"
                });

                // A1:B2 (헤더 + 데이터 한 줄)
                var header = new Row { RowIndex = 1 };
                header.Append(TextCell("A1", "Name"));
                header.Append(TextCell("B1", "Score"));
                sheetData.Append(header);

                var row2 = new Row { RowIndex = 2 };
                row2.Append(TextCell("A2", "Alice"));
                row2.Append(TextCell("B2", "95"));
                sheetData.Append(row2);

                wsPart.Worksheet.Save();
                wbPart.Workbook.Save();
            }

            return File(
                ms.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"demo_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }

        // GET /excel-demo/upload (간단 업로드 폼)
        [HttpGet("upload")]
        public IActionResult UploadForm()
        {
            var html = """
            <html><body>
              <h3>Upload 2x2 Excel (A1:B2)</h3>
              <form action="/excel-demo/upload" method="post" enctype="multipart/form-data">
                <input type="file" name="file" accept=".xlsx" required />
                <button type="submit">Upload</button>
              </form>
            </body></html>
            """;
            return Content(html, "text/html", Encoding.UTF8);
        }

        // POST /excel-demo/upload
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file is null || file.Length == 0)
                return BadRequest("No file uploaded.");

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            ms.Position = 0;

            string[,] cells = new string[2, 2]; // [row, col] → 0..1

            using (var doc = SpreadsheetDocument.Open(ms, false))
            {
                var wbPart = doc.WorkbookPart;
                if (wbPart is null)
                    return BadRequest("Invalid workbook.");

                var sheet = wbPart.Workbook?.Sheets?.Elements<Sheet>().FirstOrDefault();
                if (sheet is null || sheet.Id is null)
                    return BadRequest("No worksheet found.");

                var wsPart = wbPart.GetPartById(sheet.Id!) as WorksheetPart;
                if (wsPart is null)
                    return BadRequest("Worksheet part not found.");

                // A1,B1 (헤더), A2,B2 (데이터)
                cells[0, 0] = GetCellText(wsPart, wbPart, "A1");
                cells[0, 1] = GetCellText(wsPart, wbPart, "B1");
                cells[1, 0] = GetCellText(wsPart, wbPart, "A2");
                cells[1, 1] = GetCellText(wsPart, wbPart, "B2");
            }

            // 간단 HTML 테이블로 출력
            var html = $"""
            <html><body>
              <h3>Loaded A1:B2</h3>
              <table border="1" cellpadding="6" cellspacing="0">
                <tr><th>{Html(cells[0, 0])}</th><th>{Html(cells[0, 1])}</th></tr>
                <tr><td>{Html(cells[1, 0])}</td><td>{Html(cells[1, 1])}</td></tr>
              </table>
              <p><a href="/excel-demo/upload">Back</a> | <a href="/excel-demo/download">Download sample</a></p>
            </body></html>
            """;
            return Content(html, "text/html", Encoding.UTF8);
        }

        // ===== Helpers =====

        private static Cell TextCell(string cellRef, string text) =>
            new Cell
            {
                CellReference = cellRef,
                DataType = CellValues.String,
                CellValue = new CellValue(text ?? string.Empty)
            };

        private static string GetCellText(WorksheetPart wsPart, WorkbookPart wbPart, string cellRef)
        {
            var cell = GetCell(wsPart, cellRef);
            if (cell is null) return string.Empty;

            // SharedString
            if (cell.DataType?.Value == CellValues.SharedString)
            {
                if (int.TryParse(cell.CellValue?.Text, out var idx))
                {
                    var sst = wbPart.SharedStringTablePart?.SharedStringTable;
                    return sst?.ElementAtOrDefault(idx)?.InnerText ?? string.Empty;
                }
                return string.Empty;
            }

            // InlineString
            if (cell.DataType?.Value == CellValues.InlineString)
                return cell.InlineString?.Text?.Text ?? string.Empty;

            // 숫자/일반 문자열
            return cell.CellValue?.Text ?? string.Empty;
        }

        private static Cell? GetCell(WorksheetPart wsPart, string cellRef)
        {
            var sheetData = wsPart.Worksheet.GetFirstChild<SheetData>();
            if (sheetData is null) return null;

            // 행 번호 파싱
            int i = 0;
            while (i < cellRef.Length && char.IsLetter(cellRef[i])) i++;
            if (!int.TryParse(cellRef[i..], out var rowIndex)) return null;

            var row = sheetData.Elements<Row>()
                .FirstOrDefault(r => r.RowIndex?.Value == (uint)rowIndex);

            if (row is null) return null;

            return row.Elements<Cell>()
                .FirstOrDefault(c => string.Equals(c.CellReference?.Value, cellRef, StringComparison.OrdinalIgnoreCase));
        }

        private static string Html(string? s) =>
            System.Net.WebUtility.HtmlEncode(s ?? string.Empty);
    }
}
