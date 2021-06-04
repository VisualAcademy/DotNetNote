using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using DotNetNote.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DotNetNoteCom.Controllers
{
    [Produces("application/json")]
    [Route("api/SignServices")]
    public class SignServicesController : Controller
    {
        private readonly ISignRepository _repository;
        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-5.0
        private readonly IConfiguration _configuration;

        public SignServicesController(ISignRepository repository, IConfiguration configuration) 
        {
            _repository = repository;
            _configuration = configuration;
        }

        /// <summary>
        /// 회원 로그인
        /// </summary>
        [HttpPost("Login")]
        public IActionResult Login([FromBody] SignViewModel model)
        {
            if (!IsLogin(model))
            {
                return NotFound("이메일 또는 암호가 틀립니다.");
            }

            string t = CreateToken(model);

            //return Ok(new { Token = t });
            return Ok(new SignDto { Token = t, Email = model.Email });
        }

        /// <summary>
        /// 회원 가입
        /// </summary>
        [HttpPost("Register")]
        public IActionResult Register([FromBody] SignViewModel model)
        {
            var sign = _repository.AddSign(model);
            
            if (sign == null)
            {
                return NotFound("등록이되지 않았습니다.");
            }

            string t = CreateToken(model);

            return Ok(new SignDto { SignId = sign.SignId, Token = t, Email = model.Email });
        }

        /// <summary>
        /// 토큰 생성
        /// </summary>
        private string CreateToken(SignViewModel model)
        {
            //[1] 보안키 생성
            var key = _configuration.GetSection("SymmetricSecurityKey").Value;
            var securityKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var signingCredentials =
                new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //[2] 클레임 생성
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, model.Email)
            };

            //[3] 토큰 생성하기
            var token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: signingCredentials,
                expires: DateTime.Now.AddMinutes(5));
            var t = new JwtSecurityTokenHandler().WriteToken(token);

            //[!] 토큰 반환 
            return t;
        }

        /// <summary>
        /// 로그인 처리: 이메일/암호가 맞으면 true 반환
        /// </summary>
        private bool IsLogin(SignViewModel model)
        {
            //return true; // 일단 무조건 통과
            return _repository.IsAuthenticated(model); 
        }

        [HttpGet("LoginTest")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult LoginTest()
        {
            //return Ok("인증된 사용자만 보는 내용");
            return Ok(HttpContext.User.Claims.First().Value); // a@a.com
        }

        /// <summary>
        /// 회원 정보
        /// </summary>
        /// <returns></returns>
        [HttpGet("Details")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetSignByEmail()
        {
            // 클레임 정보에서 이메일 가져오기 
            var email = HttpContext.User.Claims.First().Value;

            // 현재 접속중인 사용자의 상세 정보 가져오기
            var sign = _repository.GetSignByEmail(email);

            return Ok(sign); 
        }
    }
}
