using System.ComponentModel.DataAnnotations;

namespace DotNetNote.Models;

public class Applicant
{
    public int Id { get; set; }

    [Display(Name = "이름")]
    [Required(ErrorMessage = "이름을 입력하시오.")]
    [StringLength(25, MinimumLength = 1, ErrorMessage = "이름은 1자 이상 25자 이하")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "아버지 이름을 입력하시오.")]
    public string FatherName { get; set; } = string.Empty;

    public string? FatherDob { get; set; }

    [Required(ErrorMessage = "아버지 회원 여부를 선택하시오.")]
    public string IsFatherMember { get; set; } = string.Empty;

    [Required(ErrorMessage = "어머니 이름을 입력하시오.")]
    public string MotherName { get; set; } = string.Empty;

    public string? MotherDob { get; set; }
}