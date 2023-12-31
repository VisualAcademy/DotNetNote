#nullable disable
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DotNetNote.Pages.CabinetTypes;

public class IndexModel(Data.ApplicationDbContext context) : PageModel
{
    public IList<CabinetType> CabinetType { get;set; }

    public async Task OnGetAsync() => CabinetType = await context.CabinetTypes.ToListAsync();
}
