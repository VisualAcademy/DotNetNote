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

namespace VisualAcademy.Pages.Cascading.Sublocations
{
    public class IndexModel : PageModel
    {
        private readonly DotNetNote.Data.ApplicationDbContext _context;

        public IndexModel(DotNetNote.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Sublocation> Sublocation { get;set; }

        public async Task OnGetAsync()
        {
            Sublocation = await _context.Sublocations
                .Include(s => s.LocationRef).ToListAsync();
        }
    }
}
