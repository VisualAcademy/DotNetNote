#nullable disable
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotNetNote.Pages.CabinetTypes;

public class CreateModel(Data.ApplicationDbContext context) : PageModel
{
    public IActionResult OnGet() => Page();

    [BindProperty]
    public CabinetType CabinetType { get; set; }

    // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        context.CabinetTypes.Add(CabinetType);
        await context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
