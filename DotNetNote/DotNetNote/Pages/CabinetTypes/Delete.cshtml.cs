﻿#nullable disable
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
    public class DeleteModel : PageModel
    {
        private readonly DotNetNote.Data.ApplicationDbContext _context;

        public DeleteModel(DotNetNote.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CabinetType CabinetType { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CabinetType = await _context.CabinetTypes.FirstOrDefaultAsync(m => m.Id == id);

            if (CabinetType == null)
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

            CabinetType = await _context.CabinetTypes.FindAsync(id);

            if (CabinetType != null)
            {
                _context.CabinetTypes.Remove(CabinetType);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
