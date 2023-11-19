using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

public class DemosController : Controller
{
    // GET: /<controller>/
    public IActionResult Index() => View();

    public IActionResult FillDropDownListWithAjax() => View();
}
