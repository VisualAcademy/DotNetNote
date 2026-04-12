/// <summary>
/// 캐릭터(Heroes) 선택: 로그인한 회원만 접근 가능
/// </summary>
namespace DotNetNote.Controllers;

[Authorize]
public class CharacterController(IHeroRepository hero, ICharacterRepository character) : Controller
{
    public IActionResult Index()
    {
        var heroes = hero.GetAllHeroes();

        var userIdClaim = User.FindFirst("UserId");
        if (userIdClaim is not null && !string.IsNullOrWhiteSpace(userIdClaim.Value))
        {
            var username = userIdClaim.Value; // 사용자 아이디
            var myCharacter = character.GetCharacterByUsername(username);
            if (myCharacter != null)
            {
                ViewBag.MyCharacter = myCharacter;
            }
        }

        return View(heroes);
    }

    public IActionResult Choice(int hero)
    {
        var userIdClaim = User.FindFirst("UserId");
        if (userIdClaim is null || string.IsNullOrWhiteSpace(userIdClaim.Value))
        {
            return Unauthorized();
        }

        // DB에 저장
        var username = userIdClaim.Value; // 사용자 아이디
        var heroId = hero; // HeroId

        var model = new CharacterModel
        {
            Username = username,
            HeroId = heroId
        };

        character.SetCharacter(model);

        ViewBag.ChoiceHero = hero.ToString();
        return View();
    }
}