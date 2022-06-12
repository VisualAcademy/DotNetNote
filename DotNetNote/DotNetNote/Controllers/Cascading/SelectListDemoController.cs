using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DotNetNote.Data;
using System.Linq;

namespace DotNetNote.Controllers.Cascading
{
    public class SelectListDemoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SelectListDemoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["PropertyId"] =
                new SelectList(_context.Properties.OrderBy(it => it.Name), "Id", "Name");
            ViewData["LocationId"] =
                new SelectList(_context.Locations.OrderBy(it => it.Name), "Id", "Name");
            ViewData["SublocationId"] =
                new SelectList(_context.Sublocations.OrderBy(it => it.SublocationName), "Id", "SublocationName");

            return View();
        }
    }
}
