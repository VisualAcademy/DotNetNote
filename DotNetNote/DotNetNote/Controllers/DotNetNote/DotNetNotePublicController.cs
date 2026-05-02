using DotNetNote.Models.Notes;
using Dul.Board;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace DotNetNote.Controllers;

[Authorize]
public class DotNetNotePublicController(
    IWebHostEnvironment environment,
    INoteRepository repository,
    INoteCommentRepository commentRepository,
    ILogger<DotNetNoteController> logger
        ) : Controller
{
    // 공통 속성: 검색 모드: 검색 모드이면 true, 그렇지 않으면 false.
    public bool SearchMode { get; set; } = false;

    public string SearchField { get; set; } = string.Empty; // 필드: Name, Title, Content

    public string SearchQuery { get; set; } = string.Empty; // 검색 내용

    /// <summary>
    /// 현재 보여줄 페이지 번호
    /// </summary>
    public int PageIndex { get; set; } = 0;

    /// <summary>
    /// 총 레코드 갯수(글번호 순서 정렬용)
    /// </summary>
    public int TotalRecordCount { get; set; } = 0;

    /// <summary>
    /// 게시판 리스트 페이지
    /// </summary>
    public IActionResult Index()
    {
        // 로깅
        logger.LogInformation("게시판 리스트 페이지 로딩");

        string searchField = Request.Query["SearchField"].ToString();
        string searchQuery = Request.Query["SearchQuery"].ToString();

        // 검색 모드 결정: ?SearchField=Name&SearchQuery=닷넷코리아 
        SearchMode = (
            !string.IsNullOrWhiteSpace(searchField) &&
            !string.IsNullOrWhiteSpace(searchQuery)
        );

        // 검색 환경이면 속성에 저장
        if (SearchMode)
        {
            SearchField = searchField;
            SearchQuery = searchQuery;
        }

        //[1] 쿼리스트링에 따른 페이지 보여주기
        string page = Request.Query["Page"].ToString();

        if (!string.IsNullOrWhiteSpace(page) && int.TryParse(page, out int pageNumber))
        {
            // Page는 보여지는 쪽은 1, 2, 3, ... 코드단에서는 0, 1, 2, ...
            PageIndex = Math.Max(pageNumber - 1, 0);
        }

        //[2] 쿠키를 사용한 리스트 페이지 번호 유지 적용(Optional): 
        //    100번째 페이지 보고 있다가 다시 리스트 왔을 때 100번째 페이지 표시
        string? savedPageNumber = Request.Cookies["DotNetNotePageNum"];

        if (!string.IsNullOrWhiteSpace(savedPageNumber) &&
            int.TryParse(savedPageNumber, out int savedPageIndex))
        {
            PageIndex = Math.Max(savedPageIndex, 0);
        }

        // 게시판 리스트 정보 가져오기
        List<Note> notes;

        if (!SearchMode)
        {
            TotalRecordCount = repository.GetCountAll();
            notes = repository.GetAll(PageIndex);
        }
        else
        {
            TotalRecordCount = repository.GetCountBySearch(SearchField, SearchQuery);
            notes = repository.GetSeachAll(PageIndex, SearchField, SearchQuery);
        }

        // 주요 정보를 뷰 페이지로 전송
        ViewBag.TotalRecord = TotalRecordCount;
        ViewBag.SearchMode = SearchMode;
        ViewBag.SearchField = SearchField;
        ViewBag.SearchQuery = SearchQuery;

        return View(notes);
    }

    /// <summary>
    /// 게시판 글쓰기 폼
    /// </summary>
    [HttpGet]
    //[Authorize] // 스팸 글 때문에 추가
    public IActionResult Create()
    {
        // 로깅
        logger.LogInformation("게시판 글쓰기 페이지 로딩");

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
    public async Task<IActionResult> Create(Note model, ICollection<IFormFile> files)
    {
        // 파일 업로드 처리 시작
        string fileName = string.Empty;
        int fileSize = 0;

        string uploadFolder = GetUploadFolder();

        foreach (IFormFile file in files)
        {
            if (file.Length > 0)
            {
                fileSize = Convert.ToInt32(file.Length);
                fileName = GetSafeUploadFileName(uploadFolder, file);

                using FileStream fileStream = new(
                    Path.Combine(uploadFolder, fileName),
                    FileMode.Create);

                await file.CopyToAsync(fileStream);
            }
        }

        Note note = new()
        {
            Name = model.Name,
            Email = Dul.HtmlUtility.Encode(model.Email),
            Homepage = model.Homepage,
            Title = Dul.HtmlUtility.Encode(model.Title),
            Content = model.Content,
            FileName = fileName,
            FileSize = fileSize,
            Password = new Dul.Security.CryptorEngine().EncryptPassword(model.Password),
            PostIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
            Encoding = model.Encoding
        };

        repository.Add(note); // 데이터 저장

        // 데이터 저장 후 리스트 페이지 이동시 toastr로 메시지 출력
        TempData["Message"] = "데이터가 저장되었습니다.";

        return RedirectToAction("Index"); // 저장 후 리스트 페이지로 이동
    }

    /// <summary>
    /// 게시판 파일 강제 다운로드 기능(/BoardDown/:Id)
    /// </summary>
    public IActionResult BoardDown(int id)
    {
        // 넘겨져 온 번호에 해당하는 파일명 가져오기(보안때문에... 파일명 숨김)
        string? fileName = repository.GetFileNameById(id);

        if (string.IsNullOrWhiteSpace(fileName))
        {
            return NotFound();
        }

        string filePath = Path.Combine(GetUploadFolder(), fileName);

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound();
        }

        // 다운로드 카운트 증가 메서드 호출
        repository.UpdateDownCountById(id);

        byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

        return File(fileBytes, "application/octet-stream", fileName);
    }

    /// <summary>
    /// 게시판의 상세 보기 페이지(Details, BoardView)
    /// </summary>
    public IActionResult Details(int id)
    {
        // 넘겨온 Id 값에 해당하는 레코드 하나 읽어서 Note 클래스에 바인딩
        Note? note = repository.GetNoteById(id);

        if (note == null)
        {
            return NotFound();
        }

        //[!] 인코딩 방식에 따른 데이터 출력: 
        // 직접 문자열 비교해도 되지만, 학습목적으로 열거형으로 비교 
        string encodingText = string.IsNullOrWhiteSpace(note.Encoding)
            ? ContentEncodingType.Html.ToString()
            : note.Encoding;

        if (!Enum.TryParse(encodingText, out ContentEncodingType encoding))
        {
            encoding = ContentEncodingType.Html;
        }

        string noteContent = note.Content ?? string.Empty;

        string encodedContent = encoding switch
        {
            // Text : 소스 그대로 표현
            ContentEncodingType.Text =>
                Dul.HtmlUtility.EncodeWithTabAndSpace(noteContent),

            // Html : HTML 형식으로 출력
            ContentEncodingType.Html =>
                noteContent,

            // Mixed : 엔터처리만
            ContentEncodingType.Mixed =>
                noteContent.Replace("\r\n", "<br />"),

            // Html : 기본
            _ =>
                noteContent
        };

        ViewBag.Content = encodedContent; //[!]

        // 첨부된 파일 확인
        if (!string.IsNullOrWhiteSpace(note.FileName) && note.FileName.Length > 1)
        {
            //[a] 파일 다운로드 링크: String.Format()으로 표현해 봄 
            ViewBag.FileName = string.Format(
                "<a href='/DotNetNote/BoardDown?Id={0}'>"
                + "{1}{2} / 전송수: {3}</a>",
                note.Id,
                "<img src=\"/images/ext/ext_zip.gif\" border=\"0\">",
                note.FileName,
                note.DownCount);

            //[b] 이미지 미리보기: C# 6.0 String 보간법으로 표현해 봄
            if (Dul.BoardLibrary.IsPhoto(note.FileName))
            {
                ViewBag.ImageDown =
                    $"<img src='/DotNetNote/ImageDown/{note.Id}'><br />";
            }
        }
        else
        {
            ViewBag.FileName = "(업로드된 파일이 없습니다.)";
        }

        // 현재 글에 해당하는 댓글 리스트와 현재 글 번호를 담아서 전달
        NoteCommentViewModel vm = new()
        {
            NoteCommentList = commentRepository.GetNoteComments(note.Id),
            BoardId = note.Id
        };

        ViewBag.CommentListAndId = vm;

        return View(note);
    }

    /// <summary>
    /// 게시판 삭제 폼
    /// </summary>
    [HttpGet]
    public IActionResult Delete(int id)
    {
        ViewBag.Id = id;
        return View();
    }

    /// <summary>
    /// 게시판 삭제 처리
    /// </summary>
    [HttpPost]
    public IActionResult Delete(int id, string Password)
    {
        string encryptedPassword =
            new Dul.Security.CryptorEngine().EncryptPassword(Password);

        //if (_repository.DeleteNote(id, Password) > 0)
        if (repository.DeleteNote(id, encryptedPassword) > 0)
        {
            TempData["Message"] = "데이터가 삭제되었습니다.";

            // 학습 목적으로 삭제 후의 이동 페이지를 2군데 중 하나로 분기
            if (DateTime.Now.Second % 2 == 0)
            {
                //[a] 삭제 후 특정 뷰 페이지로 이동
                return RedirectToAction("DeleteCompleted");
            }

            //[b] 삭제 후 Index 페이지로 이동
            return RedirectToAction("Index");
        }

        ViewBag.Message = "삭제되지 않았습니다. 비밀번호를 확인하세요.";
        ViewBag.Id = id;

        return View();
    }

    /// <summary>
    /// 게시판 삭제 완료 후 추가적인 처리할 때 페이지
    /// </summary>
    public IActionResult DeleteCompleted() => View();

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
        Note? note = repository.GetNoteById(id);

        if (note == null)
        {
            return NotFound();
        }

        // 첨부된 파일명 및 파일크기 기록
        if (!string.IsNullOrWhiteSpace(note.FileName) && note.FileName.Length > 1)
        {
            ViewBag.FileName = note.FileName;
            ViewBag.FileSize = note.FileSize;
            ViewBag.FileNamePrevious =
                $"기존에 업로드된 파일명: {note.FileName}";
        }
        else
        {
            ViewBag.FileName = string.Empty;
            ViewBag.FileSize = 0;
        }

        return View(note);
    }

    /// <summary>
    /// 게시판 수정 처리 + 파일 업로드
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Edit(
        Note model,
        ICollection<IFormFile> files,
        int id,
        string previousFileName = "",
        int previousFileSize = 0)
    {
        ViewBag.FormType = BoardWriteFormType.Modify;
        ViewBag.TitleDescription = "글 수정 - 아래 항목을 수정하세요.";
        ViewBag.SaveButtonText = "수정";

        string fileName = string.Empty;
        int fileSize = 0;

        if (!string.IsNullOrWhiteSpace(previousFileName))
        {
            fileName = previousFileName;
            fileSize = previousFileSize;
        }

        // 파일 업로드 처리 시작
        string uploadFolder = GetUploadFolder();

        foreach (IFormFile file in files)
        {
            if (file.Length > 0)
            {
                fileSize = Convert.ToInt32(file.Length);
                fileName = GetSafeUploadFileName(uploadFolder, file);

                using FileStream fileStream = new(
                    Path.Combine(uploadFolder, fileName),
                    FileMode.Create);

                await file.CopyToAsync(fileStream);
            }
        }

        Note note = new()
        {
            Id = id,
            Name = model.Name,
            Email = Dul.HtmlUtility.Encode(model.Email),
            Homepage = model.Homepage,
            Title = Dul.HtmlUtility.Encode(model.Title),
            Content = model.Content,
            FileName = fileName,
            FileSize = fileSize,
            Password = new Dul.Security.CryptorEngine().EncryptPassword(model.Password),
            ModifyIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
            Encoding = model.Encoding
        };

        int r = repository.UpdateNote(note); // 데이터베이스에 수정 적용

        if (r > 0)
        {
            TempData["Message"] = "수정되었습니다.";
            return RedirectToAction("Details", new { Id = id });
        }

        ViewBag.ErrorMessage = "업데이트가 되지 않았습니다. 암호를 확인하세요.";
        return View(note);
    }

    /// <summary>
    /// 답변 글쓰기 폼
    /// </summary>
    /// <param name="id">부모글 Id</param>
    [HttpGet]
    //[Authorize] // 스팸 글 때문에 추가
    public IActionResult Reply(int id)
    {
        ViewBag.FormType = BoardWriteFormType.Reply;
        ViewBag.TitleDescription = "글 답변 - 다음 필드들을 채워주세요.";
        ViewBag.SaveButtonText = "답변";

        // 기존 데이터를 바인딩
        Note? note = repository.GetNoteById(id); // 기존 부모글 Id

        if (note == null)
        {
            return NotFound();
        }

        string originalTitle = note.Title ?? string.Empty;
        string originalContent = note.Content ?? string.Empty;
        string originalName = note.Name ?? string.Empty;

        // 새로운 Note 개체 생성
        Note newNote = new()
        {
            // 기존 글의 제목과 내용을 새 Note 개체에 저장 후 전달
            Title = $"Re : {originalTitle}",
            Content =
                $"\n\nOn {note.PostDate}, '{originalName}' wrote:\n----------\n>"
                + $"{originalContent.Replace("\n", "\n>")}\n---------"
        };

        return View(newNote);
    }

    /// <summary>
    /// 답변 글쓰기 처리 + 파일 업로드
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Reply(
        Note model,
        ICollection<IFormFile> files,
        int id)
    {
        // 파일 업로드 처리 시작
        string fileName = string.Empty;
        int fileSize = 0;

        string uploadFolder = GetUploadFolder();

        foreach (IFormFile file in files)
        {
            if (file.Length > 0)
            {
                fileSize = Convert.ToInt32(file.Length);
                fileName = GetSafeUploadFileName(uploadFolder, file);

                using FileStream fileStream = new(
                    Path.Combine(uploadFolder, fileName),
                    FileMode.Create);

                await file.CopyToAsync(fileStream);
            }
        }

        Note note = new()
        {
            Id = Convert.ToInt32(id),
            ParentNum = Convert.ToInt32(id),
            Name = model.Name,
            Email = Dul.HtmlUtility.Encode(model.Email),
            Homepage = model.Homepage,
            Title = Dul.HtmlUtility.Encode(model.Title),
            Content = model.Content,
            FileName = fileName,
            FileSize = fileSize,
            Password = new Dul.Security.CryptorEngine().EncryptPassword(model.Password),
            PostIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
            Encoding = model.Encoding
        };

        repository.ReplyNote(note); // 데이터 답변 저장

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
        // 넘겨져 온 번호에 해당하는 파일명 가져오기(보안때문에... 파일명 숨김)
        string? fileName = repository.GetFileNameById(id);

        if (string.IsNullOrWhiteSpace(fileName))
        {
            return NotFound();
        }

        string fileExtension = Path.GetExtension(fileName).ToLowerInvariant();

        string contentType = fileExtension switch
        {
            ".gif" => "image/gif",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => "application/octet-stream"
        };

        string filePath = Path.Combine(GetUploadFolder(), fileName);

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound();
        }

        // 다운로드 카운트 증가 메서드 호출
        repository.UpdateDownCount(fileName);

        // 이미지 파일 정보 얻기
        byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

        // 이미지 파일 다운로드 
        return File(fileBytes, contentType, fileName);
    }

    /// <summary>
    /// 댓글 입력
    /// </summary>
    [HttpPost]
    //[Authorize] // 스팸 글 때문에 추가
    public IActionResult CommentAdd(
        int BoardId,
        string txtName,
        string txtPassword,
        string txtOpinion)
    {
        // 댓글 개체 생성
        NoteComment comment = new()
        {
            BoardId = BoardId,
            Name = txtName,
            Password = new Dul.Security.CryptorEngine().EncryptPassword(txtPassword),
            Opinion = txtOpinion
        };

        // 댓글 데이터 저장
        commentRepository.AddNoteComment(comment);

        // 댓글 저장 후 다시 게시판 상세 보기 페이지로 이동
        return RedirectToAction("Details", new { Id = BoardId });
    }

    /// <summary>
    /// 댓글 삭제 폼
    /// </summary>
    [HttpGet]
    public IActionResult CommentDelete(string BoardId, string Id)
    {
        ViewBag.BoardId = BoardId;
        ViewBag.Id = Id;

        return View();
    }

    /// <summary>
    /// 댓글 삭제 처리
    /// </summary>
    [HttpPost]
    public IActionResult CommentDelete(
        string BoardId,
        string Id,
        string txtPassword)
    {
        string encryptedPassword =
            new Dul.Security.CryptorEngine().EncryptPassword(txtPassword);

        if (!int.TryParse(BoardId, out int boardId) ||
            !int.TryParse(Id, out int commentId))
        {
            return BadRequest();
        }

        // 현재 삭제하려는 댓글의 암호가 맞으면, 삭제 진행
        if (commentRepository.GetCountBy(boardId, commentId, encryptedPassword) > 0)
        {
            // 삭제 처리
            commentRepository.DeleteNoteComment(boardId, commentId, encryptedPassword);

            // 게시판 상세 보기 페이지로 이동
            return RedirectToAction("Details", new { Id = boardId });
        }

        ViewBag.BoardId = BoardId;
        ViewBag.Id = Id;
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
        repository.Pinned(id);

        return RedirectToAction("Details", new { Id = id });
    }

    /// <summary>
    /// (참고) 최근 글 리스트 Web API 테스트 페이지
    /// </summary>
    public IActionResult NoteServiceDemo() => View();

    /// <summary>
    /// (참고) 최근 댓글 리스트 Web API 테스트 페이지
    /// </summary>
    public IActionResult NoteCommentServiceDemo() => View();

    private string GetUploadFolder()
    {
        string webRootPath = environment.WebRootPath ?? string.Empty;
        string uploadFolder = Path.Combine(webRootPath, "files");

        if (!Directory.Exists(uploadFolder))
        {
            Directory.CreateDirectory(uploadFolder);
        }

        return uploadFolder;
    }

    private static string GetSafeUploadFileName(string uploadFolder, IFormFile file)
    {
        string fileName = "upload.bin";

        if (!string.IsNullOrWhiteSpace(file.ContentDisposition))
        {
            ContentDispositionHeaderValue contentDisposition =
                ContentDispositionHeaderValue.Parse(file.ContentDisposition);

            string? parsedFileName = contentDisposition.FileName?.Trim('"');

            if (!string.IsNullOrWhiteSpace(parsedFileName))
            {
                fileName = Path.GetFileName(parsedFileName);
            }
        }

        return Dul.FileUtility.GetFileNameWithNumbering(uploadFolder, fileName);
    }
}