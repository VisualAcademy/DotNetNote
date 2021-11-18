using DotNetNoteCore.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DotNetNote.Controllers;

public class AppKeysController : Controller
{
    private readonly AppKeyConfig appKeyConfig;

    public AppKeysController(IOptions<AppKeyConfig> appKeyConfig)
    {
        this.appKeyConfig = appKeyConfig.Value;
    }

    public IActionResult Index()
    {
        //ViewData["AzureStorageAccount"] = appKeyConfig.AzureStorageAccount;
        ViewData["AzureStorageAccount"] = "해킹금지";
        return View();
    }
}
