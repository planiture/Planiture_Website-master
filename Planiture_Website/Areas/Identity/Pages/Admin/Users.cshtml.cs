using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Planiture_Website.Models;

namespace Planiture_Website.Areas.Identity.Pages.Admin
{
    [Authorize(Roles = "Admin, Super Admin")]
    public class UsersModel : PageModel
    {
        public List<UsersViewModel> Users { get; set; }
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public UsersModel(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            this.Users = new List<UsersViewModel>();
        }

        public async Task OnGetAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            foreach (ApplicationUser user in users)
            {
                var thisViewModel = new UsersViewModel();
                thisViewModel.UserId = user.Id.ToString();
                thisViewModel.Email = user.Email;
                thisViewModel.FirstName = user.FirstName;
                thisViewModel.LastName = user.LastName;
                thisViewModel.Roles = await GetUserRoles(user);
                Users.Add(thisViewModel);
            }
        }
        private async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }


    }

    public class UsersViewModel
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
