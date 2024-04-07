#nullable disable
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VisualAcademy.Pages.Cascading.Sublocations;

public class EditModel(DotNetNote.Data.ApplicationDbContext context) : PageModel
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
       ViewData["LocationId"] = new SelectList(context.Locations, "Id", "Id");
        return Page();
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        context.Attach(Sublocation).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SublocationExists(Sublocation.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToPage("./Index");
    }

    private bool SublocationExists(int id) => context.Sublocations.Any(e => e.Id == id);
}
