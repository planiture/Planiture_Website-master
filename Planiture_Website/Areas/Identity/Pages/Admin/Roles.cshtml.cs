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
        public List<ApplicationUser> GetUser { get; set; }
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public RolesModel(RoleManager<ApplicationRole> roleManager,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
            this.Roles = new List<ApplicationRole>();
            this.GetUser = new List<ApplicationUser>();
        }
        public async Task OnGet()
        {
            Roles = _roleManager.Roles.ToList();
            //GetUser = _userManager.Users.ToList();
            //var Customers
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
    }
}
