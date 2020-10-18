using DotNetNote.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// 캐릭터(Heroes) 선택: 로그인한 회원만 접근 가능
/// </summary>
namespace DotNetNote.Controllers
{
    [Authorize]
    public class CharacterController : Controller
    {
        private IHeroRepository _hero;
        private ICharacterRepository _character;

        public CharacterController(IHeroRepository hero, ICharacterRepository character)
        {
            _hero = hero;
            _character = character;
        }

        public IActionResult Index()
        {
            var heroes = _hero.GetAllHeroes();

            var username = User.FindFirst("UserId").Value; // 사용자 아이디 
            var myCharacter = _character.GetCharacterByUsername(username);
            if (myCharacter != null)
            {
                ViewBag.MyCharacter = myCharacter;
            }

            return View(heroes);
        }

        public IActionResult Choice(int hero)
        {
            // DB에 저장
            var username = User.FindFirst("UserId").Value; // 사용자 아이디
            var heroId = hero; // HeroId

            var model = new CharacterModel() { Username = username, HeroId = heroId };

            _character.SetCharacter(model);

            ViewBag.ChoiceHero = hero.ToString();
            return View();
        }
    }
}
