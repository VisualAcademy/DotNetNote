#nullable disable
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VisualAcademy.Pages.Cascading.Locations;

public class CreateModel : PageModel
{
    private readonly DotNetNote.Data.ApplicationDbContext _context;

    public CreateModel(DotNetNote.Data.ApplicationDbContext context) => _context = context;

    public IActionResult OnGet()
    {
        ViewData["PropertyId"] = new SelectList(_context.Properties, "Id", "Name");
        return Page();
    }

    [BindProperty]
    public Location Location { get; set; }

    // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Locations.Add(Location);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
