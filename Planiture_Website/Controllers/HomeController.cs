using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Planiture_Website.Models;

namespace Planiture_Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;


        public HomeController(
            ILogger<HomeController> logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        //Checks if user is logining for the first time
        
        public async Task<IActionResult> CheckUser()
        {
            var user = await _userManager.GetUserAsync(User);

            var Accessed = user.FirstAccessed;

            if(User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            if(User.IsInRole("Customer"))
            {
                if (Accessed == true)
                {
                    _logger.LogInformation("User's first login");
                    return RedirectToAction("AccountOptions");
                }
                else
                {
                    _logger.LogInformation("User logged in.");
                    return RedirectToAction("Index", "Profile");
                }
            }
            _logger.LogInformation("Error logging user in");
            return RedirectToAction("Index", "Home");
            
        }

        public IActionResult Menu()
        {
            return View();
        }

        public IActionResult Policy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ContactUs()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ContactUs(Feedback feedback)
        {
            //Stores user feedback to database
            _context.UserFeedback.Add(feedback);
            _logger.LogInformation("User Feedback stored");

            await _context.SaveChangesAsync();
            
            //Send Copy of User feedback to email

            
            //Success Message for User here


            return View();
        }

        public IActionResult InformationCenter()
        {
            return View();
        }

        public IActionResult OurTeam()
        {
            return View();
        }

        public IActionResult IM_Academy()
        {
            return View();
        }

        public IActionResult EarnLearn()
        {
            return View();
        }

        //Must have a user account with us to view
        //[Authorize(Roles = "Admin, Market_Analysis Rep_Access, Customer")]
        public IActionResult SignalPlans()
        {
            return View();
        }

        //Must have a user account with us to view
        //[Authorize(Roles = "Admin, Market_Analysis Rep_Access, Customer")]
        public IActionResult Stocks()
        {
            return View();
        }

        public IActionResult OurAchievements()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult CheckOut()
        {
            if(ModelState.IsValid)
            {

                return RedirectToAction("CheckOutConfirmed");
            }
            return View();
        }

        [Authorize]
        public IActionResult CheckOutConfirmed()
        {
            return View();
        }
        //Customer Account Application Proccess

        [Authorize]
        public IActionResult AccountOptions()
        {
            return View();
        }

        [Authorize]
        //[Authorize(Roles = "Admin, Golden20")]
        [HttpGet]
        public IActionResult Golden20Application()
        {
            return View();
        }

        [Authorize]
        //[Authorize(Roles = "Admin, Golden20")]
        [HttpPost]
        public async Task<IActionResult> Golden20Application(Investment_Info invest, Account_Info account)
        {
            if(ModelState.IsValid)
            {
                //Get the existing user from the database
                var user = await _userManager.GetUserAsync(User);

                //convert Ques1 & Ques2 to all Capital Letter
                invest.Ques1.ToUpper();
                invest.Ques2.ToUpper();

                //Append User signature to User Record
                user.Signature = invest.Signature;
                await _userManager.UpdateAsync(user);
                _logger.LogInformation("User signature appended");

                //Append UserID to User's investment Record
                invest.UserID = user.Id;
                invest.FormType = "Golden20 Account";
                _context.UserInvestment.Add(invest);
                _logger.LogInformation("User beneficiary and investment background added.");

                //Create User Account
                account.AccountName = user.FirstName + " " + " " + user.LastName; //concat user first and last name together and store it in AccountName
                account.AccountType = invest.FormType;
                account.AvailableBalance = 0.00f;
                account.ActualBalance = 0.00f;
                account.WithdrawalAmount = 0.00f;
                account.DepositAmount = 0.00f;
                account.OtherAccount = "N/A";
                account.UserID = user.Id;

                _context.UserAccount.Add(account);
                _logger.LogInformation("User golden20 account created.");

                await _context.SaveChangesAsync();

                return RedirectToAction("Golden20Profile", "Profile");
            }

            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult BasicAccount()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> BasicAccount(Investment_Info invest, Account_Info account)
        {
            if (ModelState.IsValid)
            {
                //Get the existing user from the database
                var user = await _userManager.GetUserAsync(User);

                //convert Ques1 & Ques2 to all Capital Letter
                invest.Ques1.ToUpper();
                invest.Ques2.ToUpper();

                //Append User signature to User Record
                user.Signature = invest.Signature;
                user.FirstAccessed = false;
                await _userManager.UpdateAsync(user);
                _logger.LogInformation("User signature appended and firstAccessed value updated");

                //Append UserID to User's investment Record
                invest.UserID = user.Id;
                invest.FormType = "Basic Account";
                _context.UserInvestment.Add(invest);
                _logger.LogInformation("User beneficiary and investment background added.");

                //Create User Account
                account.AccountName = user.FirstName + " " + " " + user.LastName; //concat user first and last name together and store it in AccountName
                account.AccountType = invest.FormType;
                account.AvailableBalance = 0.00f;
                account.ActualBalance = 0.00f;
                account.WithdrawalAmount = 0.00f;
                account.DepositAmount = 0.00f;
                account.OtherAccount = "N/A";
                account.UserID = user.Id;

                _context.UserAccount.Add(account);
                _logger.LogInformation("User basic account created.");

                await _context.SaveChangesAsync();
                
                return RedirectToAction("Index", "Profile");

            }
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult AdvancedAccount()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AdvancedAccount(Investment_Info invest, Account_Info account)
        {
            if (ModelState.IsValid)
            {
                //Get the existing user from the database
                var user = await _userManager.GetUserAsync(User);

                //convert Ques1 & Ques2 to all Capital Letter
                invest.Ques1.ToUpper();
                invest.Ques2.ToUpper();

                //Append User signature to User Record
                user.Signature = invest.Signature; 
                user.FirstAccessed = false;
                await _userManager.UpdateAsync(user);
                _logger.LogInformation("User signature appended and firstAccessed value updated");

                //Append UserID to User's investment Record
                invest.UserID = user.Id;
                invest.FormType = "Advanced Account";
                _context.UserInvestment.Add(invest);
                _logger.LogInformation("User beneficiary and investment background added.");

                //Create User Account
                account.AccountName = user.FirstName + " " + " " + user.LastName; //concat user first and last name together and store it in AccountName
                account.AccountType = invest.FormType;
                account.AvailableBalance = 0.00f;
                account.ActualBalance = 0.00f;
                account.WithdrawalAmount = 0.00f;
                account.DepositAmount = 0.00f;
                account.OtherAccount = "N/A";
                account.UserID = user.Id;

                _context.UserAccount.Add(account);
                _logger.LogInformation("User advanced account created.");

                await _context.SaveChangesAsync();

                return RedirectToAction("AdvancedProfile", "Profile");
            }

            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult StocksAccount()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> StocksAccount(Investment_Info invest, Account_Info account)
        {
            if(ModelState.IsValid)
            {
                //Get the existing user from the database
                var user = await _userManager.GetUserAsync(User);

                //Append User signature to User Record
                user.Signature = invest.Signature; 
                user.FirstAccessed = false;
                await _userManager.UpdateAsync(user);
                _logger.LogInformation("User signature appended and firstAccessed value updated");

                //Append UserID to User's investment Record
                invest.UserID = user.Id;
                invest.FormType = "Stocks Account";
                _context.UserInvestment.Add(invest);
                _logger.LogInformation("User beneficiary and investment background added.");

                //Create User Account
                account.AccountName = user.FirstName + " " + " " + user.LastName; //concat user first and last name together and store it in AccountName
                account.AccountType = invest.FormType;
                account.AvailableBalance = 0.00f;
                account.ActualBalance = 0.00f;
                account.WithdrawalAmount = 0.00f;
                account.DepositAmount = 0.00f;
                account.OtherAccount = "N/A";
                account.UserID = user.Id;

                _context.UserAccount.Add(account);
                _logger.LogInformation("User stocks account created.");

                await _context.SaveChangesAsync();

                return RedirectToAction("StocksProfile", "Profile");
            }

            return View();
        }

        //If the User Account is successfully added the following notification page will be displayed
        public IActionResult AccountConfirmed()
        {
            return View();
        }

        public IActionResult DailySignals()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Basic500 ()
        {

            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Gold2K()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult PLANTINUM5()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult MYXPLAN()
        {
            return View ();
        }

        [Authorize]
        [HttpGet]
        public IActionResult StocksPortfolio()
        {
            return View();
        }

        public IActionResult PlanitureAcademy()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
