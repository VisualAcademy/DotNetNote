using System.ComponentModel.DataAnnotations;

namespace DotNetNote.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
