#nullable disable
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VisualAcademy.Pages.Cascading.Sublocations;

public class DeleteModel(DotNetNote.Data.ApplicationDbContext context) : PageModel
{
    [BindProperty]
    public Sublocation Sublocation { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Sublocation = await context.Sublocations
            .Include(s => s.LocationRef).FirstOrDefaultAsync(m => m.Id == id);

        if (Sublocation == null)
        {
            return NotFound();
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Sublocation = await context.Sublocations.FindAsync(id);

        if (Sublocation != null)
        {
            context.Sublocations.Remove(Sublocation);
            await context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}
