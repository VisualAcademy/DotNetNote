using DotNetNote.Models.Notes;
using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

[Route("api/[controller]")]
public class NoteCommentServiceController(INoteCommentRepository repository) : Controller
{
    // 최근 댓글 리스트 반환
    [HttpGet]
    public IEnumerable<NoteComment> Get() => repository.GetRecentComments();
}
