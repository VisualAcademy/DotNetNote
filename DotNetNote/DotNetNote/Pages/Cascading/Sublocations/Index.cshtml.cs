#nullable disable
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VisualAcademy.Pages.Cascading.Sublocations;

public class IndexModel(ApplicationDbContext context) : PageModel
{
    public IList<Sublocation> Sublocation { get;set; }

    public async Task OnGetAsync()
    {
        Sublocation = await context.Sublocations
            .Include(s => s.LocationRef).ToListAsync();
    }
}
