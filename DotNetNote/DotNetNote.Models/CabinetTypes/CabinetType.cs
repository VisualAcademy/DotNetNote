namespace DotNetNote.Models
{
    public class CabinetType
    {
        public long Id { get; set; }
        public string? Identification { get; set; }
        public bool? Show { get; set; }
        public bool Adjusted { get; set; } = false;
    }
}
