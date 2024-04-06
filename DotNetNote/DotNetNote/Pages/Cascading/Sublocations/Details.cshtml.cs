﻿#nullable disable
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VisualAcademy.Pages.Cascading.Sublocations
{
    public class DetailsModel : PageModel
    {
        private readonly DotNetNote.Data.ApplicationDbContext _context;

        public DetailsModel(DotNetNote.Data.ApplicationDbContext context) => _context = context;

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
            return Page();
        }
    }
}
