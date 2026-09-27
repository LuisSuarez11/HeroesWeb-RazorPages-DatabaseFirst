using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HeroesWeb.Data;
using HeroesWeb.Models;

namespace HeroesWeb.Pages_Heroes
{
    public class EditModel : PageModel
    {
        private readonly HeroesWeb.Data.HeroesContext _context;

        public EditModel(HeroesWeb.Data.HeroesContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Heroes Heroes { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var heroes =  await _context.Heroes.FirstOrDefaultAsync(m => m.Id == id);
            if (heroes == null)
            {
                return NotFound();
            }
            Heroes = heroes;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Heroes).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HeroesExists(Heroes.Id))
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

        private bool HeroesExists(int id)
        {
            return _context.Heroes.Any(e => e.Id == id);
        }
    }
}
