namespace DotNetNote.Models.RecruitManager;

/// <summary>
/// RecruitRegistration 모델: RecruitRegistrations 테이블과 일대일 클래스
/// </summary>
public class RecruitRegistration
{
    public int Id { get; set; }

    /// <summary>
    /// RecruitSettings 테이블의 Id 값
    /// </summary>
    public int RecruitSettingId { get; set; }

    [Display(Name = "게시판 이름")]
    [Required(ErrorMessage = "게시판 이름을 입력하세요.")]
    public string BoardName { get; set; }

    [Display(Name = "게시판 번호")]
    [Required(ErrorMessage = "게시판 번호를 입력하세요.")]
    [Range(0, int.MaxValue, ErrorMessage = "숫자값을 입력하세요.")]
    public int BoardNum { get; set; }

    [Display(Name = "게시물 제목")]
    public string BoardTitle { get; set; }

    [Display(Name = "등록일")]
    public DateTimeOffset CreationDate { get; set; }

    public int UserId { get; set; }

    [Required(ErrorMessage = "로그인 사용자 아이디를 입력하세요.")]
    public string Username { get; set; }

    public string NickName { get; set; }
}
