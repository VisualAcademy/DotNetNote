using System.Collections.Generic;

namespace DotNetNote.Models
{
    /// <summary>
    /// 특정 Id에 해당하는 게시물에 대한 댓글 리스트와 게시판Id를 묶어서 전송
    /// </summary>
    public class NoteCommentViewModel
    {
        public List<NoteComment> NoteCommentList { get; set; }
        public int BoardId { get; set; }
    }
}
