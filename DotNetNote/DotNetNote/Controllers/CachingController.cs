using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace DotNetNote.Controllers;

public class CachingController : Controller
{
    private IMemoryCache _cache;

    public CachingController(IMemoryCache memoryCache) => _cache = memoryCache;

    public IActionResult Index()
    {
        // 캐시에 담을 개체
        DateTime cacheData;

        // 캐시에 데이터가 들어있으면 해당 데이터를 가져오기
        if (!_cache.TryGetValue("SetTime", out cacheData))
        {
            // 캐시에 개체 값을 담기
            cacheData = DateTime.Now;

            // 캐시에 현재 시간 저장
            _cache.Set(
                "SetTime",
                cacheData,
                (new MemoryCacheEntryOptions()).SetAbsoluteExpiration(TimeSpan.FromSeconds(5)));
        }

        ViewBag.CachedDateTime = cacheData;

        return View();
    }

    public IActionResult CacheGetOrCreate()
    {
        var cacheData = _cache.GetOrCreate("SetString", e =>
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

        var cacheData = await _cache.GetOrCreateAsync("SetString", e =>
        {
            e.SlidingExpiration = TimeSpan.FromSeconds(5);
            return Task.FromResult("초: " + DateTime.Now.Second.ToString());
        });

        ViewBag.SetString = cacheData;

        return View("CacheGetOrCreate");
    }
}
