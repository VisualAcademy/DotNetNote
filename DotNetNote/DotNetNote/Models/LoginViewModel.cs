using System.ComponentModel.DataAnnotations;

namespace DotNetNote.Models
{
    public class LoginViewModel
    {
        [Required, EmailAddress]
        public required string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public required string Password { get; set; } = string.Empty;
    }
}
