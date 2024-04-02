#nullable disable
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotNetNote.Pages.CabinetTypes;

public class EditModel(Data.ApplicationDbContext context) : PageModel
{
    [BindProperty]
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

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        context.Attach(CabinetType).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CabinetTypeExists(CabinetType.Id))
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

    private bool CabinetTypeExists(long id) => context.CabinetTypes.Any(e => e.Id == id);
}
