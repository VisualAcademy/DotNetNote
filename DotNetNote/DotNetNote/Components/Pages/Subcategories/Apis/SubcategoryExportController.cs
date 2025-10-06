using Azunt.SubcategoryManagement;
// Open XML SDK
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SS = DocumentFormat.OpenXml.Spreadsheet;

namespace Azunt.Apis.Subcategories
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrators")]
    public class SubcategoryExportController : ControllerBase
    {
        private readonly ISubcategoryRepository _subcategoryRepository;

        public SubcategoryExportController(ISubcategoryRepository subcategoryRepository)
        {
            _subcategoryRepository = subcategoryRepository;
        }

        /// <summary>
        /// Subcategory 목록 엑셀 다운로드 (Open XML SDK)
        /// GET /api/SubcategoryExport/Excel
        /// </summary>
        [HttpGet("Excel")]
        public async Task<IActionResult> ExportToExcel()
        {
            var items = await _subcategoryRepository.GetAllAsync();
            if (items is null || !items.Any())
                return NotFound("No subcategory records found.");

            using var stream = new MemoryStream();

            using (var doc = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook, true))
            {
                // Workbook
                var wbPart = doc.AddWorkbookPart();
                wbPart.Workbook = new Workbook();

                // Styles
                var styles = wbPart.AddNewPart<WorkbookStylesPart>();
                styles.Stylesheet = BuildStylesheet();
                styles.Stylesheet.Save();

                // Worksheet
                var wsPart = wbPart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();

                // Column widths (B~G) — SS.Column으로 모호성 제거
                var columns = new Columns(
                    new SS.Column { Min = 2, Max = 2, Width = 14, CustomWidth = true }, // B: Id
                    new SS.Column { Min = 3, Max = 3, Width = 24, CustomWidth = true }, // C: Name
                    new SS.Column { Min = 4, Max = 4, Width = 30, CustomWidth = true }, // D: Title
                    new SS.Column { Min = 5, Max = 5, Width = 24, CustomWidth = true }, // E: Category
                    new SS.Column { Min = 6, Max = 6, Width = 22, CustomWidth = true }, // F: Created
                    new SS.Column { Min = 7, Max = 7, Width = 22, CustomWidth = true }  // G: CreatedBy
                );

                var ws = new Worksheet();
                ws.Append(columns);
                ws.Append(sheetData);

                // Start at B2
                const int startRow = 2;
                const int startCol = 2; // B

                // Header
                var headerRow = new Row { RowIndex = (uint)startRow };
                var headers = new[] { "Id", "Name", "Title", "Category", "Created", "CreatedBy" };
                for (int i = 0; i < headers.Length; i++)
                {
                    headerRow.Append(CreateTextCell(ToRef(startCol + i, startRow), headers[i], styleIndex: 2)); // 2: Header
                }
                sheetData.Append(headerRow);

                // Data rows
                var currentRow = startRow + 1;
                foreach (var m in items)
                {
                    var row = new Row { RowIndex = (uint)currentRow };

                    // Id
                    row.Append(CreateTextCell(ToRef(startCol + 0, currentRow), m.Id.ToString()));

                    // Name, Title, Category
                    row.Append(CreateTextCell(ToRef(startCol + 1, currentRow), m.Name ?? string.Empty));
                    row.Append(CreateTextCell(ToRef(startCol + 2, currentRow), m.Title ?? string.Empty));
                    row.Append(CreateTextCell(ToRef(startCol + 3, currentRow), m.Category ?? string.Empty));

                    // Created: DateTimeOffset → Local → "yyyy-MM-dd HH:mm:ss"
                    var createdText = m.Created.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                    row.Append(CreateTextCell(ToRef(startCol + 4, currentRow), createdText));

                    // CreatedBy
                    row.Append(CreateTextCell(ToRef(startCol + 5, currentRow), m.CreatedBy ?? string.Empty));

                    sheetData.Append(row);
                    currentRow++;
                }

                wsPart.Worksheet = ws;
                wsPart.Worksheet.Save();

                // Sheets
                var sheets = new Sheets();
                sheets.Append(new Sheet
                {
                    Id = wbPart.GetIdOfPart(wsPart),
                    SheetId = 1U,
                    Name = "Subcategories"
                });
                wbPart.Workbook.Append(sheets);
                wbPart.Workbook.Save();
            }

            var bytes = stream.ToArray();
            var fileName = $"{DateTime.Now:yyyyMMddHHmmss}_Subcategories.xlsx";
            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }

        // =========================
        // Stylesheet
        // 0: 기본
        // 1: 본문(얇은 테두리)
        // 2: 헤더(흰 글꼴 + 진한 초록 배경 + 테두리)
        // =========================
        private static Stylesheet BuildStylesheet()
        {
            // Fonts (SS.Color로 모호성 제거)
            var fonts = new Fonts { Count = 2U };
            fonts.Append(new Font( // 0
                new FontSize { Val = 11 },
                new SS.Color { Theme = 1 },
                new FontName { Val = "Calibri" }
            ));
            fonts.Append(new Font( // 1
                new Bold(),
                new SS.Color { Rgb = "FFFFFFFF" },
                new FontSize { Val = 11 },
                new FontName { Val = "Calibri" }
            ));

            // Fills
            var fills = new Fills { Count = 3U };
            fills.Append(new Fill(new PatternFill { PatternType = PatternValues.None }));    // 0
            fills.Append(new Fill(new PatternFill { PatternType = PatternValues.Gray125 })); // 1
            fills.Append(new Fill( // 2: DarkGreen header
                new PatternFill(
                    new ForegroundColor { Rgb = "FF006400" },
                    new BackgroundColor { Indexed = 64U }
                )
                { PatternType = PatternValues.Solid }
            ));

            // Borders
            var borders = new Borders { Count = 2U };
            borders.Append(new Border()); // 0: none
            borders.Append(new Border(    // 1: thin
                new LeftBorder { Style = BorderStyleValues.Thin },
                new RightBorder { Style = BorderStyleValues.Thin },
                new TopBorder { Style = BorderStyleValues.Thin },
                new BottomBorder { Style = BorderStyleValues.Thin },
                new DiagonalBorder()
            ));

            // CellFormats
            var cfs = new CellFormats { Count = 3U };

            // 0: 기본
            cfs.Append(new CellFormat());

            // 1: 본문(테두리)
            cfs.Append(new CellFormat
            {
                BorderId = 1U,
                ApplyBorder = true
            });

            // 2: 헤더(흰 글꼴 + 초록 배경 + 테두리)
            cfs.Append(new CellFormat
            {
                FontId = 1U,
                FillId = 2U,
                BorderId = 1U,
                ApplyFont = true,
                ApplyFill = true,
                ApplyBorder = true,
                Alignment = new Alignment { Horizontal = HorizontalAlignmentValues.Left, Vertical = VerticalAlignmentValues.Center }
            });

            return new Stylesheet(fonts, fills, borders, cfs);
        }

        // =========================
        // Cell helpers
        // =========================
        private static Cell CreateTextCell(string cellRef, string text, uint styleIndex = 1) =>
            new Cell
            {
                CellReference = cellRef,
                DataType = CellValues.InlineString,
                StyleIndex = styleIndex,
                InlineString = new InlineString(
                    new SS.Text(text ?? string.Empty)
                )
            };

        // =========================
        // Address helpers
        // =========================
        private static string ToRef(int colIndex, int rowIndex) => $"{ToColName(colIndex)}{rowIndex}";

        private static string ToColName(int index)
        {
            var dividend = index;
            var columnName = string.Empty;
            while (dividend > 0)
            {
                var modulo = (dividend - 1) % 26;
                columnName = (char)('A' + modulo) + columnName;
                dividend = (dividend - modulo) / 26;
            }
            return columnName;
        }
    }
}
