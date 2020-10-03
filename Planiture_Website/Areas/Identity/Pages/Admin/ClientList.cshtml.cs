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
    public class ClientListModel : PageModel
    {
        public List<ApplicationUser> GetUser { get; set; }
        //public List<Investment_Info> Invest { get; set; }

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClientListModel(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            this.GetUser = new List<ApplicationUser>();
            //this.Invest = new List<Investment_Info>();
        }

        public async Task OnGetAsync()
        {
            //var userlist = await _userManager.GetUsersInRoleAsync("Customer");

            GetUser = _userManager.Users.ToList();
        }
    }
}
