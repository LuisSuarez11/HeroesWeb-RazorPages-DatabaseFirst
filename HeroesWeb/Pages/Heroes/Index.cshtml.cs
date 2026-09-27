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
    public class IndexModel : PageModel
    {
        private readonly HeroesWeb.Data.HeroesContext _context;

        public IndexModel(HeroesWeb.Data.HeroesContext context)
        {
            _context = context;
        }

        public IList<Heroes> Heroes { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Heroes = await _context.Heroes.ToListAsync();
        }
    }
}
