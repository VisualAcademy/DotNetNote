#nullable disable
using Microsoft.AspNetCore.Mvc.RazorPages;
using Acts.Models;

namespace Acts.Pages.ActionCategories;

public class CreateModel : PageModel
{
    private readonly Acts.Models.ActContext _context;

    public CreateModel(ActContext context) => _context = context;

    public IActionResult OnGet() => Page();

    [BindProperty]
    public ActionCategory ActionCategory { get; set; }

    // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.ActionCategories.Add(ActionCategory);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
