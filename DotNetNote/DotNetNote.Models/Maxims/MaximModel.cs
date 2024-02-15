using System.ComponentModel.DataAnnotations;

namespace DotNetNote.Models;

public class MaximModel
{
    public int Id { get; set; }

    [Display(Name = "이름")]
    [Required(ErrorMessage = "이름을 입력하시오.")]
    [StringLength(25, MinimumLength = 1, ErrorMessage = "이름은 1자 이상 25자 이하")]
    public string Name { get; set; }

    [Display(Name = "내용")]
    [Required(ErrorMessage = "내용을 입력하시오.")]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "내용은 1자 이상 255자 이하")]
    public string Content { get; set; }
}
