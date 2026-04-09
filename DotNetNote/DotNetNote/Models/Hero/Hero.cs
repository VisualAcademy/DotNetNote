using System.Data;
using Dapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

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

        public string Name { get; set; } = string.Empty;

        public string Icon { get; set; } = string.Empty;

        public DateTimeOffset Created { get; set; }
    }

    /// <summary>
    /// [2] 인터페이스
    /// Heroes 테이블에 대한 CRUD API 명세서 작성
    /// </summary>
    public interface IHeroRepository
    {
        Hero AddHero(Hero model);               // 입력: T Add(T model);

        List<Hero> GetAllHeroes();              // 출력: List<T> GetAll();

        Hero? GetHeroById(int id);              // 상세: T? GetById(int id);

        Hero UpdateHero(Hero model);            // 수정: T Update(T model);

        void RemoveHero(int id);                // 삭제: void Remove(int id);

        int GetRecordCountHeroes();

        List<Hero> GetAllHeroesWithPaging(int pageIndex, int pageSize = 10);

        // 추가: 이미 등록된 영웅 이름인지 확인
        Hero? GetHeroByName(string name);
    }

    /// <summary>
    /// [3] 리포지토리 클래스
    /// 인터페이스 실제 내용 구현
    /// </summary>
    public class HeroRepository : IHeroRepository
    {
        private readonly IConfiguration _config;
        private readonly IDbConnection db;

        /// <summary>
        /// 생성자
        /// </summary>
        public HeroRepository(IConfiguration config)
        {
            _config = config;

            string connectionString =
                _config.GetSection("ConnectionString").Value
                ?? _config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("A database connection string was not found.");

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
            const string sql = "Select * From Heroes Order By Id Desc";
            return db.Query<Hero>(sql).ToList();
        }

        /// <summary>
        /// 상세 메서드: GetById, GetHeroById, GetHero, FindHero()
        /// </summary>
        public Hero? GetHeroById(int id)
        {
            const string query = "Select * From Heroes Where Id = @Id";
            return db.Query<Hero>(query, new { Id = id }).SingleOrDefault();
        }

        /// <summary>
        /// 수정 메서드: Update, UpdateHero, Edit, Modify
        /// </summary>
        public Hero UpdateHero(Hero model)
        {
            const string query =
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
        public void RemoveHero(int id)
        {
            const string query = "Delete From Heroes Where Id = @Id";
            db.Execute(query, new { Id = id });
        }

        /// <summary>
        /// 카운트 메서드: 레코드 카운트 반환 메서드
        /// </summary>
        public int GetRecordCountHeroes()
        {
            const string query = "Select Count(*) From Heroes";
            return db.Query<int>(query).FirstOrDefault();
        }

        /// <summary>
        /// 페이징 메서드: 페이징 처리된 리스트 출력 메서드
        /// (카운트 메서드 + 페이징 메서드) 형태는 GoodsManager 강좌를 참고할 것
        /// </summary>
        /// <param name="pageIndex">0, 1, 2, ...</param>
        /// <param name="pageSize">한 페이지에 표시하는 레코드 수</param>
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
                sql,
                new { PageIndex = pageIndex, PageSize = pageSize }).ToList();
        }

        /// <summary>
        /// 이름 확인
        /// </summary>
        public Hero? GetHeroByName(string name)
        {
            const string query = "Select * From Heroes Where Name = @Name";
            return db.Query<Hero>(query, new { Name = name }).FirstOrDefault();
        }
    }

    /// <summary>
    /// [4] 컨트롤러 클래스: ASP.NET Core MVC 파트
    /// </summary>
    public class HeroController : Controller
    {
        private readonly IHeroRepository _repository;
        private readonly ILogger<HeroController> _logger;

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
            var hero = new Hero
            {
                Name = name ?? string.Empty,
                Icon = icon ?? string.Empty
            };

            _repository.AddHero(hero);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// 상세 보기
        /// </summary>
        public IActionResult Details(int id)
        {
            var hero = _repository.GetHeroById(id);
            if (hero is null)
            {
                return NotFound();
            }

            return View(hero);
        }

        /// <summary>
        /// 수정 처리
        /// </summary>
        [HttpPost]
        public IActionResult Edit(int id, string name, string icon)
        {
            var hero = new Hero
            {
                Id = id,
                Name = name ?? string.Empty,
                Icon = icon ?? string.Empty
            };

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
            _logger.LogInformation("{Id} 자료가 삭제되었습니다.", id);
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// [5] Web API 컨트롤러 클래스: Angular / jQuery 등의 프론트엔드에서 접근할 Web API
    /// 복수형 또는 Services 등의 접미사 사용 권장
    /// 컨벤션 기반 라우팅 대신에 특성(어트리뷰트) 라우팅 추천
    /// </summary>
    [Route("api/[controller]")]
    [EnableCors("AllowAnyOrigin")]
    public class HeroesController(IHeroRepository repository, ILogger<HeroesController> logger) : Controller
    {
        /// <summary>
        /// GET: /api/heroes
        /// </summary>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(List<Hero>), 200)]
        public IActionResult Get()
        {
            try
            {
                var models = repository.GetAllHeroes();
                return Ok(models);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "HeroesController - Get error");
                return BadRequest();
            }
        }

        /// <summary>
        /// GET: /api/heroes/{id}
        /// </summary>
        [HttpGet("{id:int}", Name = "GetHeroById")]
        [ProducesResponseType(typeof(Hero), 200)]
        public IActionResult Get(int id)
        {
            try
            {
                var model = repository.GetHeroById(id);
                if (model is null)
                {
                    return NotFound($"{id}번 데이터가 없습니다.");
                }

                return Ok(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "HeroesController - GetById error");
                return BadRequest($"에러가 발생했습니다. {ex.Message}");
            }
        }

        /// <summary>
        /// POST: /api/heroes
        /// </summary>
        [HttpPost]
        [Produces("application/json", Type = typeof(Hero))]
        [Consumes("application/json")]
        public IActionResult Post([FromBody] Hero model)
        {
            if (model is null)
            {
                return BadRequest();
            }

            try
            {
                if (string.IsNullOrWhiteSpace(model.Name))
                {
                    ModelState.AddModelError("Name", "이름을 입력해야 합니다.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                model.Name ??= string.Empty;
                model.Icon ??= string.Empty;

                var m = repository.AddHero(model);

                if (DateTime.Now.Second % 2 == 0)
                {
                    return CreatedAtRoute("GetHeroById", new { id = m.Id }, m);
                }
                else
                {
                    var uri = Url.Link("GetHeroById", new { id = m.Id });
                    return uri is not null
                        ? Created(uri, m)
                        : CreatedAtRoute("GetHeroById", new { id = m.Id }, m);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "HeroesController - Post error");
                return BadRequest($"에러가 발생했습니다. {ex.Message}");
            }
        }

        /// <summary>
        /// PUT: /api/heroes/{id}
        /// </summary>
        [HttpPut("{id:int}")]
        public IActionResult Put(int id, [FromBody] Hero model)
        {
            if (model is null)
            {
                return BadRequest();
            }

            try
            {
                var oldModel = repository.GetHeroById(id);
                if (oldModel is null)
                {
                    return NotFound($"{id}번 데이터가 없습니다.");
                }

                model.Id = id;
                model.Name ??= string.Empty;
                model.Icon ??= string.Empty;

                repository.UpdateHero(model);
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "HeroesController - Put error");
                return BadRequest($"데이터가 업데이트되지 않았습니다. {ex.Message}");
            }
        }

        /// <summary>
        /// DELETE: /api/heroes/{id}
        /// </summary>
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var oldModel = repository.GetHeroById(id);
                if (oldModel is null)
                {
                    return NotFound($"{id}번 데이터가 없습니다.");
                }

                repository.RemoveHero(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "HeroesController - Delete error");
                return BadRequest($"삭제할 수 없습니다. {ex.Message}");
            }
        }

        /// <summary>
        /// GET: /api/heroes/page/1/10
        /// </summary>
        [HttpGet("page/{pageNumber:int}/{pageSize:int}")]
        [ProducesResponseType(typeof(IEnumerable<Hero>), 200)]
        public IActionResult Get(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                int pageIndex = pageNumber > 0 ? pageNumber - 1 : 0;
                var models = repository.GetAllHeroesWithPaging(pageIndex, pageSize);

                Response.Headers["X-TotalRecordCount"] = repository.GetRecordCountHeroes().ToString();

                return Ok(models);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "HeroesController - GetByPaging error");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{name}", Name = "GetHeroByName")]
        [ProducesResponseType(typeof(Hero), 200)]
        public IActionResult Get(string name)
        {
            try
            {
                var model = repository.GetHeroByName(name);
                return Ok(model ?? new Hero());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "HeroesController - GetByName error");
                return BadRequest($"에러가 발생했습니다. {ex.Message}");
            }
        }
    }
}