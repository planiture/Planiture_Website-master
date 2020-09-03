using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Planiture_Website.Models;

namespace Planiture_Website.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<ProfileController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _HostEnvironment;

        public ProfileController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<ProfileController> logger,
            ApplicationDbContext context,
            IWebHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
            _HostEnvironment = hostEnvironment;
        }

        public SetPasswordClass SetNewPassword { get; set; }
        public Account_Info Account { get; set; }
        public CusTransaction Transaction { get; set; }


        public List<CusTransaction> GetTransaction()
        {
            var userid = _userManager.GetUserId(HttpContext.User);

            var translist = from trans in _context.UserTransaction
                       where trans.UserID == Convert.ToInt32(userid)
                       select trans;
            return translist.ToList();
        }

        public List<Account_Info> GetAccount()
        {
            var userid = _userManager.GetUserId(HttpContext.User);

            var acclist = from acc in _context.UserAccount
                            where acc.UserID == Convert.ToInt32(userid)
                            select acc;

            return acclist.ToList();
        }
        
        public List<ApplicationUser> GetUser()
        {
            var userid = _userManager.GetUserId(HttpContext.User);

            
            var list = from user in  _context.Users.ToList()
                       where user.Id == Convert.ToInt32(userid)
                       select user;

            return list.ToList();
        }

        //check which user profile to load
        /*public async Task<IActionResult> CheckProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            var account = _context.UserAccount.Where(a => a.UserID == user.Id);
            foreach(Account_Info acc in account)
            {
                if(acc.AccountType == "Basic Account")
                {
                    return RedirectToAction("Index", "Profile");
                }
                if(acc.AccountType == "Advanced Account")
                {
                    return RedirectToAction("AdvancedProfile", "Profile");
                }
                if(acc.AccountType == "Stocks Account")
                {
                    return RedirectToAction("StocksProfile", "Profile");
                }
            }

            _logger.LogInformation("ID's do not match");
            return RedirectToAction("Index", "Home");
        }*/


        //basic account user profile
        [HttpGet]
        public IActionResult Index()
        {
            var userid = _userManager.GetUserId(HttpContext.User);
            if(userid == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ApplicationUser user = _userManager.FindByIdAsync(userid).Result;

                dynamic dy = new ExpandoObject();
                dy.transactionList = GetTransaction();
                dy.accountList = GetAccount();
                dy.userlist = GetUser();
                //return View(user);
                return View(dy);
            }
            
        }

        public IActionResult PersonalInfo()
        {
            var userid = _userManager.GetUserId(HttpContext.User);
            if (userid == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ApplicationUser user = _userManager.FindByIdAsync(userid).Result;
                return View(user);
            }
        }

        //Edit userprofile
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            var model = new EditUser
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Occupation = user.Occupation,
                UserName = user.UserName,
                Gender = user.Gender,
                DOB = user.DOB,
                Signature = user.Signature,
                Address1 = user.Address1,
                Residency = user.Residency
            };
            return View(model);
            
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUser model)
        {
            var user = await _userManager.FindByIdAsync(model.Id.ToString());

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.UserName = model.UserName;
                user.Gender = model.Gender;
                user.DOB = model.DOB;
                user.Signature = model.Signature;
                user.Email = model.Email;
                user.Occupation = model.Occupation;
                user.Residency = model.Residency;

                var result = await _userManager.UpdateAsync(user);
                if(result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SetPasword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);

            if (hasPassword)
            {
                return RedirectToPage("./ChangePassword");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SetPassword()
        {
            if(!ModelState.IsValid)
            {
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }


            var addPasswordResult = await _userManager.AddPasswordAsync(user, SetNewPassword.NewPassword);
            if(!addPasswordResult.Succeeded)
            {
                foreach(var error in addPasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }

            await _signInManager.RefreshSignInAsync(user);

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            
            if(user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if(!hasPassword)
            {
                return RedirectToAction("SetPassword");
            }


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordClass change)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, change.OldPassword, change.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }
            return View();
        }

        public async Task<IActionResult> Transactions()
        {
            var user = await _userManager.GetUserAsync(User);
            var userid = _userManager.GetUserId(HttpContext.User);
            if (userid == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                dynamic dy = new ExpandoObject();
                dy.Transactionlist = GetTransaction();

                return View(dy);
            }
        }

        public async Task<IActionResult> Accounts()
        {
            var user = await _userManager.GetUserAsync(User);
            var userid = _userManager.GetUserId(HttpContext.User);
            if (userid == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                dynamic dy = new ExpandoObject();
                dy.Accountlist = GetAccount();

                return View(dy);
            }
        }

        [HttpGet]
        public IActionResult DepositWithdrawal()
        {
            //Check if this is a Deposit or Withdrawal

            //verify user information

            //verify user account information

            //Store Transaction to database

            //Email a copy of the Transaction 

            return View();
        }

        public IActionResult DepositConfirmed()
        {
            return View();
        }

        public IActionResult WithdrawalConfirmed()
        {
            return View();
        }

        public IActionResult SignalPlans()
        {
            return View();
        }

        public async Task<IActionResult> Idk()
        {
            var transactionlist = await _context.UserAccount.ToListAsync();
            return View(transactionlist);
        }
    }
}
