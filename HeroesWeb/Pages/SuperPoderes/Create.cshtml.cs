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

namespace HeroesWeb.Pages_SuperPoderes
{
    public class CreateModel : PageModel
    {
        private readonly HeroesWeb.Data.HeroesContext _context;

        public CreateModel(HeroesWeb.Data.HeroesContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["HeroeId"] = new SelectList(_context.Heroes, "Id", "Nombre");
            return Page();
        }

        [BindProperty]
        public SuperPoderes SuperPoderes { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!await _context.Heroes.AnyAsync(h => h.Id == SuperPoderes.HeroeId))
            {
                ModelState.AddModelError("SuperPoderes.HeroeId", "Seleccione un héroe válido.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["HeroeId"] = new SelectList(
                    _context.Heroes, "Id", "Nombre", SuperPoderes.HeroeId);
                return Page();
            }

            _context.SuperPoderes.Add(SuperPoderes);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
