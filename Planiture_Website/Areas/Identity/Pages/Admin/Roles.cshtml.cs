using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Planiture_Website.Models;

namespace Planiture_Website.Areas.Identity.Pages.Admin
{
    public class RolesModel : PageModel
    {
        public List<ApplicationRole> Roles { get; set; }
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public RolesModel(RoleManager<ApplicationRole> roleManager,
            ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _context = context;
            this.Roles = new List<ApplicationRole>();
        }

        public async Task OnGet()
        {
            Roles = _roleManager.Roles.ToList();
        }
        public async Task OnPostAsync(string roleName)
        {
            if (roleName != null)
            {
                var role = new ApplicationRole();
                role.Name = roleName.Trim();

                await _roleManager.CreateAsync(role);
            }
            await OnGet();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);

            if(role != null)
            {
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}
