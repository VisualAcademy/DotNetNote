using Microsoft.Extensions.Configuration;

namespace DotNetNote.Controllers;

[Authorize(Roles = "Administrators")]
public class AppSettingsDemo(IConfiguration configuration) : Controller
{
    public IActionResult Index()
    {
        string con1 = configuration.GetSection("StorageConnectionString1").Value;

        string site1 = configuration
            .GetSection("BlogStorageConnectionString")
            .GetSection("Site1").Value;

        string site2 = configuration
            .GetValue<string>("BlogStorageConnectionString:Site2");

        return Content($"{con1}, {site1}, {site2}");
    }
}
