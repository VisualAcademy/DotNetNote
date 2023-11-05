using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace DotNetNote.Controllers;

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
