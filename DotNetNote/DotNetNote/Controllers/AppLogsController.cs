using DotNetNote.Utils;
using System.Globalization;

namespace DotNetNote.Controllers
{
    public class AppLogsController : Controller
    {
        private readonly LogsDbContext _db;
        public AppLogsController(LogsDbContext db) => _db = db;

        // GET: /AppLogs
        // q: 검색어, level: 레벨, page/pageSize: 페이징
        public async Task<IActionResult> Index(string? q, string? level, int page = 1, int pageSize = 20)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 5, 200);

            var query = _db.AppLogs.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(level))
                query = query.Where(x => x.Level == level);

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(x =>
                    (x.Message != null && x.Message.Contains(q)) ||
                    (x.Exception != null && x.Exception.Contains(q)) ||
                    (x.Properties != null && x.Properties.Contains(q)) ||
                    (x.Level != null && x.Level.Contains(q)));
            }

            query = query.OrderByDescending(x => x.TimeStamp);

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // 목록에 쓸 ViewModel로 변환 (짧은 메시지 + 상세 링크 키)
            var vm = items.Select(x => new LogListItem
            {
                TimeStamp = x.TimeStamp,
                Level = x.Level,
                MessageShort = Trim(x.Message, 120),
                Key = LogKey.MakeKey(x), // 상세 조회용 해시
            }).ToList();

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Total = total;
            ViewBag.Query = q;
            ViewBag.Level = level;

            return View(vm);

            static string Trim(string? s, int n)
            {
                if (string.IsNullOrEmpty(s)) return "";
                return s.Length > n ? s[..n] + "…" : s;
            }
        }

        // GET: /AppLogs/Details?key=...&ts=...&level=...
        // URL에는 충돌 줄이기 위해 TimeStamp(+Level)도 함께 보냄
        public async Task<IActionResult> Details(string key, string ts, string? level)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(ts))
                return BadRequest();

            if (!DateTimeOffset.TryParseExact(ts, "o", CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal, out var timestamp))
                return BadRequest("Invalid timestamp.");

            // 후보를 좁히기: 같은 시각(+ 레벨) 레코드들만
            var candidates = _db.AppLogs.AsNoTracking().Where(x => x.TimeStamp == timestamp);
            if (!string.IsNullOrWhiteSpace(level))
                candidates = candidates.Where(x => x.Level == level);

            var list = await candidates.ToListAsync();
            var match = list.FirstOrDefault(x => LogKey.MakeKey(x) == key);
            if (match == null) return NotFound();

            return View(match);
        }
    }

    public sealed class LogListItem
    {
        public string? Level { get; set; }
        public DateTimeOffset? TimeStamp { get; set; }
        public string? MessageShort { get; set; }
        public string Key { get; set; } = "";
    }
}
