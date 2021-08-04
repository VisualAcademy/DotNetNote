using System;
using System.ComponentModel.DataAnnotations;

namespace DotNetNote.Models
{
    /// <summary>
    /// [2][1] 댓글 뷰 모델 클래스
    /// NoteComment 클래스: NoteComments 테이블과 일대일 매핑되는 ViewModel 클래스
    /// </summary>
    public class NoteComment
    {
        public int Id { get; set; }

        public string BoardName { get; set; }

        public int BoardId { get; set; }

        [Required(ErrorMessage = "이름을 입력하세요.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "의견을 입력하세요.")]
        public string Opinion { get; set; }

        public DateTime PostDate { get; set; }
        [Required(ErrorMessage = "암호를 입력하세요.")]

        public string Password { get; set; }
    }
}
