#nullable disable
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotNetNote.Pages.CabinetTypes;

public class DetailsModel(Data.ApplicationDbContext context) : PageModel
{
    public CabinetType CabinetType { get; set; }

    public async Task<IActionResult> OnGetAsync(long? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        CabinetType = await context.CabinetTypes.FirstOrDefaultAsync(m => m.Id == id);

        if (CabinetType == null)
        {
            return NotFound();
        }
        return Page();
    }
}
