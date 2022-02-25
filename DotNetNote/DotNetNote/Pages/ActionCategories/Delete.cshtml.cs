#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Acts.Models;

namespace Acts.Pages.ActionCategories
{
    public class DeleteModel : PageModel
    {
        private readonly Acts.Models.ActContext _context;

        public DeleteModel(Acts.Models.ActContext context)
        {
            _context = context;
        }

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

        public async Task<IActionResult> OnPostAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ActionCategory = await _context.ActionCategories.FindAsync(id);

            if (ActionCategory != null)
            {
                _context.ActionCategories.Remove(ActionCategory);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
