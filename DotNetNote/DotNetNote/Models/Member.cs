namespace DotNetNote.Models
{
    public class Member
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Gender { get; set; }
    }
}