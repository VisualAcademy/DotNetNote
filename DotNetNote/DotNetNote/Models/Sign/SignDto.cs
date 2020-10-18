namespace DotNetNote.Models
{
    /// <summary>
    /// SignDto: Data Transfer Object
    /// </summary>
    public class SignDto
    {
        public int SignId { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }
    }
}
