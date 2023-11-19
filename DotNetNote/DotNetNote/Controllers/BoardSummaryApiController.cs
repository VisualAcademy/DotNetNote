using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers
{
    //[1] �� Ŭ����
    public class BoardSummaryModel
    {
        public int Id { get; set; }
        public string Alias { get; set; } // Notice, Free, Photo, ...
        public string Title { get; set; }
        public string Name { get; set; }
        public DateTime PostDate { get; set; }
    }

    //[2] �������丮 Ŭ����
    public class BoardSummaryRepository
    {
        public List<BoardSummaryModel> GetAll()
        {
            // �� �޸� �����ͺ��̽� => ���� �����ͺ��̽�
            var lists = new List<BoardSummaryModel>() {
                new BoardSummaryModel {
                    Id = 1, Alias = "Notice", Name = "ȫ�浿",
                    Title = "���������Դϴ�.", PostDate = DateTime.Now },
                new BoardSummaryModel {
                    Id = 2, Alias = "Free", Name = "��λ�",
                    Title = "�����Խ����Դϴ�.", PostDate = DateTime.Now },
                new BoardSummaryModel {
                    Id = 3, Alias = "Photo", Name = "�Ӳ���",
                    Title = "�����Խ����Դϴ�.", PostDate = DateTime.Now },
                new BoardSummaryModel {
                    Id = 4, Alias = "Notice", Name = "ȫ�浿",
                    Title = "���������Դϴ�.", PostDate = DateTime.Now }
            };

            return lists; 
        }

        public List<BoardSummaryModel> GetByAlias(string alias) => GetAll().Where(b => b.Alias == alias).ToList();
    }

    //[3] Web API
    [Produces("application/json")]
    [Consumes("application/json")]
    [Route("api/BoardSummaryApi")]
    public class BoardSummaryApiController : Controller
    {
        private BoardSummaryRepository _repository;

        public BoardSummaryApiController() => _repository = new BoardSummaryRepository();

        [HttpGet]
        public IEnumerable<BoardSummaryModel> Get() => _repository.GetAll();

        [HttpGet("{alias}", Name = "GetByBoardSummaryModel")]
        public IEnumerable<BoardSummaryModel> Get(string alias)
        {
            return _repository.GetByAlias(alias);
        }
    }

    //[4] ��Ʈ�ѷ�
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
