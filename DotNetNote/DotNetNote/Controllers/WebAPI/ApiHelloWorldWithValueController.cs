using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ApiHelloWorld.Controllers
{
    [Route("api/[controller]")]
    public class ApiHelloWorldWithValueController : Controller
    {
        [HttpGet]
        public IEnumerable<Value> Get()
        {
            //return new string[] { "안녕하세요.", "반갑습니다." };
            return new Value[] {
                new Value { Id = 1, Text = "안녕하세요" },
                new Value { Id = 2, Text = "반갑습니다" },
                new Value { Id = 3, Text = "또 만나요" }
            };
        }

        [HttpGet("{id:int}")]
        public Value Get(int id)
        {
            //return $"넘어온 값: {id}";
            return new Value { Id = id, Text = $"넘어온 값: {id}" };
        }

        //[1]
        //[HttpPost]
        //public IActionResult Post([FromBody]Value value)
        //{
        //    // Chrome://apps의 POSTMAN, Swagger, Fiddler 등의 외부 도구로 테스트

        //    // TODO: 넘어온 JSON 데이터를 DB에 저장 후 Id 값 반환

        //    return CreatedAtAction("Get", new { id = value.Id }, value);
        //}

        ////[2] 
        //[HttpPost]
        //[Produces("application/json", Type = typeof(Value))]
        //[Consumes("application/json")]
        //public IActionResult Post([FromBody]Value value)
        //{
        //    return CreatedAtAction("Get", new { id = value.Id }, value);
        //}

        //[3] 
        [HttpPost]
        [Produces("application/json", Type = typeof(Value))]
        [Consumes("application/json")]
        public IActionResult Post([FromBody]Value value)
        {
            // 모델 유효성 검사
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // 400 에러 출력
            }
            return CreatedAtAction("Get", new { id = value.Id }, value); // 201
        }

    }

    // 모델 유효성 검사
    public class Value
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Text 속성은 필수입력값입니다.")]
        public string Text { get; set; }
    }

    //// 포맷터: JSON, XML
    //public class Value
    //{
    //    public int Id { get; set; }
    //    public string Text { get; set; }
    //}

    [Route("[controller]/[action]")]
    public class ApiHelloWorldDemoController : Controller
    {
        public IActionResult Index()
        {
            string html = @"
<html>
<head>
    <title>jQuery로 JSON 사용하기</title>
</head>
<body>
    <h1>Web API 호출</h1>
    <div id='print'></div>
    <script src='https://code.jquery.com/jquery-1.12.4.min.js'></script>
    <script>
        var API_URI = '/api/ApiHelloWorldWithValue';
        $(function() {
            $.getJSON(API_URI, function(data) {
                var str = '<dl>';
                $.each(data, function(index, entry) {
                    str += '<dt>' + entry.id + '</dt>';
                    str += '<dd>' + entry.text + '</dd>';
                });
                str += '</dl>';

                $('#print').html(str);
            });
        });
    </script>
</body>
</html>
";

            return new ContentResult()
            {
                Content = html,
                ContentType = "text/html; charset=utf-8"
            };
        }
    }

    [Route("[controller]/[action]")]
    public class ApiCorsDemoController : Controller
    {
        public IActionResult Index()
        {
            string html = @"
<html>
<head>
    <title>CORS</title>
</head>
<body>
    <h1>외부에 있는 Web API 호출</h1>
    <div id='print'></div>
    <script src='https://code.jquery.com/jquery-1.12.4.min.js'></script>
    <script>
        // CORS 설정 필요
        //var API_URI = '/api/values';
        var API_URI = 'http://dotnetnote.azurewebsites.net/api/values'; 
        $(function() {
            $.getJSON(API_URI, function(data) {
                $('#print').html(data);
            });
        });
    </script>
</body>
</html>
";

            return new ContentResult()
            {
                Content = html,
                ContentType = "text/html; charset=utf-8"
            };
        }
    }
}
