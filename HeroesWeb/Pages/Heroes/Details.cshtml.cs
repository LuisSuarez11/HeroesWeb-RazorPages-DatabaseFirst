using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HeroesWeb.Data;
using HeroesWeb.Models;

namespace HeroesWeb.Pages_Heroes
{
    public class DetailsModel : PageModel
    {
        private readonly HeroesWeb.Data.HeroesContext _context;

        public DetailsModel(HeroesWeb.Data.HeroesContext context)
        {
            _context = context;
        }

        public Heroes Heroes { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var heroes = await _context.Heroes.FirstOrDefaultAsync(m => m.Id == id);

            if (heroes is not null)
            {
                Heroes = heroes;

                return Page();
            }

            return NotFound();
        }
    }
}
