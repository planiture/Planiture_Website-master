using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using Planiture_Website.Models;

namespace Planiture_Website.Areas.Identity.Pages.Admin
{
    [Authorize(Roles = "Moderator, Admin, Super Admin")]
    public class UserAccountsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IToastNotification _toastNotification;
        private readonly ApplicationDbContext _context;

        public UserAccountsModel(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IToastNotification toastNotification,
            ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _context = context;
            this._toastNotification = toastNotification;
            _userManager = userManager;
        }

        [BindProperty]
        public List<UserRolesViewModel> UserAccounts { get; set; }
        [BindProperty]
        public int AccountNumber { get; set; }
        [BindProperty]
        public string UserName { get; set; }

        public async Task OnGet(int accountNumber)
        {


            UserAccounts = new List<UserRolesViewModel>();
            AccountNumber = accountNumber;

            var account = _context.UserAccount.Where(a => a.AccountNumber == accountNumber).FirstOrDefault();

            /*var account = from acc in _context.UserAccount.ToList()
                          where acc.AccountNumber == accountNumber
                          select acc;*/

            if (account == null)
            {
                return;
            }
            UserName = account.AccountName;

            foreach(var acc in _context.UserAccount.ToList())
            {
                var userRolesViewModel = new UserRolesViewModel
                {
                    AccountNumber = acc.AccountNumber,
                    UserID = acc.UserID,
                    AccountName = acc.AccountName,
                    AccountType = acc.AccountType,
                    AccountVersion = acc.AccountVersion,
                    Capacity = acc.Capacity,
                    AvailableBalance = acc.AvailableBalance,
                    ActualBalance = acc.ActualBalance,
                    WithdrawalAmount = acc.WithdrawalAmount,
                    DepositAmount = acc.DepositAmount,
                    Status = acc.AccountStatus
                };
                UserAccounts.Add(userRolesViewModel);
            }
        }


    }

    public class UserRolesViewModel
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
    }
}
