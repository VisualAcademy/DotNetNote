#nullable disable
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotNetNote.Pages.CabinetTypes;

public class CreateModel : PageModel
{
    private readonly DotNetNote.Data.ApplicationDbContext _context;

    public CreateModel(Data.ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    [BindProperty]
    public CabinetType CabinetType { get; set; }

    // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.CabinetTypes.Add(CabinetType);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
