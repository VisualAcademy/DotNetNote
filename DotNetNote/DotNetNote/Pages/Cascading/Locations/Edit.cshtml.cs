#nullable disable
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VisualAcademy.Models;

namespace VisualAcademy.Pages.Cascading.Locations;

public class EditModel : PageModel
{
    private readonly DotNetNote.Data.ApplicationDbContext _context;

    public EditModel(DotNetNote.Data.ApplicationDbContext context) => _context = context;

    [BindProperty]
    public Location Location { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Location = await _context.Locations
            .Include(l => l.PropertyRef).FirstOrDefaultAsync(m => m.Id == id);

        if (Location == null)
        {
            return NotFound();
        }
       ViewData["PropertyId"] = new SelectList(_context.Properties, "Id", "Id");
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

        _context.Attach(Location).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!LocationExists(Location.Id))
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

    private bool LocationExists(int id) => _context.Locations.Any(e => e.Id == id);
}
