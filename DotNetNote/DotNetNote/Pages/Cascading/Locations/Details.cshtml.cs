#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DotNetNote.Data;
using VisualAcademy.Models;

namespace VisualAcademy.Pages.Cascading.Locations
{
    public class DetailsModel : PageModel
    {
        private readonly DotNetNote.Data.ApplicationDbContext _context;

        public DetailsModel(DotNetNote.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Location Location { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Location = await _context.Locations
                .Include(l => l.PropertyRef).FirstOrDefaultAsync(m => m.Id == id);

            if (Location == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
