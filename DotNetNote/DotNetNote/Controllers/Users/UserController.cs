//[User][6]
using DotNetNote.Common;
using DotNetNote.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DotNetNote.Controllers;

/// <summary>
/// 쿠키 인증(Identity Cookie)을 사용한 회원 가입, 로그인, 로그아웃, 로그인 정보 표시 
/// </summary>
public class UserController : Controller
{
    //[User][6][1]
    private IUserRepository _repository;
    private ILoginFailedManager _loginFailed;
    private IUserModelRepository _userRepo;

    public UserController(
        IUserRepository repository,
        ILoginFailedManager loginFailed,
        IUserModelRepository userRepo)
    {
        _repository = repository;
        _loginFailed = loginFailed;
        _userRepo = userRepo; // 추가 
    }

    //[User][6][2]
    [Authorize]
    public IActionResult Index() => View();

    //[User][6][3] : 회원 가입 폼
    [HttpGet]
    public IActionResult Register() => View();

    //[User][6][4] : 회원 가입 처리
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(UserViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (_repository.GetUserByUserId(model.UserId).UserId != null)
            {
                ModelState.AddModelError("", "이미 가입된 사용자입니다.");
                return View(model);
            }
        }

        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "잘못된 가입 시도!!!");
            return View(model);
        }
        else
        {
            //_repository.AddUser(model.UserId, model.Password);
            string encryptPassword = (new Dul.Security.CryptorEngine())
                .EncryptPassword(model.Password);
            _repository.AddUser(model.UserId, encryptPassword);
            return RedirectToAction("Index");
        }
    }

    //[User][6][5] : 로그인 폼
    [HttpGet]
    [AllowAnonymous] // 인증되지 않은 사용자도 접근 가능
    public IActionResult Login(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        // Login.cshtml 뷰 페이지에서 로그인 실패 메시지 출력
        ViewBag.IsLoginFailed = false;

        return View();
    }

    //[User][6][6] : 로그인 처리
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        UserViewModel model, string returnUrl = null)
    {
        if (ModelState.IsValid)
        {
            #region 로그인 실패 5번 체크
            //[!] 로그인 실패 5번 체크
            if (_loginFailed.IsLoginFailed(model.UserId))
            {
                // Login.cshtml 뷰 페이지에서 로그인 실패 메시지 출력
                ViewBag.IsLoginFailed = true;

                return View(model);
            }
            #endregion

            #region 아이디와 암호가 맞으면 로그인(인증) 처리
            //if (_repository.IsCorrectUser(model.UserId, model.Password))
            string encryptPassword = (new Dul.Security.CryptorEngine())
                .EncryptPassword(model.Password);
            if (_repository.IsCorrectUser(model.UserId, encryptPassword))
            {

                #region 로그인 처리
                //[1] List<Claim>
                //[!] 인증 부여: 인증된 사용자의 주요 정보(Name, Role, ...)를 기록
                var claims = new List<Claim>()
                {
                    // 로그인 아이디 지정
                    new Claim("UserId", model.UserId),

                    new Claim(ClaimTypes.NameIdentifier, model.UserId), // 이 속성이 UserId 메인 속성

                    new Claim(ClaimTypes.Name, model.UserId), 
                    //new Claim(ClaimTypes.Email, model.UserId), //

                    //[3] Administrators 이름으로 관리자 권한(Policy) 설정 관련
                    // 기본 역할 지정, "Role" 기능에 "Users" 값 부여
                    new Claim(ClaimTypes.Role, "Users") // 추가 정보 기록
                };

                //[2] ClaimsIdentity 
                //var ci = new ClaimsIdentity(claims, (new Dul.Security.CryptorEngine()).EncryptPassword(model.Password));
                var ci = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                //[1] 로그인 처리: Authorize 특성 사용해서 로그인 체크 가능 
                //[1][1] ASP.NET Core 1.X 사용: 버전업되면서 아래 메서드 사용 중지 
                //await HttpContext.Authentication.SignInAsync(
                //    "Cookies", new ClaimsPrincipal(ci));
                //[1][2] ASP.NET Core 2.X 사용
                var authenticationProperties = new AuthenticationProperties()
                {
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60),
                    IssuedUtc = DateTimeOffset.UtcNow,
                    IsPersistent = true
                };

                //[3] ClaimsPrincipal(identity) => 뷰 페이지에서 @User 개체가 ClaimsPrincipal 개체를 담고 있음
                var principal = new ClaimsPrincipal(ci); 

                //[4] 로그인 처리 완료 
                //await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(ci), new AuthenticationProperties { IsPersistent = true });
                //await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(ci), authenticationProperties);
                //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(ci)); // 기본
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    authenticationProperties); // 옵션

                ////[참고] ASP.NET Core Identity에서 로그인하는 모양
                //var identity = new ClaimsIdentity("Cookies");
                //identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
                //identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));

                //await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(identity));

                // 추가: 세션에 로그인 사용자 정보 저장
                // 세션 인증 로그인: 인증 방식으로 세션 사용 금지
                //HttpContext.Session.SetString("Username", model.UserId);
                #endregion

                // 반드시 LocalRedire() 메서드 사용
                return LocalRedirect("/User/Index"); // (model.ReturnUrl)
            }
            #endregion
        }

        return View(model);
    }

    //[User][6][7]: 로그아웃 처리
    public async Task<IActionResult> Logout()
    {
        #region Old Version
        // Startup.cs의 미들웨어에서 지정한 "Cookies" 이름 사용
        // ASP.NET Core 1.X
        //await HttpContext.Authentication.SignOutAsync("Cookies");
        // ASP.NET Core 2.X
        //await HttpContext.SignOutAsync("Cookies"); 
        #endregion

        //[!] 로그아웃 공식 코드 
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); // 쿠키 인증 로그아웃
        //await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme); // 토큰 인증 로그아웃 
        //HttpContext.Session.Clear(); // 세션 인증 로그아웃

        return Redirect("/User/Index");
    }

    //[User][6][8] : 회원 정보 보기 및 수정
    [Authorize]
    public IActionResult UserInfor() => View();

    //[User][6][9] : 인사말 페이지
    public IActionResult Greetings()
    {
        //[Authorize] 특성의 또 다른 표현 방법
        // 인증되지 않은 사용자는
        if (User.Identity.IsAuthenticated == false)
        {
            // 로그인 페이지로 리디렉션
            return new ChallengeResult();
        }

        return View();
    }

    //[User][6][10] : 접근 거부 페이지
    public IActionResult Forbidden() => View();

    //[!] 추가: 사용자 상세 보기(GetUsers 저장 프로시저 사용)
    [Authorize]
    public IActionResult UserDetail()
    {
        string userId = User.FindFirst("UserId").Value;
        var user = _repository.GetUserByUserId(userId);

        //var userModel = _userRepo.GetUserInfor(user.Id); // 캐싱 적용 전
        var userModel = _userRepo.GetUserInforCache(user.Id); // 캐싱 적용 후

        return View(userModel);
    }

    /// <summary>
    /// 아이디 중복 확인 Web API 테스트
    /// </summary>
    public IActionResult CheckUsername() => View();
}
