using DotNetNote.Models.Notes;
using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

[Route("api/[controller]")]
public class NoteServiceController(INoteRepository repository) : Controller
{
    [HttpGet]
    public IEnumerable<Note> Get() =>
        // 최근 글 리스트 반환
        //return _repository.GetRecentPosts();      // 캐싱 적용 전
        repository.GetRecentPostsCache();   // 캐싱 적용 후
}
