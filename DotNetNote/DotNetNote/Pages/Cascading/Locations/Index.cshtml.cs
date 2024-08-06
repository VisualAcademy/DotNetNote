#nullable disable
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VisualAcademy.Pages.Cascading.Locations;

public class IndexModel(DotNetNote.Data.ApplicationDbContext context) : PageModel
{
    public IList<Location> Location { get;set; }

    public async Task OnGetAsync()
    {
        Location = await context.Locations
            .Include(l => l.PropertyRef).ToListAsync();
    }
}
