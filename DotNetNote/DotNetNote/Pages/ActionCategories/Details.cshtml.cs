#nullable disable
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Acts.Models;

namespace Acts.Pages.ActionCategories;

public class DetailsModel(Acts.Models.ActContext context) : PageModel
{
    public ActionCategory ActionCategory { get; set; }

    public async Task<IActionResult> OnGetAsync(long? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        ActionCategory = await context.ActionCategories.FirstOrDefaultAsync(m => m.Id == id);

        if (ActionCategory == null)
        {
            return NotFound();
        }
        return Page();
    }
}
