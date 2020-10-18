using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace DotNetNote.Models
{
    /// <summary>
    /// 모델 클래스
    /// </summary>
    public class Five
    {
        public int Id { get; set; }

        [Required]
        public string Note { get; set; }
    }

    /// <summary>
    /// 인터페이스 
    /// </summary>
    public interface IFiveRepository
    {
        Five Add(Five model);
        List<Five> GetAll();
        Five GetById(int id);
        Five Update(Five model);
        void Remove(int id);

        List<Five> GetAllWithPaging(int pageIndex, int pageSize = 10);
        int GetRecordCount();
    }

    /// <summary>
    /// 리파지터리 클래스
    /// </summary>
    public class FiveRepository : IFiveRepository
    {
        private IConfiguration _config;
        private IDbConnection db;

        public FiveRepository(IConfiguration config)
        {
            _config = config;
            db = new SqlConnection(
                _config.GetSection("ConnectionStrings")
                    .GetSection("DefaultConnection").Value);
        }

        /// <summary>
        /// 출력 메서드
        /// </summary>
        public List<Five> GetAll()
        {
            string sql = "Select * From Fives Order By Id Desc";
            return db.Query<Five>(sql).ToList();
        }

        /// <summary>
        /// 상세
        /// </summary>
        public Five GetById(int id)
        {
            string query = "Select * From Fives Where Id = @Id";
            return db.Query<Five>(query, new { Id = id }).Single();
        }

        /// <summary>
        /// 입력 메서드, Add, AddXXX, CreateXXX
        /// </summary>
        public Five Add(Five model)
        {
            string sql = @"
                Insert Into Fives (Note) Values (@Note);
                Select Cast(SCOPE_IDENTITY() As Int);
            ";
            var id = db.Query<int>(sql, model).Single();
            model.Id = id;
            return model;
        }

        /// <summary>
        /// 수정
        /// </summary>
        public Five Update(Five model)
        {
            var query =
                "Update Fives               " +
                "Set                        " +
                "   Note = @Note            " +
                "Where Id = @Id             ";
            db.Execute(query, model);
            return model;
        }

        /// <summary>
        /// 삭제
        /// </summary>
        public void Remove(int id)
        {
            var query = "Delete From Fives Where Id = @Id";
            db.Execute(query, new { Id = id });
        }

        /// <summary>
        /// 페이징 처리된 리스트
        /// </summary>
        public List<Five> GetAllWithPaging(
            int pageIndex, int pageSize = 10)
        {
            string sql = @"
                Select Id, Note
                From 
                    (
                        Select Row_Number() Over (Order By Id Desc) As RowNumbers, Id, Note
                        From Fives
                    ) As TempRowTables
                Where 
                    RowNumbers
                        Between
                            (@PageIndex * @PageSize + 1)
                        And
                            (@PageIndex + 1) * @PageSize
            ";
            return db.Query<Five>(sql, new { PageIndex = pageIndex, PageSize = pageSize }).ToList();
        }

        /// <summary>
        /// 레코드 카운트
        /// </summary>
        public int GetRecordCount()
        {
            string query = "Select Count(*) From Fives";
            return db.Query<int>(query).FirstOrDefault();
        }
    }

    // 컨벤션 기반 라우팅 대신에 어트리뷰트 라우팅 추천
    //[Route("api/fives")] // 직접 API 이름을 지정할 때
    [Route("api/[controller]")]
    public class FiveServiceController : Controller
    {
        private IFiveRepository _repository;

        public FiveServiceController(IFiveRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            // 500 에러 찍어보려면,
            // throw new Exception("인위적으로 에러 발생시켜 500에러 출력");

            try
            {
                var fives = _repository.GetAll();
                if (fives == null)
                {
                    return NotFound($"아무런 데이터가 없습니다.");
                }
                return Ok(fives); // 200 
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("{id:int}", Name = "GetFiveById")] // 이름 추가
        public IActionResult Get(int id)
        {
            try
            {
                var model = _repository.GetById(id);
                if (model == null)
                {
                    return NotFound($"{id}번 데이터가 없습니다.");
                }
                return Ok(model);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Produces("application/json", Type = typeof(Five))] // FiveForCreationDto 등으로 변경 사용 가능
        [Consumes("application/json")] // application/xml
        public IActionResult Post([FromBody]Five model) // Deserialize, 생성 전용 DTO 클래스 사용 가능 
        {
            try
            {
                // 예외 처리 방법
                if (model == null)
                {
                    return BadRequest(); // Status: 400 Bad Request 
                }

                // 예외 처리 방법 
                if (model.Note == null || model.Note.Length < 1)
                {
                    ModelState.AddModelError("Note", "노트를 입력해야 합니다.");
                }

                // 모델 유효성 검사
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState); // 400 에러 출력
                }

                // 데이터베이스에 저장
                var m = _repository.Add(model);

                if (DateTime.Now.Second % 2 == 0) //[!] 둘 중 원하는 방식 사용
                {
                    // GetById 액션 이름 사용해서 입력된 데이터 반환 
                    //return CreatedAtAction("GetById", new { id = m.Id }, m);
                    return CreatedAtRoute("GetFiveById", new { id = m.Id }, m); // Status: 201 Created
                }
                else
                {
                    var uri = Url.Link("GetFiveById", new { id = m.Id });
                    return Created(uri, m); // 201 Created
                }

                //return Ok(m); // 200
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut("{id:int}")] // HttpPatch == 부분 업데이트 
        public IActionResult Put(int id, [FromBody] Five model)
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
                model.Id = id; // * 
                _repository.Update(model);
                //return Ok(model);
                // 204 No Content
                return NoContent(); // 이미 던져준 정보에 모든 값 가지고 있기에...
            }
            catch (Exception)
            {
                return BadRequest("데이터가 업데이트되지 않았습니다.");
            }
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
                return NoContent();
            }
            catch (Exception)
            {
                return BadRequest("삭제할 수 없습니다.");
            }
        }

        [HttpGet("page/{pageNumber:int}/{pageSize:int}")] // 이름 추가
        public IActionResult Get(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                // 페이지 번호는 1, 2, 3 사용, 리파지터리에서는 0, 1, 2 사용
                pageNumber = (pageNumber > 0) ? pageNumber - 1 : 0;
                var fives = _repository.GetAllWithPaging(pageNumber, pageSize);
                if (fives == null)
                {
                    return NotFound($"아무런 데이터가 없습니다.");
                }

                // 응답 헤더에 총 레코드 수를 담아서 출력
                Response.Headers.Add(
                    "X-TotalRecordCount", _repository.GetRecordCount().ToString());

                return Ok(fives); // 200 
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
