using Dul.Board;
using Dul.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DotNetNote.Controllers
{
    public class DotNetNoteController : Controller
    {
        //[DNN] 의존성 주입
        private IWebHostEnvironment _environment; // 환경 변수
        private readonly INoteRepository _repository; // 게시판 리포지토리
        private readonly INoteCommentRepository _commentRepository; // 댓글 리포지토리
        private readonly ILogger<DotNetNoteController> _logger; // 기본 제공 로깅
        private readonly DotNetNoteContext _context; // 컨텍스트 클래스

        public DotNetNoteController(
            IWebHostEnvironment environment, // IWebHostEnvironment로 변경할 것
            INoteRepository repository,
            INoteCommentRepository commentRepository,
            ILogger<DotNetNoteController> logger,
            DotNetNoteContext context
            )
        {
            _environment = environment;
            _repository = repository;
            _commentRepository = commentRepository;
            _logger = logger;
            _context = context;
        }

        // 공통 속성: 검색 모드: 검색 모드이면 true, 그렇지 않으면 false.
        public bool SearchMode { get; set; } = false;
        public string SearchField { get; set; } // 필드: Name, Title, Content
        public string SearchQuery { get; set; } // 검색 내용

        /// <summary>
        /// 현재 보여줄 페이지 번호
        /// </summary>
        public int PageIndex { get; set; } = 0;

        /// <summary>
        /// 총 레코드 갯수(글번호 순서 정렬용)
        /// </summary>
        public int TotalRecordCount { get; set; } = 0;

        #region Index: 게시판 리스트 페이지
        /// <summary>
        /// 게시판 리스트 페이지
        /// </summary>
        public IActionResult Index()
        {
            // 로깅
            _logger.LogInformation("게시판 리스트 페이지 로딩");

            // 검색 모드 결정: ?SearchField=Name&SearchQuery=닷넷코리아 
            SearchMode = (
                !string.IsNullOrEmpty(Request.Query["SearchField"]) &&
                !string.IsNullOrEmpty(Request.Query["SearchQuery"])
            );

            // 검색 환경이면 속성에 저장
            if (SearchMode)
            {
                SearchField = Request.Query["SearchField"].ToString();
                SearchQuery = Request.Query["SearchQuery"].ToString();
            }

            //[1] 쿼리스트링에 따른 페이지 보여주기
            if (!string.IsNullOrEmpty(Request.Query["Page"].ToString()))
            {
                // Page는 보여지는 쪽은 1, 2, 3, ... 코드단에서는 0, 1, 2, ...
                PageIndex = Convert.ToInt32(Request.Query["Page"]) - 1;
            }

            //[2] 쿠키를 사용한 리스트 페이지 번호 유지 적용(Optional): 
            //    100번째 페이지 보고 있다가 다시 리스트 왔을 때 100번째 페이지 표시
            if (!string.IsNullOrEmpty(Request.Cookies["DotNetNotePageNum"]))
            {
                if (!String.IsNullOrEmpty(
                    Request.Cookies["DotNetNotePageNum"]))
                {
                    PageIndex =
                        Convert.ToInt32(Request.Cookies["DotNetNotePageNum"]);
                }
                else
                {
                    PageIndex = 0;
                }
            }

            // 게시판 리스트 정보 가져오기
            //IEnumerable<Note> notes;
            List<Note> notes = new List<Note>();
            if (!SearchMode)
            {
                TotalRecordCount = _repository.GetCountAll();
                notes = _repository.GetAll(PageIndex);
            }
            else
            {
                TotalRecordCount = _repository.GetCountBySearch(
                    SearchField, SearchQuery);
                notes = _repository.GetSeachAll(
                    PageIndex, SearchField, SearchQuery);
            }

            // 주요 정보를 뷰 페이지로 전송
            ViewBag.TotalRecord = TotalRecordCount;
            ViewBag.SearchMode = SearchMode;
            ViewBag.SearchField = SearchField;
            ViewBag.SearchQuery = SearchQuery;

            // 페이저 컨트롤 적용
            ViewBag.PageModel = new PagerBase
            {
                Url = "DotNetNote/Index",
                RecordCount = TotalRecordCount,
                PageSize = 10,
                PageNumber = PageIndex + 1,

                SearchMode = SearchMode,
                SearchField = SearchField,
                SearchQuery = SearchQuery
            };

            return View(notes);
        }
        #endregion

        #region Create: 게시판 글쓰기 페이지 
        /// <summary>
        /// 게시판 글쓰기 폼
        /// </summary>
        [HttpGet]
        [Authorize] // 스팸 글 때문에 추가
        public IActionResult Create()
        {
            // 로깅
            _logger.LogInformation("게시판 글쓰기 페이지 로딩");

            // 글쓰기 폼은 입력, 수정, 답변에서 _BoardEditorForm.cshtml 공유함
            ViewBag.FormType = BoardWriteFormType.Write;
            ViewBag.TitleDescription = "글 쓰기 - 다음 필드들을 채워주세요.";
            ViewBag.SaveButtonText = "저장";

            return View();
        }

        /// <summary>
        /// 게시판 글쓰기 처리 + 파일 업로드 처리
        /// </summary>
        [HttpPost]
        [Authorize] // 스팸 글 때문에 추가
        public async Task<IActionResult> Create(
            Note model, ICollection<IFormFile> files)
        {
            //파일 업로드 처리 시작
            string fileName = String.Empty;
            int fileSize = 0;

            var uploadFolder = Path.Combine(_environment.WebRootPath, "files");

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    fileSize = Convert.ToInt32(file.Length);
                    // 파일명 중복 처리
                    fileName = Dul.FileUtility.GetFileNameWithNumbering(
                        uploadFolder, Path.GetFileName(
                            ContentDispositionHeaderValue.Parse(
                                file.ContentDisposition).FileName.Trim('"')));
                    // 파일 업로드
                    using (var fileStream = new FileStream(
                        Path.Combine(uploadFolder, fileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
            }

            Note note = new Note();

            note.Name = model.Name;
            note.Email = Dul.HtmlUtility.Encode(model.Email);
            note.Homepage = model.Homepage;
            //note.Title    = Dul.HtmlUtility.Encode(model.Title);
            note.Title = model.Title;
            note.Content = model.Content;
            note.FileName = fileName;
            note.FileSize = fileSize;
            note.Password = (new Dul.Security.CryptorEngine())
                .EncryptPassword(model.Password);
            note.PostIp =
                HttpContext.Connection.RemoteIpAddress.ToString(); // IP 주소
            note.Encoding = model.Encoding;

            _repository.Add(note); // 데이터 저장

            // 데이터 저장 후 리스트 페이지 이동시 toastr로 메시지 출력
            TempData["Message"] = "데이터가 저장되었습니다.";

            return RedirectToAction("Index"); // 저장 후 리스트 페이지로 이동
        }
        #endregion

        /// <summary>
        /// 게시판 파일 강제 다운로드 기능(/BoardDown/:Id)
        /// </summary>
        public FileResult BoardDown(int id)
        {
            string fileName = "";

            // 넘겨져 온 번호에 해당하는 파일명 가져오기(보안때문에... 파일명 숨김)
            fileName = _repository.GetFileNameById(id);

            if (fileName == null)
            {
                return null;
            }
            else
            {
                // 다운로드 카운트 증가 메서드 호출
                _repository.UpdateDownCountById(id);

                if (System.IO.File.Exists(Path.Combine(_environment.WebRootPath, "files") + "\\" + fileName))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(Path.Combine(_environment.WebRootPath, "files") + "\\" + fileName);

                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }

                return null;
            }
        }

        #region Details - 게시판의 상세 보기 페이지(Details, BoardView)
        /// <summary>
        /// 게시판의 상세 보기 페이지(Details, BoardView)
        /// </summary>
        public IActionResult Details(int id)
        {
            // 넘겨온 Id 값에 해당하는 레코드 하나 읽어서 Note 클래스에 바인딩
            var note = _repository.GetNoteById(id);

            //[!] 인코딩 방식에 따른 데이터 출력: 
            // 직접 문자열 비교해도 되지만, 학습목적으로 열거형으로 비교 
            ContentEncodingType encoding = ContentEncodingType.Text;
            try
            {
                encoding = (ContentEncodingType)Enum.Parse(typeof(ContentEncodingType), note.Encoding);
            }
            catch (Exception)
            {
                encoding = ContentEncodingType.Text;
            }

            string encodedContent = "";
            switch (encoding)
            {
                // Text : 소스 그대로 표현: text/plain, Plain-Text
                case ContentEncodingType.Text:
                    encodedContent = Dul.HtmlUtility.EncodeWithTabAndSpace(note.Content);
                    break;
                // Html : HTML 형식으로 출력: Text/HTML
                case ContentEncodingType.Html:
                    encodedContent = note.Content; // 변환없음
                    break;
                // Mixed : 엔터처리만: Mixed-Text
                case ContentEncodingType.Mixed:
                    encodedContent = note.Content.Replace("\r\n", "<br />");
                    break;
                // Html : 기본
                default:
                    encodedContent = note.Content; // 변환없음
                    break;
            }
            ViewBag.Content = encodedContent; //[!]

            // 첨부된 파일 확인
            if (note.FileName.Length > 1)
            {
                //[a] 파일 다운로드 링크: String.Format()으로 표현해 봄 
                ViewBag.FileName = String.Format("<a href='/DotNetNote/BoardDown?Id={0}'>" + "{1}{2} / 전송수: {3}</a>", note.Id, "<img src=\"/images/ext/ext_zip.gif\" border=\"0\">", note.FileName, note.DownCount);
                //[b] 이미지 미리보기: C# 6.0 String 보간법으로 표현해 봄
                if (Dul.BoardLibrary.IsPhoto(note.FileName))
                {
                    ViewBag.ImageDown = $"<img src=\'/DotNetNote/ImageDown/{note.Id}\'><br />";
                }
            }
            else
            {
                ViewBag.FileName = "(업로드된 파일이 없습니다.)";
            }

            // 현재 글에 해당하는 댓글 리스트와 현재 글 번호를 담아서 전달
            NoteCommentViewModel vm = new NoteCommentViewModel();
            vm.NoteCommentList = _commentRepository.GetNoteComments(note.Id);
            vm.BoardId = note.Id;
            ViewBag.CommentListAndId = vm;

            return View(note);
        }
        #endregion

        /// <summary>
        /// 게시판 삭제 폼
        /// </summary>
        [HttpGet]
        public IActionResult Delete(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        #region 게시판 삭제 처리
        /// <summary>
        /// 게시판 삭제 처리
        /// </summary>
        [HttpPost]
        public IActionResult Delete(int id, string password)
        {
            //if (_repository.DeleteNote(id, Password) > 0)
            if (_repository.DeleteNote(id,
                (new Dul.Security.CryptorEngine()).EncryptPassword(password)) > 0)
            {
                TempData["Message"] = "데이터가 삭제되었습니다.";

                // 학습 목적으로 삭제 후의 이동 페이지를 2군데 중 하나로 분기
                if (DateTime.Now.Second % 2 == 0)
                {
                    //[a] 삭제 후 특정 뷰 페이지로 이동
                    return RedirectToAction("DeleteCompleted");
                }
                else
                {
                    //[b] 삭제 후 Index 페이지로 이동
                    return RedirectToAction("Index");
                }
            }
            else
            {
                ViewBag.Message = "삭제되지 않았습니다. 비밀번호를 확인하세요.";
            }

            ViewBag.Id = id;
            return View();
        }
        #endregion

        /// <summary>
        /// 게시판 삭제 완료 후 추가적인 처리할 때 페이지
        /// </summary>
        public IActionResult DeleteCompleted() => View();

        #region 수정
        /// <summary>
        /// 게시판 수정 폼
        /// </summary>
        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.FormType = BoardWriteFormType.Modify;
            ViewBag.TitleDescription = "글 수정 - 아래 항목을 수정하세요.";
            ViewBag.SaveButtonText = "수정";

            // 기존 데이터를 바인딩
            var note = _repository.GetNoteById(id);

            // 첨부된 파일명 및 파일크기 기록
            if (note.FileName.Length > 0)
            {
                ViewBag.FileName = note.FileName;
                ViewBag.FileSize = note.FileSize;
                ViewBag.FileNamePrevious =
                    $"기존에 업로드된 파일명: {note.FileName}";
            }
            else
            {
                ViewBag.FileName = "";
                ViewBag.FileSize = 0;
            }

            return View(note);
        }

        /// <summary>
        /// 게시판 수정 처리 + 파일 업로드
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Edit(
            Note model, ICollection<IFormFile> files,
            int id, string previousFileName = "", int previousFileSize = 0)
        {
            ViewBag.FormType = BoardWriteFormType.Modify;
            ViewBag.TitleDescription = "글 수정 - 아래 항목을 수정하세요.";
            ViewBag.SaveButtonText = "수정";

            string fileName = "";
            int fileSize = 0;

            if (previousFileName != null)
            {
                fileName = previousFileName;
                fileSize = previousFileSize;
            }

            //파일 업로드 처리 시작
            var uploadFolder = Path.Combine(_environment.WebRootPath, "files");

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    fileSize = Convert.ToInt32(file.Length);
                    // 파일명 중복 처리
                    fileName = Dul.FileUtility.GetFileNameWithNumbering(
                        uploadFolder, Path.GetFileName(
                            ContentDispositionHeaderValue.Parse(
                                file.ContentDisposition).FileName.Trim('"')));
                    // 파일업로드
                    using (var fileStream = new FileStream(
                        Path.Combine(uploadFolder, fileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
            }

            Note note = new Note();

            note.Id = id;
            note.Name = model.Name;
            //note.Email = Dul.HtmlUtility.Encode(model.Email);
            note.Email = model.Email;
            note.Homepage = model.Homepage;
            //note.Title = Dul.HtmlUtility.Encode(model.Title);
            note.Title = model.Title;
            note.Content = model.Content;
            note.FileName = fileName;
            note.FileSize = fileSize;
            note.Password =
                (new Dul.Security.CryptorEngine()).EncryptPassword(model.Password);
            note.ModifyIp =
                HttpContext.Connection.RemoteIpAddress.ToString(); // IP 주소
            note.Encoding = model.Encoding;

            int r = _repository.UpdateNote(note); // 데이터베이스에 수정 적용
            if (r > 0)
            {
                TempData["Message"] = "수정되었습니다.";
                return RedirectToAction("Details", new { Id = id });
            }
            else
            {
                ViewBag.ErrorMessage =
                    "업데이트가 되지 않았습니다. 암호를 확인하세요.";
                return View(note);
            }
        }
        #endregion

        /// <summary>
        /// 답변 글쓰기 폼
        /// </summary>
        /// <param name="id">부모글 Id</param>
        [HttpGet]
        [Authorize] // 스팸 글 때문에 추가
        public IActionResult Reply(int id)
        {
            ViewBag.FormType = BoardWriteFormType.Reply;
            ViewBag.TitleDescription = "글 답변 - 다음 필드들을 채워주세요.";
            ViewBag.SaveButtonText = "답변";

            // 기존 데이터를 바인딩
            var note = _repository.GetNoteById(id); // 기존 부모글 Id

            // 새로운 Note 개체 생성
            var newNote = new Note();

            // 기존 글의 제목과 내용을 새 Note 개체에 저장 후 전달
            newNote.Title = $"Re : {note.Title}";
            newNote.Content =
                $"\n\nOn {note.PostDate}, '{note.Name}' wrote:\n----------\n>"
                + $"{note.Content.Replace("\n", "\n>")}\n---------";

            return View(newNote);
        }

        /// <summary>
        /// 답변 글쓰기 처리 + 파일 업로드
        /// </summary>
        [HttpPost]
        [Authorize] // 스팸 글 때문에 추가
        public async Task<IActionResult> Reply(
            Note model, ICollection<IFormFile> files, int id)
        {
            //파일 업로드 처리 시작
            string fileName = String.Empty;
            int fileSize = 0;

            var uploadFolder = Path.Combine(_environment.WebRootPath, "files");

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    fileSize = Convert.ToInt32(file.Length);
                    // 파일명 중복 처리
                    fileName = Dul.FileUtility.GetFileNameWithNumbering(
                        uploadFolder, Path.GetFileName(
                            ContentDispositionHeaderValue.Parse(
                                file.ContentDisposition).FileName.Trim('"')));
                    // 파일 업로드
                    using (var fileStream = new FileStream(
                        Path.Combine(uploadFolder, fileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
            }

            Note note = new Note();

            note.Id = note.ParentNum = Convert.ToInt32(id); // 부모글 저장
            note.Name = model.Name;
            note.Email = Dul.HtmlUtility.Encode(model.Email);
            note.Homepage = model.Homepage;
            note.Title = Dul.HtmlUtility.Encode(model.Title);
            note.Content = model.Content;
            note.FileName = fileName;
            note.FileSize = fileSize;
            note.Password =
                (new Dul.Security.CryptorEngine()).EncryptPassword(model.Password);
            note.PostIp = HttpContext.Connection.RemoteIpAddress.ToString();
            note.Encoding = model.Encoding;

            _repository.ReplyNote(note); // 데이터 답변 저장

            TempData["Message"] = "데이터가 저장되었습니다.";

            return RedirectToAction("Index");
        }

        /// <summary>
        /// ImageDown : 완성형(DotNetNote) 게시판의 이미지전용다운 페이지
        /// 이미지 경로를 보여주지 않고 다운로드: 
        ///    대용량 사이트 운영시 직접 이미지 경로 사용 권장(CDN 사용)
        /// /DotNetNote/ImageDown/1234 => 1234번 이미지 파일을 강제 다운로드
        /// <img src="/DotNetNote/ImageDown/1234" /> => 이미지 태그 실행
        /// </summary>
        public IActionResult ImageDown(int id)
        {
            string fileName = "";

            // 넘겨져 온 번호에 해당하는 파일명 가져오기(보안때문에... 파일명 숨김)
            fileName = _repository.GetFileNameById(id);

            if (fileName == null)
            {
                return null;
            }
            else
            {
                string strFileName = fileName;
                string strFileExt = Path.GetExtension(strFileName);
                string strContentType = "";
                if (strFileExt == ".gif" || strFileExt == ".jpg"
                    || strFileExt == ".jpeg" || strFileExt == ".png")
                {
                    switch (strFileExt)
                    {
                        case ".gif":
                            strContentType = "image/gif"; break;
                        case ".jpg":
                            strContentType = "image/jpeg"; break;
                        case ".jpeg":
                            strContentType = "image/jpeg"; break;
                        case ".png":
                            strContentType = "image/png"; break;
                    }
                }

                if (System.IO.File.Exists(Path.Combine(_environment.WebRootPath, "files") + "\\" + fileName))
                {
                    // 다운로드 카운트 증가 메서드 호출
                    _repository.UpdateDownCount(fileName);

                    // 이미지 파일 정보 얻기
                    byte[] fileBytes = System.IO.File.ReadAllBytes(Path.Combine(
                        _environment.WebRootPath, "files") + "\\" + fileName);

                    // 이미지 파일 다운로드 
                    return File(fileBytes, strContentType, fileName);
                }

                return Content("http://placehold.it/250x150?text=NoImage");
            }
        }

        #region CommentAdd: 댓글 입력 처리
        /// <summary>
        /// 댓글 입력
        /// </summary>
        [HttpPost]
        [Authorize] // 스팸 글 때문에 추가
        public IActionResult CommentAdd(
            int BoardId, string txtName, string txtPassword, string txtOpinion)
        {
            // 댓글 개체 생성
            NoteComment comment = new NoteComment();
            comment.BoardId = BoardId;
            comment.Name = txtName;
            comment.Password =
                (new Dul.Security.CryptorEngine()).EncryptPassword(txtPassword);
            comment.Opinion = txtOpinion;

            // 댓글 데이터 저장
            _commentRepository.AddNoteComment(comment);

            // 댓글 저장 후 다시 게시판 상세 보기 페이지로 이동
            return RedirectToAction("Details", new { Id = BoardId });
        }
        #endregion

        /// <summary>
        /// 댓글 삭제 폼
        /// </summary>
        [HttpGet]
        public IActionResult CommentDelete(string boardId, string id)
        {
            ViewBag.BoardId = boardId;
            ViewBag.Id = id;

            return View();
        }

        /// <summary>
        /// 댓글 삭제 처리
        /// </summary>
        [HttpPost]
        public IActionResult CommentDelete(
            string boardId, string id, string txtPassword)
        {
            txtPassword = (new Dul.Security.CryptorEngine()).EncryptPassword(txtPassword);
            // 현재 삭제하려는 댓글의 암호가 맞으면, 삭제 진행
            if (_commentRepository.GetCountBy(Convert.ToInt32(boardId)
                , Convert.ToInt32(id), txtPassword) > 0)
            {
                // 삭제 처리
                _commentRepository.DeleteNoteComment(
                    Convert.ToInt32(boardId), Convert.ToInt32(id), txtPassword);
                // 게시판 상세 보기 페이지로 이동
                return RedirectToAction("Details", new { Id = boardId });
            }

            ViewBag.BoardId = boardId;
            ViewBag.Id = id;
            ViewBag.ErrorMessage = "암호가 틀립니다. 다시 입력해주세요.";

            return View();
        }

        /// <summary>
        /// 공지글로 올리기(관리자 전용)
        /// </summary>
        [HttpGet]
        [Authorize("Administrators")]
        public IActionResult Pinned(int id)
        {
            // 공지사항(NOTICE)으로 올리기
            _repository.Pinned(id);

            return RedirectToAction("Details", new { Id = id });
        }

        /// <summary>
        /// (참고) 최근 글 리스트 Web API 테스트 페이지
        /// </summary>
        [Authorize("Administrators")]
        public IActionResult NoteServiceDemo() => View();

        /// <summary>
        /// (참고) 최근 댓글 리스트 Web API 테스트 페이지
        /// </summary>
        [Authorize("Administrators")]
        public IActionResult NoteCommentServiceDemo() => View();

        #region 공지사항 모듈
        /// <summary>
        /// 로그인 페이지의 메인 공지사항 목록에 사용될 공지사항 목록 조회
        /// </summary>
        /// <param name="page">페이지</param>
        /// <returns>공지사항 목록</returns>
        //[HttpPost]
        [AllowAnonymous]
        public JsonResult MainList(int page = 1)
        {
            return Json(new
            {
                list = _context.Notes.OrderByDescending(n => n.Id).Skip((page - 1) * 5).Take(5).Select(x => new
                {
                    num = x.Id,
                    title = x.Title,
                    postDate = x.PostDate.ToString("yyyy.MM.dd")
                }),
                count = _context.Notes.Count()
            });
        }

        public IActionResult MainListPage() => View();

        /// <summary>
        /// 로그인 페이지의 팝업 공지사항 목록에 사용될 공지사항 목록 조회
        /// </summary>
        /// <param name="page">페이지</param>
        /// <param name="keyword">검색 키워드</param>
        /// <returns>공지사항 목록</returns>
        [HttpPost]
        [AllowAnonymous]
        public JsonResult PopupList(int page = 1, string keyword = "")
        {
            int cnt = 0;

            List<Note> notices;
            if (string.IsNullOrWhiteSpace(keyword))
            {
                notices = _context.Notes.OrderByDescending(n => n.Id).Skip((page - 1) * 5).Take(5).ToList();
                cnt = _context.Notes.Count();
            }
            else
            {
                notices = _context.Notes.OrderByDescending(n => n.Id).Where(n => n.Title.Contains(keyword)).Skip((page - 1) * 5).Take(5).ToList();
                cnt = _context.Notes.Where(n => EF.Functions.Like(n.Title, "N%" + keyword + "%") || n.Title.Contains(keyword)).Count();
            }

            return Json(new
            {
                list = notices.Select(x => new
                {
                    num = x.Id,
                    title = x.Title,
                    postDate = x.PostDate.ToString("yyyy.MM.dd"),
                    fileName = x.FileName
                }),
                count = cnt
            });
        }

        /// <summary>
        /// 로그인 페이지의 공지사항 상세에 사용될 공지사항 정보 조회
        /// </summary>
        /// <param name="num">키</param>
        /// <param name="keyword">검색 키워드</param>
        /// <returns>공지사항 정보</returns>
        [HttpPost]
        [AllowAnonymous]
        public JsonResult NoticeView(int num, string keyword = "")
        {
            var articleBase = _context.Notes.Where(n => n.Id == num).SingleOrDefault();

            Note prevArticleSet = new Note();
            Note nextArticleSet = new Note();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                prevArticleSet = _context.Notes.Where(n => n.Id < num).OrderByDescending(n => n.Id).FirstOrDefault(); // 이전 
                nextArticleSet = _context.Notes.Where(n => n.Id > num).OrderBy(n => n.Id).FirstOrDefault(); // 다음 
            }
            else
            {
                prevArticleSet = _context.Notes.Where(n => n.Id < num && n.Title.Contains(keyword)).OrderByDescending(n => n.Id).FirstOrDefault(); // 이전 
                nextArticleSet = _context.Notes.Where(n => n.Id > num && n.Title.Contains(keyword)).OrderBy(n => n.Id).FirstOrDefault(); // 다음 
            }

            return Json(new
            {
                title = articleBase.Title,
                postDate = articleBase.PostDate.ToString("yyyy.MM.dd"),
                content = articleBase.Content,
                fileName = articleBase.FileName,
                prevNum = (prevArticleSet != null ? prevArticleSet.Id : -1),
                prevTitle = (prevArticleSet != null ? prevArticleSet.Title : ""),
                nextNum = (nextArticleSet != null ? nextArticleSet.Id : -1),
                nextTitle = (nextArticleSet != null ? nextArticleSet.Title : "")
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult DeleteArticleById(int id)
        {
            var deleteArticle = _context.Notes.Where(n => n.Id == id).SingleOrDefault();
            _context.Entry(deleteArticle).State = EntityState.Deleted;
            _context.SaveChanges();

            return Json(new
            {
                message = "DELETED"
            });
        }

        /// <summary>
        /// 새로운 프로젝트를 등록하고 그 결과를 반환합니다.
        /// </summary>
        /// <param name="note">Project 개체의 인스턴스</param>
        /// <returns>등록결과</returns>
        [HttpPost]
        [Route("api/DotNetNote/PostNote")]
        [AllowAnonymous]
        public IActionResult PostNote([FromBody] Note note)
        {
            if (ModelState.IsValid)
            {
                if (note.Id == -1)
                {
                    // 입력
                    note.Id = 0;

                    _repository.Add(note);

                    // 201 응답을 생성합니다.
                    var response = new HttpResponseMessage(HttpStatusCode.Created)
                    {
                        Content = new StringContent(note.Id.ToString())
                    };

                    return Created("", "");

                }
                else
                {
                    int r = _repository.UpdateNote(note);

                    if (r < 1)
                    {
                        return Json(new
                        {
                            modified = false,
                            msg = "수정되지 않았습니다."
                        });
                    }
                    return Created("", "");
                }
            }
            else
            {
                //return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState); // ASP.NET 
                HttpRequestMessage request = new HttpRequestMessage();
                //return request.CreateResponse(HttpStatusCode.BadRequest); // ASP.NET Core
                return BadRequest(ModelState);
            }
        }

        ///// <summary>
        ///// 로그인 페이지의 공지사항 상세 파일 다운로드
        ///// </summary>
        ///// <param name="num">게시물 번호</param>
        ///// <returns>파일</returns>
        //[AllowAnonymous]
        //public ActionResult Download(int num)
        //{
        //    string tableID = HttpContext.Application["Notice.TID"].ToString();
        //    string tableName = tableID.ToUpper();
        //    string fileName = (new ArticleRepository()).GetFileNameById(tableName, num);

        //    if (Convert.ToBoolean(ConfigurationManager.AppSettings["AZURE_STORAGE_ENABLE"].ToString()))
        //    {
        //        //return Redirect($"/BoardDown.aspx?BoardName={boardName}&Num={file.ArticleId}&FileName={file.FileName}&IsSub={file.ArticleId.ToString().PadLeft(8, '0')}");
        //        // 서브 폴더에서 다운로드: 멀티파일 리스트에서 다운로드할 때 사용 됨 
        //        AzureBlobFileManager.FileDownFromAzureBlob(ConfigurationManager.AppSettings["AZURE_STORAGE_CONNECTIONSTRING"].ToString(), ConfigurationManager.AppSettings["AZURE_STORAGE_CONTAINERNAME"].ToString(), ConfigurationManager.AppSettings["AZURE_STORAGE_SUBFOLDER"].ToString(), tableName, fileName);

        //        return View();
        //    }
        //    else
        //    {
        //        // 대상 파일 다운로드
        //        // FILE_UPLOAD_FOLDER 항목이 지정되어 있지 않으면 프로젝트 루트의 BoardFiles 폴더에 저장, 지정되었으면 해당 폴더에 저장 
        //        string downloadPath = (ConfigurationManager.AppSettings["FILE_UPLOAD_FOLDER"].ToString() == "") ?
        //            Path.Combine(Server.MapPath("./BoardFiles"), tableID) + "\\" :
        //            Path.Combine(HttpContext.Application["BOARD_FILE_PATH"].ToString(), tableID) + "\\";

        //        byte[] fileBytes = System.IO.File.ReadAllBytes(Path.Combine(downloadPath, fileName));

        //        return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        //    }
        //}
        #endregion
    }
}
