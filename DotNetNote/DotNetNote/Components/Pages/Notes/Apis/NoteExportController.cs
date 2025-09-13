using Microsoft.AspNetCore.Mvc;
using Azunt.NoteManagement;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Azunt.Apis.Notes
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoteExportController : ControllerBase
    {
        private readonly INoteRepository _noteRepository;

        public NoteExportController(INoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        /// <summary>
        /// 게시글 목록 엑셀 다운로드
        /// GET /api/NoteExport/Excel
        /// </summary>
        [HttpGet("Excel")]
        public async Task<IActionResult> ExportToExcel()
        {
            var items = await _noteRepository.GetAllAsync();
            if (items == null || !items.Any())
                return NotFound("No note records found.");

            using var ms = new MemoryStream();
            using (var doc = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook, true))
            {
                // Workbook / Worksheet
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
                    Name = "Notes"
                });

                // ----- 헤더: A1~F1 -----
                uint headerRowIndex = 1;
                var headerRow = new Row { RowIndex = headerRowIndex };
                sheetData.Append(headerRow);

                string[] headers = { "Id", "Name", "Title", "Category", "Created", "CreatedBy" };
                for (int i = 0; i < headers.Length; i++)
                {
                    headerRow.Append(TextCell(Ref(i + 1, (int)headerRowIndex), headers[i]));
                }

                // ----- 데이터: A2부터 -----
                uint rowIndex = 2;
                foreach (var m in items)
                {
                    var row = new Row { RowIndex = rowIndex };
                    sheetData.Append(row);

                    var values = new[]
                    {
                        m.Id.ToString(),
                        m.Name ?? string.Empty,
                        m.Title ?? string.Empty,
                        m.Category ?? string.Empty,
                        m.Created.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"),
                        m.CreatedBy ?? string.Empty
                    };

                    for (int i = 0; i < values.Length; i++)
                    {
                        row.Append(TextCell(Ref(i + 1, (int)rowIndex), values[i]));
                    }

                    rowIndex++;
                }

                wsPart.Worksheet.Save();
                wbPart.Workbook.Save();
            }

            return File(
                ms.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"{DateTime.Now:yyyyMMddHHmmss}_Notes.xlsx");
        }

        // ===== Helpers (최소) =====
        private static Cell TextCell(string cellRef, string text) =>
            new Cell
            {
                CellReference = cellRef,
                DataType = CellValues.String,
                CellValue = new CellValue(text ?? string.Empty)
            };

        private static string Ref(int col1Based, int row) => $"{ColName(col1Based)}{row}";

        private static string ColName(int index)
        {
            var dividend = index; // 1=A
            string col = string.Empty;
            while (dividend > 0)
            {
                var modulo = (dividend - 1) % 26;
                col = (char)('A' + modulo) + col;
                dividend = (dividend - modulo) / 26;
            }
            return col;
        }
    }
}
