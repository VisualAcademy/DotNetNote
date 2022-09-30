#nullable disable
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DotNetNote.Pages.CabinetTypes
{
    public class IndexModel : PageModel
    {
        private readonly Data.ApplicationDbContext _context;

        public IndexModel(DotNetNote.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<CabinetType> CabinetType { get;set; }

        public async Task OnGetAsync()
        {
            CabinetType = await _context.CabinetTypes.ToListAsync();
        }
    }
}
