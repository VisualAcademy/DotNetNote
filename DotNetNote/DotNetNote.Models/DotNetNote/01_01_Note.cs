using System;
using System.ComponentModel.DataAnnotations;

namespace DotNetNote.Models
{
    /// <summary>
    /// [1][1] Note 클래스: Notes 테이블과 일대일 매핑되는 ViewModel 클래스
    /// </summary>
    public class Note
    {
        [Display(Name = "번호")]
        public int Id { get; set; }

        [Display(Name = "작성자")]
        [Required(ErrorMessage = "* 이름을 작성해 주세요.")]
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "* 이메일을 정확히 입력하세요.")]
        public string Email { get; set; }

        [Display(Name = "제목")]
        [Required(ErrorMessage = "* 제목을 작성해 주세요.")]
        public string Title { get; set; }

        [Display(Name = "작성일")]
        public DateTime PostDate { get; set; }

        [Display(Name = "작성IP")]
        public string PostIp { get; set; }

        [Display(Name = "내용")]
        [Required(ErrorMessage = "* 내용을 작성해 주세요.")]
        public string Content { get; set; }

        [Display(Name = "비밀번호")]
        [Required(ErrorMessage = "* 비밀번호를 작성해 주세요.")]
        public string Password { get; set; }

        [Display(Name = "조회수")]
        public int ReadCount { get; set; }

        [Display(Name = "인코딩")]
        public string Encoding { get; set; } = "Text";

        [Display(Name = "홈페이지")]
        public string Homepage { get; set; }

        [Display(Name = "수정일")]
        public DateTime? ModifyDate { get; set; }

        [Display(Name = "수정IP")]
        public string ModifyIp { get; set; }

        [Display(Name = "파일")]
        public string FileName { get; set; }

        [Display(Name = "파일크기")]
        public int FileSize { get; set; }

        [Display(Name = "다운수")]
        public int DownCount { get; set; }

        [Display(Name = "참조번호")]
        public int Ref { get; set; }

        [Display(Name = "들여쓰기")]
        public int Step { get; set; }

        [Display(Name = "참조순서")]
        public int RefOrder { get; set; }

        [Display(Name = "답변수")]
        public int AnswerNum { get; set; }

        [Display(Name = "부모번호")]
        public int ParentNum { get; set; }

        [Display(Name = "댓글수")]
        public int CommentCount { get; set; }

        [Display(Name = "카테고리")]
        public string Category { get; set; } // = "Free"; // 자유게시판(Free) 기본

        public int? Num { get; set; }
        public int? UserId { get; set; }
        public int? CategoryId { get; set; }
        public int? BoardId { get; set; }
        public int? ApplicationId { get; set; }
    }
}
