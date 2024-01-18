using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DotNetNote.Data;
using System.Linq;

namespace DotNetNote.Controllers.Cascading;

public class SelectListDemoController(ApplicationDbContext context) : Controller
{
    public IActionResult Index()
    {
        ViewData["PropertyId"] =
            new SelectList(context.Properties.OrderBy(it => it.Name), "Id", "Name");
        ViewData["LocationId"] =
            new SelectList(context.Locations.OrderBy(it => it.Name), "Id", "Name");
        ViewData["SublocationId"] =
            new SelectList(context.Sublocations.OrderBy(it => it.SublocationName), "Id", "SublocationName");

        return View();
    }
}
