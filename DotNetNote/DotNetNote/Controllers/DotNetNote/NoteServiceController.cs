using DotNetNote.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace DotNetNote.Controllers
{
    [Route("api/[controller]")]
    public class NoteServiceController : Controller
    {
        private readonly INoteRepository _repository;

        public NoteServiceController(INoteRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IEnumerable<Note> Get()
        {
            // 최근 글 리스트 반환
            //return _repository.GetRecentPosts();      // 캐싱 적용 전
            return _repository.GetRecentPostsCache();   // 캐싱 적용 후
        }
    }
}
