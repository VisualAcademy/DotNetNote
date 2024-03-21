#nullable disable
using Microsoft.AspNetCore.Mvc.RazorPages;
using Acts.Models;

namespace Acts.Pages.ActionCategories;

public class CreateModel(ActContext context) : PageModel
{
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

        context.ActionCategories.Add(ActionCategory);
        await context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
