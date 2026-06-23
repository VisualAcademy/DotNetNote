using System.ComponentModel.DataAnnotations;

namespace DotNetNote.Models.ManageViewModels;

public class IndexViewModel
{
    public string Username { get; set; } = string.Empty;

    public bool IsEmailConfirmed { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [Display(Name = "Phone number")]
    public string? PhoneNumber { get; set; }

    public string? StatusMessage { get; set; }
}