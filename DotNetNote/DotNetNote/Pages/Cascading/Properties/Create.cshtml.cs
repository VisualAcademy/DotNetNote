#nullable disable
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VisualAcademy.Models;

namespace VisualAcademy.Pages.Cascading.Properties;

public class CreateModel : PageModel
{
    private readonly DotNetNote.Data.ApplicationDbContext _context;

    public CreateModel(DotNetNote.Data.ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    [BindProperty]
    public Property Property { get; set; }

    // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Properties.Add(Property);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
