using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace DotNetNote.Models.Exams
{
    public class ExamClass
    {

    }

    /// <summary>
    /// 모델 클래스
    /// </summary>
    public class Question
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
    }

    /// <summary>
    /// 인터페이스 
    /// </summary>
    public interface IQuestionRepository
    {
        Question Add(Question model);
        List<Question> GetAll();
        Question GetById(int id);
        Question Update(Question model);
        void Remove(int id);

        List<Question> GetAllWithPaging(int pageIndex, int pageSize = 10);
        int GetRecordCount();
    }

    /// <summary>
    /// 리포지토리 클래스 
    /// </summary>
    public class QuestionRepository : IQuestionRepository
    {
        private IConfiguration _config;
        private IDbConnection db;

        /// <summary>
        /// 생성자
        /// </summary>
        public QuestionRepository(IConfiguration config)
        {
            _config = config;
            db = new SqlConnection(
                _config.GetSection("ConnectionStrings")
                    .GetSection("DefaultConnection").Value);
        }

        /// <summary>
        /// 입력 메서드 
        /// </summary>
        public Question Add(Question model)
        {
            string sql = @"
                Insert Into Questions (Title) Values (@Title);
                Select Cast(SCOPE_IDENTITY() As Int);
            ";
            var id = db.Query<int>(sql, model).Single();
            model.Id = id;
            return model;
        }

        /// <summary>
        /// 출력 메서드: GetAll, GetQuestions 
        /// </summary>
        public List<Question> GetAll()
        {
            string sql = "Select * From Questions Order By Id Desc";
            return db.Query<Question>(sql).ToList();
        }

        /// <summary>
        /// 상세 메서드: GetById, GetQuestion, GetQuestionById
        /// </summary>
        public Question GetById(int id)
        {
            string query = "Select * From Questions Where Id = @Id";
            return db.Query<Question>(query, new { Id = id }).Single();
        }

        /// <summary>
        /// 수정 메서드 
        /// </summary>
        public Question Update(Question model)
        {
            var query =
                "Update Questions           " +
                "Set                        " +
                "   Title = @Title          " +
                "Where Id = @Id             ";
            db.Execute(query, model);
            return model;
        }

        /// <summary>
        /// 삭제 메서드
        /// </summary>
        public void Remove(int id)
        {
            var query = "Delete From Questions Where Id = @Id";
            db.Execute(query, new { Id = id });
        }

        /// <summary>
        /// 레코드 카운트 반환 메서드
        /// </summary>
        public int GetRecordCount()
        {
            string query = "Select Count(*) From Questions";
            return db.Query<int>(query).FirstOrDefault();
        }

        /// <summary>
        /// 페이징 처리된 리스트 출력 메서드
        /// </summary>
        public List<Question> GetAllWithPaging(int pageIndex, int pageSize = 10)
        {
            string sql = @"
                Select Id, Title
                From 
                    (
                        Select Row_Number() Over (Order By Id Desc) As RowNumbers, Id, Title
                        From Questions
                    ) As TempRowTables
                Where 
                    RowNumbers
                        Between
                            (@PageIndex * @PageSize + 1)
                        And
                            (@PageIndex + 1) * @PageSize
            ";
            return db.Query<Question>(sql, new { PageIndex = pageIndex, PageSize = pageSize }).ToList();
        }
    }


    /// <summary>
    /// DTO 클래스
    /// </summary>
    public class QuestionDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(4000, ErrorMessage = "문제는 4000자 이하로 입력하세요.")]
        public string Title { get; set; }
    }

    /// <summary>
    /// Web API 컨트롤러 클래스
    /// 복수형 사용 권장 
    /// 컨벤션 기반 라우팅 대신에 특성(어트리뷰트) 라우팅 추천
    /// </summary>
    //[Route("api/questions")] // 직접 Web API 이름을 지정할 때
    [Route("api/[controller]")] // /api/QuestionService 
    public class QuestionServiceController : Controller
    {
        private IQuestionRepository _repository;
        private ILogger<QuestionServiceController> _logger;

        public QuestionServiceController(IQuestionRepository repository, ILogger<QuestionServiceController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // GET: /api/QuestionService 
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(List<Question>), 200)]
        public IActionResult Get()
        {
            // 500 에러 찍어보려면,
            // throw new Exception("인위적으로 에러 발생시켜 500에러 출력");
            try
            {
                var models = _repository.GetAll();
                if (models == null)
                {
                    return NotFound("아무런 데이터가 없습니다.");
                }
                return Ok(models); // 200 
            }
            catch (Exception ex)
            {
                _logger.LogError($"에러 발생: {ex.Message}");
                return BadRequest();
            }
        }

        // GET: /api/QuestionService/{id} 
        [HttpGet("{id:int}", Name = "GetQuestionById")] // 이름 추가
        [ProducesResponseType(typeof(Question), 200)]
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
            catch (Exception ex)
            {
                return BadRequest($"에러가 발생했습니다. {ex.Message}");
            }
        }

        [HttpPost]
        [Produces("application/json", Type = typeof(QuestionDto))]
        [Consumes("application/json")] // application/xml
        public IActionResult Post([FromBody]QuestionDto model) // Deserialize, 생성 전용 DTO 클래스 사용 가능 
        {
            // 예외 처리 방법
            if (model == null)
            {
                return BadRequest(); // Status: 400 Bad Request 
            }

            try
            {
                // 예외 처리 
                if (model.Title == null || model.Title.Length < 1)
                {
                    ModelState.AddModelError("Title", "문제를 입력해야 합니다.");
                }

                // 모델 유효성 검사
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState); // 400 에러 출력
                }

                // QuestionDto를 Question 모델로 변경해서 리포지토리에 전달
                // AutoMapper Mapper.Map() 형식으로 대체 가능 
                var newModel = new Question { Id = model.Id, Title = model.Title };

                var m = _repository.Add(newModel); // 저장

                if (DateTime.Now.Second % 2 == 0) //[!] 둘 중 원하는 방식 사용
                {
                    //return CreatedAtAction("GetById", new { id = m.Id }, m);
                    return CreatedAtRoute("GetQuestionById", new { id = m.Id }, m); // Status: 201 Created
                }
                else
                {
                    var uri = Url.Link("GetQuestionById", new { id = m.Id });
                    return Created(uri, m); // 201 Created
                }
                //return Ok(m); // 200
            }
            catch (Exception)
            {
                return BadRequest();
            }
        } // </Post>

        // PUT: /api/QuestionService 
        [HttpPut("{id:int}")] // HttpPatch == 부분 업데이트 
        public IActionResult Put(int id, [FromBody] Question model)
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
                return NoContent(); // 이미 넘어온 정보에 모든 값을 가지고 있기에...
            }
            catch (Exception)
            {
                return BadRequest("데이터가 업데이트되지 않았습니다.");
            }
        }

        // DELETE: /api/QuestionService/{id} 
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

                // 삭제
                _repository.Remove(id);

                return NoContent();
            }
            catch (Exception)
            {
                return BadRequest("삭제할 수 없습니다.");
            }
        }

        // 페이징 처리 Web API 
        // GET: /api/QuestionService/page/1/1
        [HttpGet("page/{pageNumber:int}/{pageSize:int}")] // 이름 추가
        [ProducesResponseType(typeof(IEnumerable<Question>), 200)]
        public IActionResult Get(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                // 페이지 번호는 1, 2, 3 사용, 리포지토리에서는 0, 1, 2 사용
                pageNumber = (pageNumber > 0) ? pageNumber - 1 : 0;
                var models = _repository.GetAllWithPaging(pageNumber, pageSize);
                if (models == null)
                {
                    return NotFound($"아무런 데이터가 없습니다.");
                }

                // 응답 헤더에 총 레코드 수를 담아서 출력
                Response.Headers.Add(
                    "X-TotalRecordCount", _repository.GetRecordCount().ToString());

                return Ok(models); // 200 
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
