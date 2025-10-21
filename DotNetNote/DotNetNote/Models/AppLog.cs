namespace DotNetNote.Models
{
    // Serilog 표준 테이블 스키마 (Id 없음)
    public class AppLog
    {
        public string? Message { get; set; }
        public string? MessageTemplate { get; set; }
        public string? Level { get; set; }
        public DateTimeOffset? TimeStamp { get; set; }
        public string? Exception { get; set; }
        public string? Properties { get; set; }
    }
}
