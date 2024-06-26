﻿using DotNetNote.Models.Notes;
using System.Diagnostics;

namespace DotNetNote.Controllers;

public class HomeController : Controller
{
    // 강력한 형식의 클래스의 인스턴스 생성
    private DotNetNoteSettings _dnnSettings;
    private readonly INoteRepository _repository; // 게시판 
    private readonly INoteCommentRepository _commentRepo; // 댓글
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        IOptions<DotNetNoteSettings> options,
        INoteRepository repository,
        INoteCommentRepository commentRepo,
        ILogger<HomeController> logger
        )
    {
        _dnnSettings = options.Value; // Value 속성으로 인스턴스화된 개체 반환
        _repository = repository;
        _commentRepo = commentRepo;
        _logger = logger;
    }

    public IActionResult Index()
    {
        //_logger.LogInformation("HOME - Index 페이지가 로드되었습니다.");

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
        //ViewData["Photos"] = _repository.GetNewPhotos(); // 캐싱 사용 전
        ViewData["Photos"] = _repository.GetNewPhotosCache(); // 캐싱 사용 후

        //ViewData["Notice"] =
        //    _repository.GetNoteSummaryByCategory("Notice"); // 공지사항
        //ViewData["Free"] =
        //    _repository.GetNoteSummaryByCategory("Free"); // 자유게시판
        //ViewData["Data"] =
        //    _repository.GetNoteSummaryByCategory("Data"); // 자료실
        //ViewData["Qna"] =
        //    _repository.GetNoteSummaryByCategory("Qna"); // Q&A
        ViewData["Notice"] =
            _repository.GetNoteSummaryByCategoryCache("Notice"); // 공지사항
        ViewData["Free"] =
            _repository.GetNoteSummaryByCategoryCache("Free"); // 자유게시판
        ViewData["Data"] =
            _repository.GetNoteSummaryByCategoryCache("Data"); // 자료실
        ViewData["Qna"] =
            _repository.GetNoteSummaryByCategoryCache("Qna"); // Q&A

        ViewData["RecentPost"] =
            //_repository.GetRecentPosts(); // 최근 글 리스트
            _repository.GetRecentPostsCache(); // 최근 글 리스트

        try
        {
            ViewData["RecentComment"] =
                _commentRepo.GetRecentComments(); // 최근 댓글 리스트
        }
        catch (System.Exception ex)
        {
            _logger.LogError($"최근 댓글 리스트 가져오기 에러: {ex.Message}");
        }

        return View();
    }

    public IActionResult Privacy() => View();

    public IActionResult About()
    {
        //_logger.LogInformation("HOME - About 페이지가 로드되었습니다.");

        ViewData["Message"] = "안녕하세요. DotNetNote 사이트입니다.";

        return View();
    }

    public IActionResult Contact()
    {
        //_logger.LogInformation("HOME - Contact 페이지가 로드되었습니다.");

        ViewData["Message"] = "안녕하세요. DotNetNote 사이트입니다.";

        return View();
    }

    public IActionResult Angular() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    // VisualAcademy 섹션 강의: ASP.NET Core 2.X 버전으로 진행
    public IActionResult IndexVisualAcademy() => View();
}
