namespace DotNetNote.Controllers;

[Route("api/PersonService")]
public class PersonServiceController : Controller
{
    [HttpGet]
    public IEnumerable<PersonModel> Get() =>
        [
            new PersonModel { Id = 1, Name = "김태영" },
            new PersonModel { Id = 2, Name = "박용준" },
            new PersonModel { Id = 3, Name = "한상훈" }
        ];

    [HttpPost]
    public PersonModel Post([FromBody] PersonModel model) =>
        // 중단점 설정 후 크롬 확장 도구인 POSTMAN으로 테스트
        model;
}
