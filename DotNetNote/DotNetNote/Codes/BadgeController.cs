using VisualAcademy.Models.Codes;
using VisualAcademy.Models.Configuration;

namespace VisualAcademy.Codes
{
    [AllowAnonymous]
    [Route("badge")]
    public class BadgeController : Controller
    {
        private readonly TenantSettings _tenantSettings;
        private readonly IConfiguration _configuration;

        public BadgeController(
            IOptions<TenantSettings> tenantSettings,
            IConfiguration configuration)
        {
            _tenantSettings = tenantSettings.Value ?? new TenantSettings();
            _configuration = configuration;
        }

        // GET /badge
        [HttpGet("")]
        public IActionResult Index()
        {
            // 테넌트 목록을 노출하지 않음
            return View();
        }

        // GET /badge/{tenant}
        [HttpGet("{tenant}")]
        public IActionResult Tenant(string tenant)
        {
            if (string.IsNullOrWhiteSpace(tenant))
            {
                return NotFound();
            }

            var normalizedTenant = tenant.Trim();

            // 테넌트 검증 (대소문자 무시)
            var matchedTenant = (_tenantSettings.Tenants ?? Array.Empty<string>())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .FirstOrDefault(x => x.Equals(normalizedTenant, StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrWhiteSpace(matchedTenant))
            {
                return NotFound();
            }

            // BaseUrl 직접 읽기
            var baseUrl = _configuration["BaseUrl"]?.Trim();

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                baseUrl = "https://portal.example.com";
            }

            var model = new BadgePageViewModel
            {
                Tenant = matchedTenant,
                CandidatePortalUrl = baseUrl,
                ChangeOfInformationUrl = $"{baseUrl}/Identity/Account/Manage/ChangeOfInformation",
                PageTitle = $"{matchedTenant} Badge QR Code",
                Heading = $"{matchedTenant} Badge QR Code"
            };

            return View("Tenant", model);
        }
    }
}