using Microsoft.AspNetCore.Mvc;
using System;

namespace DotNetNote.Controllers;

[Route("api/[controller]")]
public class UserServiceController(IUserRepository repo) : Controller
{
    /// <summary>
    /// 아이디 중복 확인 Web API
    /// </summary>
    /// <param name="username">사용자 아이디</param>
    /// <returns>가입 여부 확인</returns>
    [HttpGet("{username}")]
    public IActionResult Get(string username)
    {
        try
        {
            var model = repo.GetUserByUserId(username);
            if (model.UserId == null) // TODO: 모델 클래스로 null 값 체크?
            {
                return NotFound($"{username} 아이디가 없습니다."); // 404 Not Found
            }
            return Ok("중복된 아이디 입니다."); // 200 OK
        }
        catch (Exception ex)
        {
            return BadRequest($"에러가 발생했습니다. {ex.Message}");
        }
    }
}
