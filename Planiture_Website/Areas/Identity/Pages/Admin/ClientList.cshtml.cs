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
        public List<ApplicationUser> Clients { get; set; }

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        [BindProperty(SupportsGet = true)]
        public string searchTerm {get; set; }

        public ClientListModel(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            this.GetUser = new List<ApplicationUser>();
            //this.Clients = new List<ApplicationUser>();
        }

        public async Task OnGetAsync()
        {
            //var userlist = await _userManager.GetUsersInRoleAsync("Customer");

            GetUser = _userManager.Users.ToList();
        }

        public async Task OnGetSearchAsync()
        {
            var list = from s in _context.Users
                       select s;

            if(!String.IsNullOrEmpty(searchTerm))
            {
                list = list.Where(m => m.FirstName.Contains(searchTerm) || m.LastName.Contains(searchTerm) || m.Email.Contains(searchTerm) || m.Gender.Contains(searchTerm) ||
                m.UserName.Contains(searchTerm) || m.Id.ToString().Contains(searchTerm));
            }

            GetUser = list.ToList();
        }
    }
}
