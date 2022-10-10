#nullable disable
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VisualAcademy.Models;

namespace VisualAcademy.Pages.Cascading.Sublocations;

public class EditModel : PageModel
{
    private readonly DotNetNote.Data.ApplicationDbContext _context;

    public EditModel(DotNetNote.Data.ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Sublocation Sublocation { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Sublocation = await _context.Sublocations
            .Include(s => s.LocationRef).FirstOrDefaultAsync(m => m.Id == id);

        if (Sublocation == null)
        {
            return NotFound();
        }
       ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Id");
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

        _context.Attach(Sublocation).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
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

    private bool SublocationExists(int id)
    {
        return _context.Sublocations.Any(e => e.Id == id);
    }
}
