namespace DotNetNote.Models.Exams;

public class ExamClass
{
}

/// <summary>
/// [1] 모델 클래스
/// </summary>
public class Question
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;
}

/// <summary>
/// [2] 인터페이스
/// </summary>
public interface IQuestionRepository
{
    Question Add(Question model);

    List<Question> GetAll();

    Question GetById(int id);

    Question Update(Question model);

    void Remove(int id);

    List<Question> GetAllWithPaging(
        int pageIndex,
        int pageSize = 10);

    int GetRecordCount();
}

/// <summary>
/// [3] 리포지토리 클래스
/// </summary>
public class QuestionRepository : IQuestionRepository
{
    private readonly IDbConnection _db;

    public QuestionRepository(IConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(config);

        var connectionString =
            config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "DefaultConnection 연결 문자열이 설정되지 않았습니다.");

        _db = new SqlConnection(connectionString);
    }

    /// <summary>
    /// 입력 메서드
    /// </summary>
    public Question Add(Question model)
    {
        ArgumentNullException.ThrowIfNull(model);

        const string sql = """
            INSERT INTO Questions (Title)
            VALUES (@Title);

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        var id = _db.Query<int>(sql, model).Single();

        model.Id = id;

        return model;
    }

    /// <summary>
    /// 출력 메서드
    /// </summary>
    public List<Question> GetAll()
    {
        const string sql = """
            SELECT Id, Title
            FROM Questions
            ORDER BY Id DESC
            """;

        return _db.Query<Question>(sql).ToList();
    }

    /// <summary>
    /// 상세 메서드
    /// </summary>
    public Question GetById(int id)
    {
        const string sql = """
            SELECT Id, Title
            FROM Questions
            WHERE Id = @Id
            """;

        return _db.Query<Question>(
            sql,
            new { Id = id }).Single();
    }

    /// <summary>
    /// 수정 메서드
    /// </summary>
    public Question Update(Question model)
    {
        ArgumentNullException.ThrowIfNull(model);

        const string sql = """
            UPDATE Questions
            SET Title = @Title
            WHERE Id = @Id
            """;

        _db.Execute(sql, model);

        return model;
    }

    /// <summary>
    /// 삭제 메서드
    /// </summary>
    public void Remove(int id)
    {
        const string sql = """
            DELETE FROM Questions
            WHERE Id = @Id
            """;

        _db.Execute(sql, new { Id = id });
    }

    /// <summary>
    /// 레코드 카운트 반환 메서드
    /// </summary>
    public int GetRecordCount()
    {
        const string sql = """
            SELECT COUNT(*)
            FROM Questions
            """;

        return _db.Query<int>(sql).FirstOrDefault();
    }

    /// <summary>
    /// 페이징 처리된 리스트 출력 메서드
    /// </summary>
    public List<Question> GetAllWithPaging(
        int pageIndex,
        int pageSize = 10)
    {
        const string sql = """
            SELECT Id, Title
            FROM
            (
                SELECT
                    ROW_NUMBER() OVER (ORDER BY Id DESC) AS RowNumbers,
                    Id,
                    Title
                FROM Questions
            ) AS TempRowTables
            WHERE RowNumbers
                BETWEEN (@PageIndex * @PageSize + 1)
                AND ((@PageIndex + 1) * @PageSize)
            """;

        return _db.Query<Question>(
            sql,
            new
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            }).ToList();
    }
}

/// <summary>
/// DTO 클래스
/// </summary>
public class QuestionDto
{
    public int Id { get; set; }

    [Required]
    [MaxLength(
        4000,
        ErrorMessage = "문제는 4000자 이하로 입력하세요.")]
    public string Title { get; set; } = string.Empty;
}

/// <summary>
/// [4] Web API 컨트롤러 클래스
/// </summary>
[Route("api/[controller]")]
public class QuestionServiceController : Controller
{
    private readonly IQuestionRepository _repository;
    private readonly ILogger<QuestionServiceController> _logger;

    public QuestionServiceController(
        IQuestionRepository repository,
        ILogger<QuestionServiceController> logger)
    {
        _repository = repository
            ?? throw new ArgumentNullException(nameof(repository));

        _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    [Route("")]
    [ProducesResponseType(typeof(List<Question>), 200)]
    public IActionResult Get()
    {
        try
        {
            var models = _repository.GetAll();

            if (models.Count == 0)
            {
                return NotFound("아무런 데이터가 없습니다.");
            }

            return Ok(models);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "질문 목록을 가져오는 중 오류가 발생했습니다.");

            return BadRequest();
        }
    }

    [HttpGet("{id:int}", Name = "GetQuestionById")]
    [ProducesResponseType(typeof(Question), 200)]
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
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "{QuestionId}번 질문을 가져오는 중 오류가 발생했습니다.",
                id);

            return BadRequest("질문을 가져오는 중 오류가 발생했습니다.");
        }
    }

    [HttpPost]
    [Produces("application/json", Type = typeof(QuestionDto))]
    [Consumes("application/json")]
    public IActionResult Post([FromBody] QuestionDto? model)
    {
        if (model is null)
        {
            return BadRequest();
        }

        if (string.IsNullOrWhiteSpace(model.Title))
        {
            ModelState.AddModelError(
                nameof(QuestionDto.Title),
                "문제를 입력해야 합니다.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var newModel = new Question
            {
                Id = model.Id,
                Title = model.Title
            };

            var addedModel = _repository.Add(newModel);

            if (DateTime.Now.Second % 2 == 0)
            {
                return CreatedAtRoute(
                    "GetQuestionById",
                    new { id = addedModel.Id },
                    addedModel);
            }

            var uri = Url.Link(
                "GetQuestionById",
                new { id = addedModel.Id });

            return Created(uri ?? string.Empty, addedModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "질문을 등록하는 중 오류가 발생했습니다.");

            return BadRequest();
        }
    }

    [HttpPut("{id:int}")]
    public IActionResult Put(
        int id,
        [FromBody] Question? model)
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
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "{QuestionId}번 질문을 수정하는 중 오류가 발생했습니다.",
                id);

            return BadRequest("데이터가 업데이트되지 않았습니다.");
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
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "{QuestionId}번 질문을 삭제하는 중 오류가 발생했습니다.",
                id);

            return BadRequest("삭제할 수 없습니다.");
        }
    }

    [HttpGet("page/{pageNumber:int}/{pageSize:int}")]
    [ProducesResponseType(typeof(IEnumerable<Question>), 200)]
    public IActionResult Get(
        int pageNumber = 1,
        int pageSize = 10)
    {
        try
        {
            pageNumber = pageNumber > 0
                ? pageNumber - 1
                : 0;

            var models = _repository.GetAllWithPaging(
                pageNumber,
                pageSize);

            if (models.Count == 0)
            {
                return NotFound("아무런 데이터가 없습니다.");
            }

            Response.Headers["X-TotalRecordCount"] =
                _repository.GetRecordCount().ToString();

            return Ok(models);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "질문 페이징 목록을 가져오는 중 오류가 발생했습니다.");

            return BadRequest();
        }
    }
}