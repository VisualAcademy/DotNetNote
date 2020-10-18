using System.Linq;
using DotNetNote.Models;
using Microsoft.AspNetCore.Mvc;

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
