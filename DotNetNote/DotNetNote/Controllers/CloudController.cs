using DotNetNote.Models.Notes;
using DotNetNote.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DotNetNote.Controllers
{
    public class CloudController : Controller
    {
        // 강력한 형식의 클래스의 인스턴스 생성
        private DotNetNoteSettings _dnnSettings;
        private INoteRepository _repository; // 게시판 
        private INoteCommentRepository _commentRepo; // 댓글

        public CloudController(
            IOptions<DotNetNoteSettings> options,
            INoteRepository repository,
            INoteCommentRepository commentRepo)
        {
            _dnnSettings = options.Value; // Value 속성으로 인스턴스화된 개체 반환
            _repository = repository;
            _commentRepo = commentRepo;
        }

        public IActionResult Index()
        {
            // ViewData[] 또는 ViewBag. 개체로 뷰 페이지로 값 전송 
            ViewBag.SiteName = _dnnSettings.SiteName;
            ViewBag.SiteUrl = _dnnSettings.SiteUrl;


            //메인 페이지에 최근 올라온 사진 리스트 전송
            //[a] 샘플 데이터 인메모리 테스트
            //List<Note> photos = new List<Models.Note>() {
            //  new Note() { Id = 1, Title = "사진 제목 1", FileName="구글.png" },
            //  new Note() { Id = 2, Title = "사진 제목 2", FileName="구글.png" },
            //  new Note() { Id = 3, Title = "사진 제목 3", FileName="구글.png" },
            //};
            //ViewData["Photos"] = photos;            
            //[b] 실제 데이터베이스의 데이터 전송
            ViewData["Photos"] = _repository.GetNewPhotos();


            ViewData["Notice"] =
                _repository.GetNoteSummaryByCategory("Notice"); // 공지사항
            ViewData["Free"] =
                _repository.GetNoteSummaryByCategory("Free"); // 자유게시판
            ViewData["Data"] =
                _repository.GetNoteSummaryByCategory("Data"); // 자료실
            ViewData["Qna"] =
                _repository.GetNoteSummaryByCategory("Qna"); // Q&A

            ViewData["RecentPost"] =
                _repository.GetRecentPosts(); // 최근 글 리스트
            ViewData["RecentComment"] =
                _commentRepo.GetRecentComments(); // 최근 댓글 리스트

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "안녕하세요. 블로그입니다.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "안녕하세요. 블로그입니다.";

            return View();
        }

        public IActionResult Angular()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
