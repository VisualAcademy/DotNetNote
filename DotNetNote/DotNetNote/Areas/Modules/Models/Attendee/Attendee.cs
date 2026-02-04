namespace AttendeeApp.Models;

/// <summary>
/// Attendees 테이블과 일대일인 Attendee 클래스
/// </summary>
public class Attendee
{
    public int Id { get; set; }

    [Display(Name = "UID")]
    [Required(ErrorMessage = "UID를 입력하시오.")]
    public int UID { get; set; }

    [Required(ErrorMessage = "아이디를 입력하시오.")]
    [StringLength(100, MinimumLength = 1,
        ErrorMessage = "아이디를 확인하세요.")]
    public string UserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "이름을 입력하시오.")]
    [StringLength(25, MinimumLength = 1,
        ErrorMessage = "이름을 확인하세요.")]
    public string Name { get; set; } = string.Empty;

    public DateTime CreationDate { get; set; } = DateTime.UtcNow;
}
