﻿#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DotNetNote.Data;
using DotNetNote.Models;

namespace DotNetNote.Pages.CabinetTypes
{
    public class CreateModel : PageModel
    {
        private readonly DotNetNote.Data.ApplicationDbContext _context;

        public CreateModel(DotNetNote.Data.ApplicationDbContext context)
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
}
