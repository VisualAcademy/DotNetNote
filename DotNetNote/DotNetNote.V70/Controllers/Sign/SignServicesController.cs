// JWT 인증 테스트 
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

namespace DotNetNote.Controllers;

[Produces("application/json")]
[Route("api/SignServices")]
public class SignServicesController : Controller
{
    private readonly ISignRepository _repository;
    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration
    private readonly IConfiguration _configuration;

    public SignServicesController(ISignRepository repo, IConfiguration config)
    {
        _repository = repo;
        _configuration = config;
    }

    /// <summary>
    /// 회원 로그인
    /// </summary>
    [HttpPost("Login")]
    public IActionResult Login([FromBody] SignViewModel model)
    {
        if (!IsLogin(model))
        {
            //return NotFound("이메일 또는 암호가 틀립니다.");
            return BadRequest("이메일 또는 암호가 틀립니다.");
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

        string jwtToken = CreateToken(model);

        return Ok(new SignDto
        {
            SignId = sign.SignId,
            Token = jwtToken,
            Email = model.Email
        });
    }

    // https://www.memoengine.com/blog/aspnet-core-jwt-%ED%86%A0%ED%81%B0-%EC%83%9D%EC%84%B1-%EA%B5%90%EA%B3%BC%EC%84%9C-%EC%BD%94%EB%93%9C/
    /// <summary>
    /// 토큰 생성: JWT 토큰 생성 교과서 코드
    /// TODO: Claim 추가 및  JwtSecurityToken에 issuer 등의 추가 매개 변수 지정
    /// </summary>
    private string CreateToken(SignViewModel model)
    {
        //[1] 클레임 생성
        var claims = new Claim[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, model.Email),
            new Claim(JwtRegisteredClaimNames.Email, model.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        //[2] 보안키 생성
        var key = _configuration.GetSection("SymmetricSecurityKey").Value;
        var securityKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var signingCredentials =
            new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        //[3] 토큰 생성하기
        var token = new JwtSecurityToken(
            issuer: "https://localhost:5001/", // 내용 채울 것
            audience: "DotNetNote", // 내용 채울 것            
            expires: DateTime.UtcNow.AddMinutes(5),

            claims: claims,
            signingCredentials: signingCredentials
        );
        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

        //[!] 토큰 반환 
        return jwtToken;
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
