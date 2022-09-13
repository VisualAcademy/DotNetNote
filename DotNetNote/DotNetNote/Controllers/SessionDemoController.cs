using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

namespace DotNetNote.Controllers;

/// <summary>
/// Session 개체 사용 데모 
/// </summary>
public class SessionDemoController : Controller
{
    public IActionResult Index()
    {
        //[1] 세션(Session) 개체 저장
        HttpContext.Session.SetString("Username", "Green");
        return View();
    }

    public IActionResult GetSession()
    {
        //[2] 세션(Session) 개체 읽기
        ViewBag.Username = HttpContext.Session.GetString("Username");
        return View();
    }

    /// <summary>
    /// 세션에 날짜값 저장
    /// /Extensions/SessionExtensions.cs 파일이 필요
    /// </summary>
    public IActionResult SetDate()
    {
        // 현재 시간을 세션 개체에 저장
        HttpContext.Session.Set<DateTime>("NowDate", DateTime.Now);
        return RedirectToAction("GetDate");
    }

    /// <summary>
    /// 세션에서 날짜값 읽어오기
    /// </summary>
    public IActionResult GetDate()
    {
        // 세션에서 "NowDate"의 값을 읽어오기
        var date = HttpContext.Session.Get<DateTime>("NowDate");
        var sessionTime = date.TimeOfDay.ToString();
        var currentTime = DateTime.Now.TimeOfDay.ToString();
        return Content($"현재 시간: {currentTime} - "
            + $"세션에 저장되어 있는 시간: {sessionTime}");
    }
}

/// <summary>
/// 개체 형식을 세션에 저장하고 읽어오고자 한다면. 
/// https://docs.microsoft.com/ko-kr/aspnet/core/fundamentals/app-state
/// http://www.c-sharpcorner.com/article/session-state-in-asp-net-core/
/// </summary>
public static class SessionExtensions
{
    public static T GetComplexData<T>(this ISession session, string key)
    {
        var data = session.GetString(key);
        if (data == null)
        {
            return default(T);
        }
        return JsonConvert.DeserializeObject<T>(data);
    }

    public static void SetComplexData(this ISession session, string key, object value) 
        => session.SetString(key, JsonConvert.SerializeObject(value));
}  
