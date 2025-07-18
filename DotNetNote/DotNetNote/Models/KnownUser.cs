namespace DotNetNote.Models
{
    public class KnownUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = default!;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
