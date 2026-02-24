using System.ComponentModel.DataAnnotations;

namespace DotNetNote;

public class DotNetNoteUserRegisterModel
{
    [Required]
    public string? UserName { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Required]
    [Compare("Password")]
    [DataType(DataType.Password)]
    public string? ConfirmPassword { get; set; }
}