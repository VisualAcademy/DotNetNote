namespace DotNetNote.Models;

/// <summary>
/// [1] 모델 클래스
/// </summary>
public class Four
{
    public int Id { get; set; }

    [Required]
    public string Note { get; set; }
}

/// <summary>
/// [2] 인터페이스 
/// </summary>
public interface IFourRepository
{
    Four Add(Four model);
    List<Four> GetAll();
    Four GetById(int id);
    Four Update(Four model);
    void Remove(int id);
}

/// <summary>
/// [3] 리포지토리 클래스 
/// </summary>
public class FourRepository : IFourRepository
{
    private IConfiguration _config;
    private IDbConnection db;

    public FourRepository(IConfiguration config)
    {
        _config = config;
        db = new SqlConnection(
            _config
                .GetSection("ConnectionStrings")
                    .GetSection("DefaultConnection").Value);
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

    public Four GetById(int id)
    {
        string sql = "Select * From Fours  Where Id = @Id";
        return db.Query<Four>(sql, new { Id = id }).Single();
    }

    public Four Update(Four model)
    {
        var sql =
            "Update Fours                           " +
            "Set                                    " +
            "    Note       =       @Note           " +
            "Where Id = @Id                         ";
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
/// [4] Web API 컨트롤러 클래스: Angular / jQuery 등의 프론트엔드에서 접근할 Web API
/// </summary>
// 컨벤션 기반 라우팅 대신에 어트리뷰트 라우팅 추천 
//[Route("api/fours")] // 직접 API 이름을 지정할 때 
[Route("api/[controller]")]
public class FourServiceController : Controller
{
    private IFourRepository _repository;

    public FourServiceController(IFourRepository repository) => _repository = repository;

    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            var fours = _repository.GetAll();
            if (fours == null)
            {
                return NotFound($"아무런 데이터가 없습니다.");
            }
            return Ok(fours); // 200
        }
        catch
        {
            return BadRequest();
        }
    }


    [HttpPost]
    [Produces("application/json", Type = typeof(Four))]
    [Consumes("application/json")] // application/xml 
    public IActionResult Post([FromBody]Four model)
    {
        try
        {
            if (model.Note == null || model.Note.Length < 2)
            {
                ModelState.AddModelError("Note", "노트는 2자 이상 입력해야 합니다.");
            }

            // 모델 유효성 검사
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // 400 에러 출력
            }

            var m = _repository.Add(model);

            if (DateTime.Now.Second % 2 == 0) //[!] 둘 중 원하는 방식 사용
            {
                //return CreatedAtAction("GetById", new { id = m.Id }, m); // 201
                return CreatedAtRoute("GetById", new { id = m.Id }, m);
            }
            else
            {
                var uri = Url.Link("GetById", new { id = m.Id });
                return Created(uri, m); // 201
            }
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpGet("{id:int}", Name = "GetById")] // TODO: 이름 추가 
    //public Four Get(int id)
    //{
    //    return _repository.GetAll().Where(m => m.Id == id).Single();
    //}
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

    [HttpGet("{pageIndex:int}/{pageSize}")] // TODO: 이름 추가 
    public IActionResult Get(int pageIndex, int pageSize)
    {
        return Ok();
    }


    [HttpPut("{id:int}")] // HttpPatch == 부분 업데이트 
    public IActionResult Put(int id, [FromBody] Four model)
    {
        if (model == null)
        {
            BadRequest();
        }
        try
        {
            var oldModel = _repository.GetById(id);
            if (oldModel == null)
            {
                return NotFound($"{id}번 데이터가 없습니다.");
            }
            model.Id = id; // * 
            _repository.Update(model);
            //return Ok(model);
            return NoContent(); // 이미 던져준 정보에 모든 값 가지고 있기에
        }
        catch (Exception)
        {
        }

        return BadRequest("데이터가 업데이트되지 않았습니다.");
    }


    [HttpDelete("{id:int}")] // 데코레이터 특성
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
            //return Ok(); 
            return NoContent();
        }
        catch (Exception)
        {
            //return NotFound("실패했습니다.");
            return BadRequest("삭제할 수 없습니다.");
        }
    }
}
