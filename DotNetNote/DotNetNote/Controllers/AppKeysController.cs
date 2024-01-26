namespace DotNetNote.Controllers;

public class AppKeysController(IOptions<AppKeyConfig> appKeyConfig) : Controller
{
    private readonly AppKeyConfig appKeyConfig = appKeyConfig.Value;

    public IActionResult Index()
    {
        //ViewData["AzureStorageAccount"] = appKeyConfig.AzureStorageAccount;
        ViewData["AzureStorageAccount"] = "해킹금지";
        return View();
    }
}
