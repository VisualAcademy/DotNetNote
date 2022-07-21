using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;

namespace DotNetNote.Controllers
{
    /// <summary>
    /// 명언(Maxim) 제공 서비스: /api/maximservice/
    /// 기본 뼈대 만드는 것은 Web API 스캐폴딩으로 구현 후 각각의 코드를 구현
    /// </summary>
    [Route("api/[controller]")]
    public class MaximServiceController : Controller
    {
        private MaximServiceRepository repo;

        public MaximServiceController(MaximServiceRepository maximService) => repo = maximService;

        // GET: api/MaximService
        //[1] Get 요청의 가장 기본적인 모양
        //[HttpGet("")]
        //public IEnumerable<Maxim> Get()
        //{
        //    return repo.GetMaxims().AsEnumerable();
        //}

        //[2] Get 요청에 대한 예외 처리가 적용된 모양: Ok()와 BadRequest() 사용 
        [HttpGet("")]
        public IActionResult Get()
        {
            try
            {
                var maxims = repo.GetMaxims().AsEnumerable();
                return Ok(maxims);
            }
            catch
            {
                return BadRequest();         
            }
        }
        
        // GET: api/MaximService/5
        //[1] 기본 모양
        //[HttpGet("{id}", Name = "GetMaxim")]
        //public Maxim GetById(int id)
        //{
        //    // 데이터 조회
        //    Maxim maxim = repo.GetMaximById(id);
        //    if (maxim == null)
        //    {
        //        Response.StatusCode = (int)HttpStatusCode.NotFound;
        //        return null;
        //    }
        //    return maxim;
        //}

        //[2] NotFound를 사용한 모양
        [HttpGet("{id}", Name = "GetMaxim")]
        public IActionResult GetById(int id)
        {
            try
            {
                // 데이터 조회
                Maxim maxim = repo.GetMaximById(id);
                if (maxim == null)
                {
                    // 에러에 대한 정보를 전달
                    return NotFound($"{id}에 해당하는 값을 찾을 수 없습니다.");
                }
                return Ok(maxim);
            }
            catch
            {
                return BadRequest(); 
            }
        }

        // POST: api/MaximService
        [HttpPost]
        public JsonResult Post([FromBody]Maxim maxim)
        {
            if (ModelState.IsValid)
            {
                // 데이터 입력
                var m = repo.AddMaxim(maxim);

                Response.StatusCode = (int)HttpStatusCode.Created;
                return Json(m);
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = "실패", ModelState = ModelState });
            }
        }

        // PUT: api/MaximService/5
        [HttpPut("{id}")]
        public JsonResult Put(int id, [FromBody]Maxim maxim)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = "실패", ModelState = ModelState });
            }
            if (id != maxim.Id)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = "실패", ModelState = ModelState });
            }

            try
            {
                // 데이터 수정
                repo.UpdateMaxim(maxim);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(
                    new {Message = $"실패: {ex}", ModelState = ModelState });
            }
            Response.StatusCode = (int)HttpStatusCode.OK;
            return Json(maxim);
        }

        // DELETE: api/MaximService/5
        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            Maxim maxim = repo.GetMaximById(id);
            if (maxim == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new { Message = "실패", ModelState = ModelState });
            }

            try
            {
                // 데이터 삭제
                repo.RemoveMaxim(id);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(
                    new { Message = $"실패: {ex}", ModelState = ModelState });
            }
            Response.StatusCode = (int)HttpStatusCode.OK;
            return Json(maxim);
        }
    }
}
