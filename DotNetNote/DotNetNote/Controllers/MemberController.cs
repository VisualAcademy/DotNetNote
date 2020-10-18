using System.Linq;
using DotNetNote.Models;
using MemberManagement.Data;
using Microsoft.AspNetCore.Mvc;

namespace MemberManagement.Controllers
{
    public class MemberController : Controller
    {
        private MemberDbContext _memberDbContext;

        public MemberController(MemberDbContext memberDbContext)
        {
            _memberDbContext = memberDbContext;
        }

        public IActionResult Index()
        {
            var members = _memberDbContext.Members.OrderByDescending(m => m.Id).ToList();

            return View(members);
        }

        [HttpPost]
        public IActionResult Index(string firstName)
        {
            var member = new Member { FirstName = firstName };

            _memberDbContext.Add(member);
            _memberDbContext.SaveChanges();

            return RedirectToAction(nameof(Index)); 
        }
    }
}
