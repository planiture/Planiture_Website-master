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
    [Authorize(Roles = "Moderator, Admin, Super Admin")]
    public class ManageAccountModel : PageModel
    {
        public List<AccountsViewModel> Accounts { get; set; }
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public ManageAccountModel(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
            this.Accounts = new List<AccountsViewModel>();
        }

        public async Task OnGetAsync()
        {
            var accounts = await _context.UserAccount.ToListAsync();
            foreach(Account_Info account in accounts)
            {
                var thisViewModel = new AccountsViewModel();
                thisViewModel.AccountName = account.AccountName;
                thisViewModel.UserID = account.UserID;
                thisViewModel.AccountName = account.AccountName;
                thisViewModel.AccountType = account.AccountType;
                thisViewModel.AccountVersion = account.AccountVersion;
                thisViewModel.Capacity = account.Capacity;
                thisViewModel.AvailableBalance = account.AvailableBalance;
                thisViewModel.ActualBalance = account.ActualBalance;
                thisViewModel.WithdrawalAmount = account.WithdrawalAmount;
                thisViewModel.DepositAmount = account.DepositAmount;
                thisViewModel.Status = account.AccountStatus;
                Accounts.Add(thisViewModel);
            }
        }
    }
    public class AccountsViewModel
    {
        public int AccountNumber { get; set; }
        public int UserID { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public string AccountVersion { get; set; }
        public float Capacity { get; set; }
        public float AvailableBalance { get; set; }
        public float ActualBalance { get; set; }
        public float WithdrawalAmount { get; set; }
        public float DepositAmount { get; set; }
        public string Status { get; set; }
        //public IEnumerable<string> Roles { get; set; }
    }
}
