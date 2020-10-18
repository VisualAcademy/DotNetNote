using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Cors;

namespace DotNetNote.Models
{
    /// <summary>
    /// [1] 모델 클래스
    /// Heroes 테이블과 일대일로 매핑되는 모델 클래스: 
    /// Heroes, HeroesModel, HeroesViewModel, HeroesBase, HeroesDto, HeroesEntity, ...
    /// </summary>
    public class Hero
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public DateTimeOffset Created { get; set; }
    }

    /// <summary>
    /// [2] 인터페이스 
    /// Heroes 테이블에 대한 CRUD API 명세서 작성
    /// </summary>
    public interface IHeroRepository
    {
        Hero AddHero(Hero model);       // 입력: T Add(T model);
        List<Hero> GetAllHeroes();      // 출력: List<T> GetAll();
        Hero GetHeroById(int id);       // 상세: T GetById(int id);
        Hero UpdateHero(Hero model);    // 수정: T Update(T model);
        void RemoveHero(int id);        // 삭제: void Remove(int id); 

        int GetRecordCountHeroes();
        List<Hero> GetAllHeroesWithPaging(int pageIndex, int pageSize = 10);

        // 추가: 이미 등록된 영웅 이름인지 확인
        Hero GetHeroByName(string name);  
    }

    /// <summary>
    /// [3] 리포지토리 클래스
    /// 인터페이스 실제 내용 구현
    /// </summary>
    public class HeroRepository : IHeroRepository
    {
        private IConfiguration _config;
        private IDbConnection db;

        /// <summary>
        /// 생성자 
        /// </summary>
        public HeroRepository(IConfiguration config)
        {
            _config = config;
            string connectionString = _config.GetSection("ConnectionString").Value;
            //db = new SqlConnection(_config.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
            db = new SqlConnection(connectionString); 
        }

        /// <summary>
        /// 입력 메서드: Add, AddHero, Write, Persist, Create
        /// </summary>
        public Hero AddHero(Hero model)
        {
            string sql = @"
                Insert Into Heroes (Name, Icon) Values (@Name, @Icon);
                Select Cast(SCOPE_IDENTITY() As Int);
            ";
            var id = db.Query<int>(sql, model).Single();
            model.Id = id;
            return model;
        }

        /// <summary>
        /// 출력 메서드: GetAll, GetAllHeroes, GetHeroes
        /// </summary>
        public List<Hero> GetAllHeroes()
        {
            string sql = "Select * From Heroes Order By Id Desc";
            return db.Query<Hero>(sql).ToList();
        }

        /// <summary>
        /// 상세 메서드: GetById, GetHeroById, GetHero, FindHero()
        /// </summary>
        public Hero GetHeroById(int id)
        {
            string query = "Select * From Heroes Where Id = @Id";
            return db.Query<Hero>(query, new { Id = id }).SingleOrDefault(); // null 반환 허용
        }

        /// <summary>
        /// 수정 메서드: Update, UpdateHero, Edit, Modify
        /// </summary>
        public Hero UpdateHero(Hero model)
        {
            var query =
                "Update Heroes          " +
                "Set                    " +
                "   Name = @Name,       " +
                "   Icon = @Icon        " +
                "Where Id = @Id         ";
            db.Execute(query, model);
            return model;
        }

        /// <summary>
        /// 삭제 메서드: Remove, RemoveHero, Delete
        /// </summary>
        /// <param name="id"></param>
        public void RemoveHero(int id)
        {
            var query = "Delete From Heroes Where Id = @Id";
            db.Execute(query, new { Id = id });
        }

        /// <summary>
        /// 카운트 메서드: 레코드 카운트 반환 메서드
        /// </summary>
        public int GetRecordCountHeroes()
        {
            string query = "Select Count(*) From Heroes";
            return db.Query<int>(query).FirstOrDefault();
        }

        /// <summary>
        /// 페이징 메서드: 페이징 처리된 리스트 출력 메서드
        /// (카운트 메서드 + 페이징 메서드) 형태는 GoodsManager 강좌를 참고할 것
        /// </summary>
        /// <param name="pageIndex">0, 1, 2, ...</param>
        /// <param name="pageSize">한 페이지에 표시하 레코드 수</param>
        public List<Hero> GetAllHeroesWithPaging(int pageIndex, int pageSize = 10)
        {
            string sql = @"
                Select Id, Name, Icon, Created
                From
                    (
                        Select Row_Number() Over (Order By Id Desc) As RowNumbers, Id, Name, Icon, Created
                        From Heroes
                    ) As TempRowTables 
                Where
                    RowNumbers
                        Between
                            (@PageIndex * @PageSize + 1) 
                        And
                            (@PageIndex + 1) * @PageSize
            ";
            return db.Query<Hero>(
                sql, new { PageIndex = pageIndex, PageSize = pageSize }).ToList();
        }

        /// <summary>
        /// 이름 확인
        /// </summary>
        public Hero GetHeroByName(string name)
        {
            string query = "Select * From Heroes Where Name = @Name";
            return db.Query<Hero>(query, new { Name = name }).FirstOrDefault(); // 이미 '홍길동'이 여러개 들어가 있음
        }
    }

    /// <summary>
    /// 컨트롤러 클래스: ASP.NET Core MVC 파트  
    /// </summary>
    public class HeroController : Controller
    {
        private IHeroRepository _repository;
        private ILogger<HeroController> _logger;

        // 인터페이스를 통한 생성자 주입 방식 사용: 
        //     Startup.cs에서 services.AddTransient로 등록됨
        public HeroController(IHeroRepository repository, ILogger<HeroController> logger)
        {
            _repository = repository;
            _logger = logger; 
        }

        /// <summary>
        /// 리스트 
        /// </summary>
        public IActionResult Index()
        {
            // 모든 데이터 출력
            var heroes = _repository.GetAllHeroes();
            return View(heroes);
        }

        /// <summary>
        /// 입력 폼
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 입력 처리
        /// </summary>
        [HttpPost]
        public IActionResult Create(string name, string icon)
        {
            var hero = new Hero() { Name = name, Icon = icon };
            _repository.AddHero(hero);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// 상세 보기 
        /// </summary>
        public IActionResult Details(int id)
        {
            var hero = _repository.GetHeroById(id);
            return View(hero);
        }

        /// <summary>
        /// 수정 처리
        /// </summary>
        [HttpPost]
        public IActionResult Edit(int id, string name, string icon)
        {
            var hero = new Hero() { Id = id, Name = name, Icon = icon };
            _repository.UpdateHero(hero);
            return RedirectToAction(nameof(Details), new { Id = id });
        }

        /// <summary>
        /// 삭제 처리
        /// </summary>
        [HttpGet]
        public IActionResult Delete(int id)
        {
            _repository.RemoveHero(id);
            _logger.LogInformation($"{id} 자료가 삭제되었습니다.");
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Web API 컨트롤러 클래스: Angular / jQuery 등의 프론트엔드에서 접근할 Web API
    /// 복수형 또는 Services 등의 접미사 사용 권장
    /// 컨벤션 기반 라우팅 대신에 특성(어트리뷰트) 라우팅 추천
    /// </summary>
    //[Route("api/heroes")] // 직접 Web API 이름을 지정할 때
    //@Path("/api/Heroes") : Java EE 참고용으로 기록
    [Route("api/[controller]")] // /api/heroes 
    [EnableCors("AllowAnyOrigin")]
    public class HeroesController : Controller
    {
        // @Inject
        private IHeroRepository _repository;
        private ILogger<HeroesController> _logger;

        public HeroesController(IHeroRepository repository, ILogger<HeroesController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// GET: /api/heroes
        /// </summary>
        //@Get
        //Produces(APPLICATION_JSON) 
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(List<Hero>), 200)]
        public IActionResult Get()
        {
            // 500 에러 찍어보려면,
            // throw new Exception("인위적으로 에러 발생시켜 500에러 출력");
            try
            {
                var models = _repository.GetAllHeroes();
                if (models == null)
                {
                    return NotFound("아무런 데이터가 없습니다.");
                }
                return Ok(models); // 200 OK 
            }
            catch (Exception ex)
            {
                _logger.LogError($"에러 발생: {ex.Message}");
                return BadRequest();
            }
        }
        
        /// <summary>
        /// GET: /api/heroes/{id}
        /// </summary>
        [HttpGet("{id:int}", Name = "GetHeroById")] // 이름 추가: Name 속성은 프로젝트에서 유일한 값 지정
        [ProducesResponseType(typeof(Hero), 200)]
        public IActionResult Get(int id)
        {
            try
            {
                var model = _repository.GetHeroById(id);
                if (model == null)
                {
                    return NotFound($"{id}번 데이터가 없습니다.");
                }
                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"HeroesController - GetById 에러 발생: {ex.Message}");
                return BadRequest($"에러가 발생했습니다. {ex.Message}");
            }
        }

        /// <summary>
        /// POST: /api/heroes
        /// </summary>
        [HttpPost]
        [Produces("application/json", Type = typeof(Hero))]
        [Consumes("application/json")] // application/xml 
        public IActionResult Post([FromBody]Hero model) // Deserialize, 생성 전용 DTO 클래스 사용 가능
        {
            // 예외 처리 방법
            if (model == null)
            {
                return BadRequest(); // Status: 400 Bad Request
            }

            try
            {
                // 예외 처리
                if (model.Name == null || model.Name.Length < 1)
                {
                    ModelState.AddModelError("Name", "이름을 입력해야 합니다.");
                }

                // 모델 유효성 검사
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState); // 400 에러 출력
                }

                var m = _repository.AddHero(model); // 저장 

                if (DateTime.Now.Second % 2 == 0) //[!] 2가지 방식 중 원하는 방식 사용
                {
                    //return CreatedAtAction("GetHeroById", new { id = m.Id }, m);
                    return CreatedAtRoute("GetHeroById", new { id = m.Id }, m); // Status: 201 Created
                }
                else
                {
                    var uri = Url.Link("GetHeroById", new { id = m.Id });
                    return Created(uri, m); // 201 Created
                }
                //return Ok(m); // 200 OK
            }
            catch (Exception ex)
            {
                _logger.LogError($"HeroesController - Post 에러 발생: {ex.Message}");
                return BadRequest($"에러가 발생했습니다. {ex.Message}");
            }
        } // </Post>

        // PUT: /api/heroes/{id}
        [HttpPut("{id:int}")] // HttpPatch == 부분 업데이트 
        public IActionResult Put(int id, [FromBody]Hero model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            try
            {
                var oldModel = _repository.GetHeroById(id);
                if (oldModel == null)
                {
                    return NotFound($"{id}번 데이터가 없습니다.");
                }
                model.Id = id; // *
                _repository.UpdateHero(model); // 수정
                //return Ok(model);
                // 204 No Content
                return NoContent(); // 이미 넘어온 정보에 모든 값을 가지고 있기에...
            }
            catch (Exception ex)
            {
                _logger.LogError($"HeroesController - Put 에러 발생: {ex.Message}");
                return BadRequest($"데이터가 업데이트되지 않았습니다. {ex.Message}");
            }
        } // </Put>

        // DELETE: /api/heroes/{id}
        [HttpDelete("{id:int}")] // 데코레이터 특성
        public IActionResult Delete(int id)
        {
            try
            {
                var oldModel = _repository.GetHeroById(id);
                if (oldModel == null)
                {
                    return NotFound($"{id}번 데이터가 없습니다.");
                }

                // 삭제 
                _repository.RemoveHero(id);

                return NoContent(); // 204 No Content 
            }
            catch (Exception ex)
            {
                _logger.LogError($"HeroesController - Delete 에러 발생: {ex.Message}");
                return BadRequest($"삭제할 수 없습니다. {ex.Message}");
            }
        } // </Delete> 

        // 페이징 처리 Web API
        // GET: /api/heroes/page/1/10
        [HttpGet("page/{pageNumber:int}/{pageSize:int}")]
        [ProducesResponseType(typeof(IEnumerable<Hero>), 200)]
        public IActionResult Get(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                // 페이지 번호는 1, 2, 3 사용, 리포지토리에서는 0, 1, 2 사용
                int pageIndex = (pageNumber > 0) ? pageNumber - 1 : 0;
                var models = _repository.GetAllHeroesWithPaging(pageIndex, pageSize);
                if (models == null)
                {
                    return NotFound("아무런 데이터가 없습니다.");
                }

                // 응답 헤더에 총 레코드 수를 담아서 출력 
                Response.Headers.Add(
                    "X-TotalRecordCount", _repository.GetRecordCountHeroes().ToString());

                return Ok(models); // 200 OK 
            }
            catch (Exception ex)
            {
                _logger.LogError($"HeroesController - GetByPaging 에러 발생: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{name}", Name = "GetHeroByName")] // 이름 추가: Name 속성은 프로젝트에서 유일한 값 지정
        [ProducesResponseType(typeof(Hero), 200)]
        public IActionResult Get(string name)
        {
            try
            {
                var model = _repository.GetHeroByName(name);
                if (model == null)
                {
                    return Ok(new Hero());
                }
                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"HeroesController - GetByName 에러 발생: {ex.Message}");
                return BadRequest($"에러가 발생했습니다. {ex.Message}");
            }
        }
    }
}
