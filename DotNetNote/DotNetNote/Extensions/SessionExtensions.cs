//[!] 개체 형식을 세션에 저장하고 읽어오기
//https://docs.microsoft.com/ko-kr/aspnet/core/fundamentals/app-state
#nullable enable

using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

public static class SessionExtensions
{
    public static void Set<T>(this ISession session, string key, T value)
    {
        if (session == null)
        {
            throw new ArgumentNullException(nameof(session));
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Session key cannot be null or whitespace.", nameof(key));
        }

        var json = JsonConvert.SerializeObject(value) ?? "null";

        session.SetString(key, json);
    }

    public static T? Get<T>(this ISession session, string key)
    {
        if (session == null)
        {
            throw new ArgumentNullException(nameof(session));
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Session key cannot be null or whitespace.", nameof(key));
        }

        var value = session.GetString(key);

        if (string.IsNullOrWhiteSpace(value))
        {
            return default;
        }

        return JsonConvert.DeserializeObject<T>(value);
    }
}