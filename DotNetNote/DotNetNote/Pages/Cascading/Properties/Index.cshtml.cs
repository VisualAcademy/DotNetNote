#nullable disable
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VisualAcademy.Models;

namespace VisualAcademy.Pages.Cascading.Properties;

public class IndexModel : PageModel
{
    private readonly DotNetNote.Data.ApplicationDbContext _context;

    public IndexModel(DotNetNote.Data.ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Property> Property { get;set; }

    public async Task OnGetAsync()
    {
        Property = await _context.Properties.ToListAsync();
    }
}
