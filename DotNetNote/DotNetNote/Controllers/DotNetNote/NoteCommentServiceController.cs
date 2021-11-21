﻿using DotNetNote.Models.Notes;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace DotNetNote.Controllers
{
    [Route("api/[controller]")]
    public class NoteCommentServiceController : Controller
    {
        private INoteCommentRepository _repository;

        public NoteCommentServiceController(INoteCommentRepository repository)
        {
            _repository = repository;
        }

        // 최근 댓글 리스트 반환
        [HttpGet]
        public IEnumerable<NoteComment> Get() => _repository.GetRecentComments();
    }
}
