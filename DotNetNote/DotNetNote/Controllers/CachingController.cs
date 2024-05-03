using Microsoft.Extensions.Caching.Memory;

namespace DotNetNote.Controllers;

public class CachingController(IMemoryCache memoryCache) : Controller
{
    public IActionResult Index()
    {
        // 캐시에 담을 개체
        DateTime cacheData;

        // 캐시에 데이터가 들어있으면 해당 데이터를 가져오기
        if (!memoryCache.TryGetValue("SetTime", out cacheData))
        {
            // 캐시에 개체 값을 담기
            cacheData = DateTime.Now;

            // 캐시에 현재 시간 저장
            memoryCache.Set(
                "SetTime",
                cacheData,
                (new MemoryCacheEntryOptions()).SetAbsoluteExpiration(TimeSpan.FromSeconds(5)));
        }

        ViewBag.CachedDateTime = cacheData;

        return View();
    }

    public IActionResult CacheGetOrCreate()
    {
        var cacheData = memoryCache.GetOrCreate("SetString", e =>
        {
            e.SlidingExpiration = TimeSpan.FromSeconds(5);
            return "초: " + DateTime.Now.Second.ToString();
        });

        ViewBag.SetString = cacheData;

        return View();
    }

    public async Task<IActionResult> CacheGetOrCreateAsync()
    {
        // 캐시 읽어오기
        //var cacheData = _cache.Get<string>("SetString");
        // 캐시 제거
        // _cache.Remove("SetString");

        var cacheData = await memoryCache.GetOrCreateAsync("SetString", e =>
        {
            e.SlidingExpiration = TimeSpan.FromSeconds(5);
            return Task.FromResult("초: " + DateTime.Now.Second.ToString());
        });

        ViewBag.SetString = cacheData;

        return View("CacheGetOrCreate");
    }
}
