using System.ComponentModel.DataAnnotations;

namespace DotNetNote.Models.RecruitManager;

/// <summary>
/// RecruitSetting 모델: RecruitSettings 테이블과 일대일 모델 클래스
/// </summary>
public class RecruitSetting
{
    public int Id { get; set; }

    [Display(Name = "비고")]
    public string Remarks { get; set; }

    public DateTimeOffset CreationDate { get; set; }

    [Display(Name = "게시판 이름")]
    [Required(ErrorMessage = "게시판 이름을 입력하세요.")]
    public string BoardName { get; set; }

    [Display(Name = "게시판 번호")]
    [Required(ErrorMessage = "게시판 번호를 입력하세요.")]
    [Range(0, int.MaxValue, ErrorMessage = "숫자값을 입력하세요.")]
    public int BoardNum { get; set; }

    [Display(Name = "게시물 제목")]
    public string BoardTitle { get; set; }

    [Display(Name = "게시물 내용")]
    public string BoardContent { get; set; }

    [Display(Name = "이벤트 표시 시작일")]
    public DateTime StartDate { get; set; }

    [Display(Name = "이벤트 등록 시작일")]
    public DateTime EventDate { get; set; }

    [Display(Name = "이벤트 표시 종료일")]
    public DateTime EndDate { get; set; }

    [Display(Name = "선착순 최대 등록자")]
    public int MaxCount { get; set; }
}
