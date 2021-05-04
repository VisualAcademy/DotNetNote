using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers
{
    //[1] 모델 클래스
    public class BoardSummaryModel
    {
        public int Id { get; set; }
        public string Alias { get; set; } // Notice, Free, Photo, ...
        public string Title { get; set; }
        public string Name { get; set; }
        public DateTime PostDate { get; set; }
    }

    //[2] 리포지토리 클래스
    public class BoardSummaryRepository
    {
        public List<BoardSummaryModel> GetAll()
        {
            // 인 메모리 데이터베이스 => 실제 데이터베이스
            var lists = new List<BoardSummaryModel>() {
                new BoardSummaryModel {
                    Id = 1, Alias = "Notice", Name = "홍길동",
                    Title = "공지사항입니다.", PostDate = DateTime.Now },
                new BoardSummaryModel {
                    Id = 2, Alias = "Free", Name = "백두산",
                    Title = "자유게시판입니다.", PostDate = DateTime.Now },
                new BoardSummaryModel {
                    Id = 3, Alias = "Photo", Name = "임꺽정",
                    Title = "사진게시판입니다.", PostDate = DateTime.Now },
                new BoardSummaryModel {
                    Id = 4, Alias = "Notice", Name = "홍길동",
                    Title = "공지사항입니다.", PostDate = DateTime.Now }
            };

            return lists; 
        }

        public List<BoardSummaryModel> GetByAlias(string alias)
        {
            return GetAll().Where(b => b.Alias == alias).ToList(); 
        }
    }

    //[3] Web API
    [Produces("application/json")]
    [Consumes("application/json")]
    [Route("api/BoardSummaryApi")]
    public class BoardSummaryApiController : Controller
    {
        private BoardSummaryRepository _repository;

        public BoardSummaryApiController()
        {
            _repository = new BoardSummaryRepository();
        }

        [HttpGet]
        public IEnumerable<BoardSummaryModel> Get()
        {
            return _repository.GetAll();
        }

        [HttpGet("{alias}", Name = "GetByBoardSummaryModel")]
        public IEnumerable<BoardSummaryModel> Get(string alias)
        {
            return _repository.GetByAlias(alias);
        }
    }

    //[4] 컨트롤러
    public class BoardSummaryDemoController : Controller
    {
        public IActionResult Index()
        {
            string html = @"
<div id='print'></div>
<script src='https://code.jquery.com/jquery-2.2.4.min.js'></script>
<script>
$(function() {
    $.getJSON('/api/BoardSummaryApi', function(data) {
        var str = '<dl>';

        $.each(data, function (index, entry) {
            str += '<dt>' + entry.title + '</dt><dd>' + entry.name + '</dd>';
        });
        
        str += '</dl>';
        $('#print').html(str);
    });
});
</script>
";
            ContentResult cr = new ContentResult() {
                ContentType = "text/html", Content = html };

            return cr;
        }
    }
}
