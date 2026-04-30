namespace DotNetNote.Models;

using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// [1] 모델 클래스
/// </summary>
public class Four
{
    public int Id { get; set; }

    [Required]
    public string Note { get; set; } = string.Empty; // null 방지
}

/// <summary>
/// [2] 인터페이스 
/// </summary>
public interface IFourRepository
{
    Four Add(Four model);
    List<Four> GetAll();
    Four? GetById(int id); // null 가능
    Four Update(Four model);
    void Remove(int id);
}

/// <summary>
/// [3] 리포지토리 클래스 
/// </summary>
public class FourRepository : IFourRepository
{
    private readonly IConfiguration _config;
    private readonly IDbConnection db;

    public FourRepository(IConfiguration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));

        // ✅ 안전한 방식
        var connStr = _config.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connStr))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");
        }

        db = new SqlConnection(connStr);
    }

    public List<Four> GetAll()
    {
        string sql = "Select * From Fours Order By Id Asc";
        return db.Query<Four>(sql).ToList();
    }

    public Four Add(Four model)
    {
        string sql = @"
            Insert Into Fours (Note) Values (@Note);
            Select Cast(SCOPE_IDENTITY() As Int);
        ";

        var id = db.Query<int>(sql, model).Single();
        model.Id = id;
        return model;
    }

    public Four? GetById(int id)
    {
        string sql = "Select * From Fours Where Id = @Id";
        return db.QueryFirstOrDefault<Four>(sql, new { Id = id }); // null 가능
    }

    public Four Update(Four model)
    {
        var sql =
            "Update Fours " +
            "Set Note = @Note " +
            "Where Id = @Id";

        db.Execute(sql, model);
        return model;
    }

    public void Remove(int id)
    {
        string sql = "Delete From Fours Where Id = @Id";
        db.Execute(sql, new { Id = id });
    }
}

/// <summary>
/// [4] Web API 컨트롤러
/// </summary>
[Route("api/[controller]")]
public class FourServiceController : Controller
{
    private readonly IFourRepository _repository;

    public FourServiceController(IFourRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            var fours = _repository.GetAll();
            return Ok(fours);
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpPost]
    [Produces("application/json", Type = typeof(Four))]
    [Consumes("application/json")]
    public IActionResult Post([FromBody] Four model)
    {
        try
        {
            if (model == null)
            {
                return BadRequest();
            }

            if (string.IsNullOrWhiteSpace(model.Note) || model.Note.Length < 2)
            {
                ModelState.AddModelError("Note", "노트는 2자 이상 입력해야 합니다.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var m = _repository.Add(model);

            if (DateTime.Now.Second % 2 == 0)
            {
                return CreatedAtRoute("GetById", new { id = m.Id }, m);
            }
            else
            {
                var uri = Url.Link("GetById", new { id = m.Id });
                return Created(uri!, m); // null-forgiving
            }
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpGet("{id:int}", Name = "GetById")]
    public IActionResult Get(int id)
    {
        try
        {
            var model = _repository.GetById(id);

            if (model == null)
            {
                return NotFound($"{id} 데이터가 없습니다.");
            }

            return Ok(model);
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpPut("{id:int}")]
    public IActionResult Put(int id, [FromBody] Four model)
    {
        if (model == null)
        {
            return BadRequest();
        }

        try
        {
            var oldModel = _repository.GetById(id);

            if (oldModel == null)
            {
                return NotFound($"{id}번 데이터가 없습니다.");
            }

            model.Id = id;
            _repository.Update(model);

            return NoContent();
        }
        catch
        {
            return BadRequest("데이터가 업데이트되지 않았습니다.");
        }
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        try
        {
            var oldModel = _repository.GetById(id);

            if (oldModel == null)
            {
                return NotFound($"{id}번 데이터가 없습니다.");
            }

            _repository.Remove(id);
            return NoContent();
        }
        catch
        {
            return BadRequest("삭제할 수 없습니다.");
        }
    }
}