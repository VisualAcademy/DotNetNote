#nullable disable
using Microsoft.AspNetCore.Mvc.RazorPages;
using Acts.Models;

namespace Acts.Pages.ActionCategories;

public class EditModel : PageModel
{
    private readonly Acts.Models.ActContext _context;

    public EditModel(Acts.Models.ActContext context) => _context = context;

    [BindProperty]
    public ActionCategory ActionCategory { get; set; }

    public async Task<IActionResult> OnGetAsync(long? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        ActionCategory = await _context.ActionCategories.FirstOrDefaultAsync(m => m.Id == id);

        if (ActionCategory == null)
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

        _context.Attach(ActionCategory).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ActionCategoryExists(ActionCategory.Id))
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

    private bool ActionCategoryExists(long id) => _context.ActionCategories.Any(e => e.Id == id);
}
