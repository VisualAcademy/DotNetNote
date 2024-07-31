#nullable disable
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VisualAcademy.Pages.Cascading.Locations
{
    public class IndexModel : PageModel
    {
        private readonly DotNetNote.Data.ApplicationDbContext _context;

        public IndexModel(DotNetNote.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Location> Location { get;set; }

        public async Task OnGetAsync()
        {
            Location = await _context.Locations
                .Include(l => l.PropertyRef).ToListAsync();
        }
    }
}
