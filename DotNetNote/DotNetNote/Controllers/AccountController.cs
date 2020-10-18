using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using DotNetNote.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DotNetNoteCom.Controllers
{
    [Produces("application/json")]
    [Route("api/Account")]
    public class AccountController : Controller
    {
        [HttpPost]
        public IActionResult Register([FromBody] SignBase sign)
        {
            //[!] 회원 가입 소스 들어오는 곳
            RegisterProcess(sign); 

            //[!] 토큰 생성하기 코드 들어오는 곳
            var jst = new JwtSecurityToken();

            return Ok(new JwtSecurityTokenHandler().WriteToken(jst));
        }

        private void RegisterProcess(SignBase sign)
        {
            //[!] 회원 가입 소스 들어오는 곳
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            if (!LoginProcess(new LoginViewModel()))
            {
                return NotFound("이메일 또는 암호가 틀립니다.");
            }

            // 보안키 생성
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("DotNetNote1234567890"));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            // 클레임 생성
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "Administrator")
            };

            //[!] 토큰 생성하기 코드 들어오는 곳
            var token = new JwtSecurityToken(claims: claims, signingCredentials: signingCredentials);

            string t = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(t);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginViewModel login)
        {
            if (!LoginProcess(login))
            {
                return NotFound("이메일 또는 암호가 틀립니다.");
            }

            // 보안키 생성
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("DotNetNote1234567890"));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            // 클레임 생성
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, login.Email)
            };

            //[!] 토큰 생성하기 코드 들어오는 곳
            var token = new JwtSecurityToken(claims: claims, signingCredentials: signingCredentials, expires: DateTime.Now.AddMinutes(5));

            string t = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(t);
        }

        /// <summary>
        /// 로그인 처리: 이메일/암호가 맞으면 true 반환
        /// </summary>
        private bool LoginProcess(LoginViewModel login)
        {
            return true; 
        }

        //[Authorize]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("LoginTest")]
        public IActionResult LoginTest()
        {
            //var username = HttpContext.User.Claims.First().Value;
            return Ok("인증됨");
        }
    }
}
