#nullable disable
using Microsoft.AspNetCore.Mvc.RazorPages;
using Acts.Models;

namespace Acts.Pages.ActionCategories;

public class IndexModel : PageModel
{
    private readonly Acts.Models.ActContext _context;

    public IndexModel(Acts.Models.ActContext context) => _context = context;

    public IList<ActionCategory> ActionCategory { get;set; }

    public async Task OnGetAsync() => ActionCategory = await _context.ActionCategories.ToListAsync();
}
