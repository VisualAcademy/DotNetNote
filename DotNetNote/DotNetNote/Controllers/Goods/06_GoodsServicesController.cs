using System;
using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers
{
    /// <summary>
    /// [6] Web API 컨트롤러 클래스: Angular / jQuery 등의 프론트엔드에서 접근할 Web API
    /// 복수형 또는 Services 등의 접미사 사용 권장
    /// 컨벤션 기반 라우팅 대신에 특성(어트리뷰트) 라우팅 추천
    /// </summary>
    [Route("api/[controller]")]
    public class GoodsServicesController : Controller
    {
        // @Inject
        private readonly IGoodsRepository _repository;

        public GoodsServicesController(IGoodsRepository repository) => _repository = repository;

        // GET: api/<controller>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(List<GoodsBase>), 200)]
        public IActionResult Get()
        {
            try
            {
                var models = _repository.GetAllGoods();
                if (models == null)
                {
                    return NotFound("아무런 데이터가 없습니다.");
                }
                return Ok(models); // 200 OK 
            }
            catch (Exception ex)
            {
                return BadRequest(ex); 
            }
        }

        // GET api/<controller>/5
        [HttpGet("{id:int}", Name = "GetGoodsById")] // Name 속성은 프로젝트에서 유일한 값 지정
        [ProducesResponseType(typeof(GoodsBase), 200)]
        public IActionResult Get(int id)
        {
            ////500 에러 찍어보려면,
            //throw new Exception("인위적으로 에러 발생시켜 500에러 출력");
            try
            {
                var model = _repository.GetGoodsById(id);
                if (model == null)
                {
                    return NotFound($"{id}번 데이터가 없습니다."); // 404 
                }
                return Ok(model); // 200

            }
            catch (Exception ex)
            {
                return BadRequest($"에러가 발생했습니다. {ex.Message}"); // 500
            }
        }

        // POST api/<controller>
        [HttpPost]
        [Produces("application/json", Type = typeof(GoodsBase))]
        [Consumes("application/json")] // application/xml 
        public IActionResult Post([FromBody]GoodsBase model) // Deserialize, 생성 전용 DTO 클래스 사용 가능
        {
            // 예외 처리 방법
            if (model == null)
            {
                return BadRequest(); // Status: 400 Bad Request
            }

            try
            {
                // 예외 처리
                if (model.GoodsName == null || model.GoodsName.Length < 1)
                {
                    ModelState.AddModelError("GoodsName", "제품명을 입력해야 합니다."); 
                }

                // 모델 유효성 검사
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState); // 400 에러 출력
                }

                var m = _repository.AddGoods(model); // 저장

                //return Ok(m); // 200 OK
                //return CreatedAtRoute("GetGoodsById", new { id = m.GoodsId }, m); // Status: 201 Created
                if (DateTime.Now.Second % 2 == 0) //[!] 2가지 방식 중 원하는 방식 사용
                {
                    //return CreatedAtAction("GetGoodsById", new { id = m.GoodsId }, m);
                    return CreatedAtRoute("GetGoodsById", new { id = m.GoodsId }, m); // Status: 201 Created
                }
                else
                {
                    var uri = Url.Link("GetGoodsById", new { id = m.GoodsId });
                    return Created(uri, m); // 201 Created
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"에러가 발생했습니다. {ex.Message}"); 
            }
        }

        // PUT api/<controller>/5
        [HttpPut("{id:int}")] // HttpPatch == 부분 업데이트
        public IActionResult Put(int id, [FromBody]GoodsBase model)
        {
            if (model == null)
            {
                return BadRequest(); 
            }

            try
            {
                var oldModel = _repository.GetGoodsById(id);
                if (oldModel == null)
                {
                    return NotFound($"{id}번 데이터가 없습니다.");
                }
                model.GoodsId = id;
                _repository.UpdateGoods(model); // 수정
                return NoContent(); // 204: 이미 넘어온 정보에 모든 값을 가지고 있기에...
            }
            catch (Exception ex)
            {
                return BadRequest($"데이터가 업데이트되지 않았습니다. {ex.Message}");
            }
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")] // 데코레이터 특성
        public IActionResult Delete(int id)
        {
            try
            {
                var oldModel = _repository.GetGoodsById(id);
                if (oldModel == null)
                {
                    return NotFound($"{id}번 데이터가 없습니다."); 
                }

                // 삭제
                _repository.RemoveGoods(id);

                return NoContent(); // 204 No Content 
            }
            catch (Exception ex)
            {
                return BadRequest($"삭제할 수 없습니다. {ex.Message}");
            }
        }

        // 페이징 처리 Web API
        // GET: /api/GoodsServices/page/1/10
        [HttpGet("page/{pageNumber:int}/{pageSize:int}")]
        public IActionResult Get(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var goodsSet = _repository.GetAllGoodsWithPaging(pageNumber, pageSize);
                if (goodsSet == null || goodsSet.Goods == null)
                {
                    return NotFound("아무런 데이터가 없습니다.");
                }

                // 응답 헤더에 총 레코드 수를 담아서 출력
                Response.Headers.Add("X-TotalRecordCount", goodsSet.GoodsCount.ToString());

                return Ok(goodsSet.Goods); // 200 OK
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); 
            }
        }
    }
}
