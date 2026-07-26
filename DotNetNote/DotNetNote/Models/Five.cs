using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace DotNetNote.Models;

/// <summary>
/// [1] 모델 클래스
/// </summary>
public class Five
{
    public int Id { get; set; }

    [Required]
    public string Note { get; set; } = string.Empty;
}

/// <summary>
/// [2] 인터페이스
/// </summary>
public interface IFiveRepository
{
    Five Add(Five model);

    List<Five> GetAll();

    Five GetById(int id);

    Five Update(Five model);

    void Remove(int id);

    List<Five> GetAllWithPaging(
        int pageIndex,
        int pageSize = 10);

    int GetRecordCount();
}

/// <summary>
/// [3] 리포지토리 클래스
/// </summary>
public class FiveRepository : IFiveRepository
{
    private readonly IConfiguration _config;
    private readonly IDbConnection _db;

    public FiveRepository(IConfiguration config)
    {
        _config = config
            ?? throw new ArgumentNullException(nameof(config));

        var connectionString =
            _config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "DefaultConnection 연결 문자열이 설정되지 않았습니다.");

        _db = new SqlConnection(connectionString);
    }

    /// <summary>
    /// 출력 메서드
    /// </summary>
    public List<Five> GetAll()
    {
        const string sql = """
            SELECT *
            FROM Fives
            ORDER BY Id DESC
            """;

        return _db.Query<Five>(sql).ToList();
    }

    /// <summary>
    /// 상세
    /// </summary>
    public Five GetById(int id)
    {
        const string sql = """
            SELECT *
            FROM Fives
            WHERE Id = @Id
            """;

        return _db.Query<Five>(
            sql,
            new { Id = id }).Single();
    }

    /// <summary>
    /// 입력 메서드, Add, AddXXX, CreateXXX
    /// </summary>
    public Five Add(Five model)
    {
        const string sql = """
            INSERT INTO Fives (Note)
            VALUES (@Note);

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        var id = _db.Query<int>(sql, model).Single();

        model.Id = id;

        return model;
    }

    /// <summary>
    /// 수정
    /// </summary>
    public Five Update(Five model)
    {
        const string sql = """
            UPDATE Fives
            SET Note = @Note
            WHERE Id = @Id
            """;

        _db.Execute(sql, model);

        return model;
    }

    /// <summary>
    /// 삭제
    /// </summary>
    public void Remove(int id)
    {
        const string sql = """
            DELETE FROM Fives
            WHERE Id = @Id
            """;

        _db.Execute(sql, new { Id = id });
    }

    /// <summary>
    /// 페이징 처리된 리스트
    /// </summary>
    public List<Five> GetAllWithPaging(
        int pageIndex,
        int pageSize = 10)
    {
        const string sql = """
            SELECT Id, Note
            FROM
            (
                SELECT
                    ROW_NUMBER() OVER (ORDER BY Id DESC) AS RowNumbers,
                    Id,
                    Note
                FROM Fives
            ) AS TempRowTables
            WHERE RowNumbers
                BETWEEN (@PageIndex * @PageSize + 1)
                AND ((@PageIndex + 1) * @PageSize)
            """;

        return _db.Query<Five>(
            sql,
            new
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            }).ToList();
    }

    /// <summary>
    /// 레코드 카운트
    /// </summary>
    public int GetRecordCount()
    {
        const string sql = """
            SELECT COUNT(*)
            FROM Fives
            """;

        return _db.Query<int>(sql).FirstOrDefault();
    }
}

/// <summary>
/// [4] Web API 컨트롤러 클래스
/// </summary>
// 컨벤션 기반 라우팅 대신에 어트리뷰트 라우팅 추천
// [Route("api/fives")] // 직접 API 이름을 지정할 때
[Route("api/[controller]")]
public class FiveServiceController : Controller
{
    private readonly IFiveRepository _repository;

    public FiveServiceController(IFiveRepository repository)
    {
        _repository = repository
            ?? throw new ArgumentNullException(nameof(repository));
    }

    [HttpGet]
    public IActionResult Get()
    {
        // 500 에러를 확인하려면 다음 예외를 사용합니다.
        // throw new Exception("인위적으로 에러 발생시켜 500에러 출력");

        try
        {
            var fives = _repository.GetAll();

            if (fives.Count == 0)
            {
                return NotFound("아무런 데이터가 없습니다.");
            }

            return Ok(fives);
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    [HttpGet("{id:int}", Name = "GetFiveById")]
    public IActionResult Get(int id)
    {
        try
        {
            var model = _repository.GetById(id);

            return Ok(model);
        }
        catch (InvalidOperationException)
        {
            return NotFound($"{id}번 데이터가 없습니다.");
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    [HttpPost]
    [Produces("application/json", Type = typeof(Five))]
    [Consumes("application/json")]
    public IActionResult Post([FromBody] Five? model)
    {
        try
        {
            if (model is null)
            {
                return BadRequest();
            }

            if (string.IsNullOrWhiteSpace(model.Note))
            {
                ModelState.AddModelError(
                    nameof(Five.Note),
                    "노트를 입력해야 합니다.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var addedModel = _repository.Add(model);

            if (DateTime.Now.Second % 2 == 0)
            {
                return CreatedAtRoute(
                    "GetFiveById",
                    new { id = addedModel.Id },
                    addedModel);
            }

            var uri = Url.Link(
                "GetFiveById",
                new { id = addedModel.Id });

            return Created(uri ?? string.Empty, addedModel);
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    [HttpPut("{id:int}")]
    public IActionResult Put(
        int id,
        [FromBody] Five? model)
    {
        if (model is null)
        {
            return BadRequest();
        }

        try
        {
            _repository.GetById(id);

            model.Id = id;
            _repository.Update(model);

            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound($"{id}번 데이터가 없습니다.");
        }
        catch (Exception)
        {
            return BadRequest(
                "데이터가 업데이트되지 않았습니다.");
        }
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        try
        {
            _repository.GetById(id);
            _repository.Remove(id);

            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound($"{id}번 데이터가 없습니다.");
        }
        catch (Exception)
        {
            return BadRequest("삭제할 수 없습니다.");
        }
    }

    [HttpGet("page/{pageNumber:int}/{pageSize:int}")]
    public IActionResult Get(
        int pageNumber = 1,
        int pageSize = 10)
    {
        try
        {
            // 페이지 번호는 외부에서 1, 2, 3을 사용하고
            // 리포지토리에서는 0, 1, 2를 사용합니다.
            pageNumber = pageNumber > 0
                ? pageNumber - 1
                : 0;

            var fives = _repository.GetAllWithPaging(
                pageNumber,
                pageSize);

            if (fives.Count == 0)
            {
                return NotFound("아무런 데이터가 없습니다.");
            }

            Response.Headers["X-TotalRecordCount"] =
                _repository.GetRecordCount().ToString();

            return Ok(fives);
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }
}