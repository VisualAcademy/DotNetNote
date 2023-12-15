#nullable enable
namespace Acts.Models
{
    public partial class ActionCategory
    {
        public long Id { get; set; }
        public bool GbAdjusted { get; set; }
        public string? Category { get; set; }
        public bool? Active { get; set; }
    }
}
