using MemberManagement.Data;

namespace DotNetNote.Controllers;

public class MemberController(MemberDbContext memberDbContext) : Controller
{
    public IActionResult Index()
    {
        var members = memberDbContext.Members.OrderByDescending(m => m.Id).ToList();

        return View(members);
    }

    [HttpPost]
    public IActionResult Index(string firstName)
    {
        var member = new Member { FirstName = firstName };

        memberDbContext.Add(member);
        memberDbContext.SaveChanges();

        return RedirectToAction(nameof(Index));
    }
}
