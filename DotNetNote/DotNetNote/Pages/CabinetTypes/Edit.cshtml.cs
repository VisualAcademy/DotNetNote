﻿#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DotNetNote.Data;
using DotNetNote.Models;

namespace DotNetNote.Pages.CabinetTypes
{
    public class EditModel : PageModel
    {
        private readonly DotNetNote.Data.ApplicationDbContext _context;

        public EditModel(DotNetNote.Data.ApplicationDbContext context)
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

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(CabinetType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CabinetTypeExists(CabinetType.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool CabinetTypeExists(long id)
        {
            return _context.CabinetTypes.Any(e => e.Id == id);
        }
    }
}
