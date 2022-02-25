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
    public class IndexModel : PageModel
    {
        private readonly Acts.Models.ActContext _context;

        public IndexModel(Acts.Models.ActContext context)
        {
            _context = context;
        }

        public IList<ActionCategory> ActionCategory { get;set; }

        public async Task OnGetAsync()
        {
            ActionCategory = await _context.ActionCategories.ToListAsync();
        }
    }
}
