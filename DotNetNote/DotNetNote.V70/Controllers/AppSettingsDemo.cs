using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DotNetNote.Controllers;

[Authorize(Roles = "Administrators")]
public class AppSettingsDemo : Controller
{
    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-5.0
    private readonly IConfiguration _configuration;

    public AppSettingsDemo(IConfiguration configuration) => this._configuration = configuration;

    public IActionResult Index()
    {
        string con1 = _configuration.GetSection("StorageConnectionString1").Value;

        string site1 = _configuration
            .GetSection("BlogStorageConnectionString")
            .GetSection("Site1").Value;

        string site2 = _configuration
            .GetValue<string>("BlogStorageConnectionString:Site2");

        return Content($"{con1}, {site1}, {site2}");
    }
}
