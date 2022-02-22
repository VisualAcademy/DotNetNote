#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DotNetNote.Data;
using DotNetNote.Models;

namespace DotNetNote.Pages.CabinetTypes
{
    public class IndexModel : PageModel
    {
        private readonly DotNetNote.Data.ApplicationDbContext _context;

        public IndexModel(DotNetNote.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<CabinetType> CabinetType { get;set; }

        public async Task OnGetAsync()
        {
            CabinetType = await _context.CabinetTypes.ToListAsync();
        }
    }
}
