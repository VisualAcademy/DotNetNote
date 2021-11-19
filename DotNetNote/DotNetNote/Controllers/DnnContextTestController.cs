using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace DotNetNote.Controllers
{
    public class DnnContextTestController : Controller
    {
        private readonly DotNetNoteContext _context;

        public DnnContextTestController(DotNetNoteContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var ideas = _context.Ideas.ToList();
            return View(ideas);
        }
    }
}
