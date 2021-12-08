using Microsoft.AspNetCore.Mvc;

namespace ApiHelloWorld.Controllers
{
    //[!] 애트리뷰트(어트리뷰트) 라우팅
    [Route("api/[controller]")]
    public class ApiHelloWorldController : Controller
    {
        // api/ApiHelloWorld
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "안녕하세요.", "반갑습니다." };
        }

        // api/ApiHelloWorld/id
        //[!] 라우트 매개변수
        //[HttpGet("{id}")]
        //[!] 모델 바인딩 + 인라인 제약 조건(:)
        [HttpGet("{id:int}")]
        public string Get(int id)
        {
            return $"넘어온 값: {id}";
        }
    }
}
