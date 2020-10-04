using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Planiture_Website.Models;

namespace Planiture_Website.Areas.Identity.Pages.Admin
{
    public class AccountsModel : PageModel
    {
        public List<Account_Info> GetAccount { get; set; }
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        [BindProperty(SupportsGet = true)]
        public string searchTerm { get; set; }

        public AccountsModel(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            this.GetAccount = new List<Account_Info>();
        }

        public void OnGet()
        {
            GetAccount = _context.UserAccount.ToList();
        }

        public async Task OnGetSearchAsync()
        {
            var list = from s in _context.UserAccount
                       select s;

            if (!String.IsNullOrEmpty(searchTerm))
            {
                list = list.Where(m => m.AccountName.Contains(searchTerm) || m.AccountType.Contains(searchTerm) || m.AccountStatus.Contains(searchTerm) || m.AccountVersion.Contains(searchTerm) ||
                        m.UserID.ToString().Contains(searchTerm) || m.AccountNumber.ToString().Contains(searchTerm));
            }

            GetAccount = list.ToList();
        }
    }
}
