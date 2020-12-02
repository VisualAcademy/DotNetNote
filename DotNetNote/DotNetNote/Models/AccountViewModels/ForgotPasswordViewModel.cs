using System.ComponentModel.DataAnnotations;

namespace DotNetNote.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
