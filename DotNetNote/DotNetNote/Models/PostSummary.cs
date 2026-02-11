namespace DotNetNote.Models
{
    public class PostSummary
    {
        public string BlogName { get; set; } = string.Empty;
        public string PostTitle { get; set; } = string.Empty;
        public DateOnly PublishedOn { get; set; }
    }
}
