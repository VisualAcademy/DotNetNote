namespace DotNetNote.Models
{
    /// <summary>
    /// SignDto: Data Transfer Object
    /// </summary>
    public class SignDto
    {
        public int SignId { get; set; }

        public string Token { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
    }
}