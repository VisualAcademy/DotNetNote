using System.ComponentModel.DataAnnotations;

namespace DotNetNote.Models;

public class CommunityCampJoinMember
{
    public int Id { get; set; }

    [Display(Name="커뮤니티")]
    public string CommunityName { get; set; }

    [Display(Name="이름")]
    [Required(ErrorMessage ="이름을 입력하시오.")]
    [StringLength(25, MinimumLength = 1, ErrorMessage = "이름을 확인하세요.")]
    public string Name { get; set; }
    
    [Display(Name="연락처")]
    [Required(ErrorMessage ="연락처를 입력하시오.")]
    public string Mobile { get; set; }
    
    [Required(ErrorMessage ="이메일을 입력하시오.")]
    public string Email { get; set; }
    
    [Display(Name="티셔츠 사이즈")]
    public string Size { get; set; }
    
    public DateTime CreationDate { get; set; }
}
