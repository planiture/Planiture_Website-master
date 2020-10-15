using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
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
using Planiture_Website.ViewModels;
using Microsoft.EntityFrameworkCore;
using SendGrid;
using SendGrid.Helpers.Mail;

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
                Accessed = false;

                return RedirectToAction("Index", "Home");
            }
            if(User.IsInRole("Customer-Account"))
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
            if(User.IsInRole("Customer-Profile"))
            {
                if (Accessed == true)
                {
                    _logger.LogInformation("User's first login");
                    return RedirectToAction("ProfileApplication", "Account");
                }
                else
                {
                    _logger.LogInformation("User logged in.");
                    return RedirectToAction("Index", "Account");
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
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("akeamsmith41@gmail.com");
            var to = new EmailAddress("akeamsmith41@gmail.com");
            string subject = "Planiture Customer Feedback / Report";
            string htmlContent = "Customer Feedback / Report: <br /><br /> From: "+feedback.FullName+"<br />Investment Number: "+feedback.Investment_Number+ "<br />Subject: "+feedback.Subject+ "<br />Message: " + feedback.Message;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
            var response = client.SendEmailAsync(msg);

            //Success Message for User here
            return RedirectToAction("FeedbackConfirm", "Home");
        }

        public IActionResult FeedbackConfirm()
        {
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

        public IActionResult SignalPlans()
        {
            return View();
        }

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
        [HttpPost]
        public async Task<IActionResult> AccountOptions(TokenViewModel model, Token token)
        {
            //find the status of the token entered
            var check = (from m in _context.UserToken
                         where m.Code == model.Code
                         select m.Status).SingleOrDefault();

            //check if the token was already used
            if(check == "Used")
            {
                ViewBag.UsedMsg = "Promotion Code was already used!";
                return View();
            }

            var code = (from m in _context.UserToken
                       where m.Code == model.Code
                       select m.Code).SingleOrDefault();

            //if the token was not used then add it to the user's ID
            if(code == model.Code)
            {
                var user = await _userManager.GetUserAsync(User);
                var userid = _userManager.GetUserId(User);
                List<string> str = new List<string>();


                using (SqlConnection con = new SqlConnection(@"Data Source=MSI;Initial Catalog=Planiture_Records;Integrated Security=True"))
                {
                    SqlCommand cmd = new SqlCommand($"select * from UserToken where Code='{model.Code}'", con);
                    con.Open();

                    

                    using (SqlDataReader da = cmd.ExecuteReader())
                    {
                        while (da.Read())
                        {
                            str.Add(da[0].ToString());
                            str.Add(da[1].ToString());
                        }

                    }

                    con.Close();
                }

                token.ID = Convert.ToInt32(str[0]);
                token.Name = "testing";
                token.Code = model.Code;
                token.UsedBy = userid;
                token.Status = "Used";

                _context.UserToken.Update(token);
                _context.SaveChanges();
                _logger.LogInformation("User token used");

                return RedirectToAction("Golden20Application", "Home");
            }

            ViewBag.ErrorMsg = "Invalid Promotion Code Entered!";
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Golden20Application()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Golden20Application(Investment_Info invest, Account_Info account)
        {
            if(ModelState.IsValid)
            {
                //Get the existing user from the database
                var user = await _userManager.GetUserAsync(User);

                //check if user already filled out the investment Questions
                var entity = _context.UserInvestment.FirstOrDefault(i => i.UserID == user.Id);

                if (entity != null)
                {
                    //update user investment question

                    //convert Ques1 & Ques2 to all Capital Letter
                    invest.Ques1 = invest.Ques1.ToUpper();
                    invest.Ques2 = invest.Ques2.ToUpper();

                    //Append User signature to User Record
                    user.Signature = invest.Signature;
                    user.FirstAccessed = false;
                    await _userManager.UpdateAsync(user);
                    _logger.LogInformation("User signature appended updated");

                    //Append UserID to User's investment Record
                    entity.FormType = "Golden20 Account";

                    _context.UserInvestment.Update(entity);
                    _context.SaveChanges();
                    _logger.LogInformation("User investment info Updated");
                }
                else
                {
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
                    invest.FormType = "Golden20 Account";
                    _context.UserInvestment.Add(invest);
                    _logger.LogInformation("User beneficiary and investment background added.");
                }


                //check if user already has an account with us
                var check = _context.UserAccount.FirstOrDefault(i => i.UserID == user.Id);

                if (check != null)
                {
                    //update user account

                    check.AccountName = user.FirstName + " " + " " + user.LastName; //concat user first and last name together and store it in AccountName
                    check.AccountType = "Investments";
                    check.AccountVersion = invest.FormType;
                    check.AvailableBalance = 0.00f;
                    check.ActualBalance = 0.00f;
                    check.WithdrawalAmount = 0.00f;
                    check.DepositAmount = 0.00f;
                    check.AccountStatus = "Active";
                    //account.UserID = user.Id;

                    _context.UserAccount.Update(check);
                    _context.SaveChanges();
                    _logger.LogInformation("User Account Updated");
                }
                else
                {
                    //Create User Account
                    account.AccountName = user.FirstName + " " + " " + user.LastName; //concat user first and last name together and store it in AccountName
                    account.AccountType = "Investments & Stocks";
                    account.AccountVersion = invest.FormType;
                    account.AvailableBalance = 0.00f;
                    account.ActualBalance = 0.00f;
                    account.WithdrawalAmount = 0.00f;
                    account.DepositAmount = 0.00f;
                    account.AccountStatus = "Active";
                    account.UserID = user.Id;

                    _context.UserAccount.Add(account);
                    _logger.LogInformation("User golden20 account created.");

                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Index", "Profile");
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

                //check if user already filled out the investment Questions
                var entity = _context.UserInvestment.FirstOrDefault(i => i.UserID == user.Id);

                if (entity != null)
                {
                    //update user investment question

                    //Append User signature to User Record
                    user.Signature = invest.Signature;
                    user.FirstAccessed = false;
                    await _userManager.UpdateAsync(user);
                    _logger.LogInformation("User signature appended updated");

                    //Append UserID to User's investment Record
                    entity.FormType = "Basic Account";

                    _context.UserInvestment.Update(entity);
                    _context.SaveChanges();
                    _logger.LogInformation("User investment info Updated");
                }
                else
                {

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
                }


                //check if user already has an account with us
                var check = _context.UserAccount.FirstOrDefault(i => i.UserID == user.Id);

                if (check != null)
                {
                    //update user account

                    check.AccountName = user.FirstName + " " + " " + user.LastName; //concat user first and last name together and store it in AccountName
                    check.AccountType = "Investments";
                    check.AccountVersion = invest.FormType;
                    check.AvailableBalance = 0.00f;
                    check.ActualBalance = 0.00f;
                    check.WithdrawalAmount = 0.00f;
                    check.DepositAmount = 0.00f;
                    check.AccountStatus = "Active";
                    //account.UserID = user.Id;

                    _context.UserAccount.Update(check);
                    _context.SaveChanges();
                    _logger.LogInformation("User Account Updated");
                }
                else
                {
                    //Create User Account
                    account.AccountName = user.FirstName + " " + " " + user.LastName; //concat user first and last name together and store it in AccountName
                    account.AccountType = "Investments";
                    account.AccountVersion = invest.FormType;
                    account.AvailableBalance = 0.00f;
                    account.ActualBalance = 0.00f;
                    account.WithdrawalAmount = 0.00f;
                    account.DepositAmount = 0.00f;
                    account.AccountStatus = "Active";
                    account.UserID = user.Id;

                    _context.UserAccount.Add(account);
                    _logger.LogInformation("User basic account created.");

                    await _context.SaveChangesAsync();
                }

                
                
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

                //check if user already filled out the investment Questions
                var entity = _context.UserInvestment.FirstOrDefault(i => i.UserID == user.Id);

                if (entity != null)
                {
                    //update user investment question
                    //convert Ques1 & Ques2 to all Capital Letter
                    entity.Ques1 = invest.Ques1.ToUpper();
                    entity.Ques2 = invest.Ques2.ToUpper();

                    //Append User signature to User Record
                    user.Signature = invest.Signature;
                    user.FirstAccessed = false;
                    await _userManager.UpdateAsync(user);
                    _logger.LogInformation("User signature appended updated");

                    entity.FormType = "Advanced Account";

                    _context.UserInvestment.Update(entity);
                    _context.SaveChanges();
                    _logger.LogInformation("User investment info Updated");
                }
                else
                {
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
                }


                //check if user already has an account with us
                var check = _context.UserAccount.FirstOrDefault(i => i.UserID == user.Id);

                if (check != null)
                {
                    //update user account

                    check.AccountName = user.FirstName + " " + " " + user.LastName; //concat user first and last name together and store it in AccountName
                    check.AccountType = "Investments & Stocks";
                    check.AccountVersion = invest.FormType;
                    check.AvailableBalance = 0.00f;
                    check.ActualBalance = 0.00f;
                    check.WithdrawalAmount = 0.00f;
                    check.DepositAmount = 0.00f;
                    check.AccountStatus = "Active";

                    _context.UserAccount.Update(check);
                    _context.SaveChanges();
                    _logger.LogInformation("User Account Upgraded");
                }
                else
                {
                    //Create User Account
                    account.AccountName = user.FirstName + " " + " " + user.LastName; //concat user first and last name together and store it in AccountName
                    account.AccountType = "Investments & Stocks";
                    account.AccountVersion = invest.FormType;
                    account.AvailableBalance = 0.00f;
                    account.ActualBalance = 0.00f;
                    account.WithdrawalAmount = 0.00f;
                    account.DepositAmount = 0.00f;
                    account.AccountStatus = "Active";
                    account.UserID = user.Id;

                    _context.UserAccount.Add(account);
                    _logger.LogInformation("User advanced account created.");

                    await _context.SaveChangesAsync();
                }


                return RedirectToAction("Index", "Profile");
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

                //check if user already filled out the investment Questions
                var entity = _context.UserInvestment.FirstOrDefault(i => i.UserID == user.Id);

                if (entity != null)
                {
                    //convert Ques1 & Ques2 to all Capital Letter
                    entity.Ques1 = invest.Ques1.ToUpper();
                    entity.Ques2 = invest.Ques2.ToUpper();

                    //Append User signature to User Record
                    user.Signature = invest.Signature;
                    user.FirstAccessed = false;
                    await _userManager.UpdateAsync(user);
                    _logger.LogInformation("User signature appended updated");

                    //Append UserID to User's investment Record
                    entity.FormType = "Stocks Account";

                    _context.UserInvestment.Update(entity);
                    _context.SaveChanges();
                    _logger.LogInformation("User investment info Updated");
                }
                else
                {
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
                    invest.FormType = "Stocks Account";
                    _context.UserInvestment.Add(invest);
                    _logger.LogInformation("User beneficiary and investment background added.");
                }


                //check if user already has an account with us
                var check = _context.UserAccount.FirstOrDefault(i => i.UserID == user.Id);

                if (check != null)
                {
                    //Create User Account
                    check.AccountName = user.FirstName + " " + " " + user.LastName; //concat user first and last name together and store it in AccountName
                    check.AccountType = "Stocks";
                    check.AccountVersion = invest.FormType;
                    check.AvailableBalance = 0.00f;
                    check.ActualBalance = 0.00f;
                    check.WithdrawalAmount = 0.00f;
                    check.DepositAmount = 0.00f;
                    check.AccountStatus = "Active";
                    //account.UserID = user.Id;

                    _context.UserAccount.Update(check);
                    _context.SaveChanges();
                    _logger.LogInformation("User Account Upgraded");
                }
                else
                {
                    //Create User Account
                    account.AccountName = user.FirstName + " " + " " + user.LastName; //concat user first and last name together and store it in AccountName
                    account.AccountType = "Stocks";
                    account.AccountVersion = invest.FormType;
                    account.AvailableBalance = 0.00f;
                    account.ActualBalance = 0.00f;
                    account.WithdrawalAmount = 0.00f;
                    account.DepositAmount = 0.00f;
                    account.AccountStatus = "Active";
                    account.UserID = user.Id;

                    _context.UserAccount.Add(account);
                    _logger.LogInformation("User stocks account created.");

                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Index", "Profile");
            }

            return View();
        }

        //If the User Account is successfully added the following notification page will be displayed
        public IActionResult AccountConfirmed()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult APLAN ()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> APLAN(ActivePlans plans)
        {
            var user = await _userManager.GetUserAsync(User);

            plans.PlanName = "A-PLAN";
            plans.UserID = user.Id;
            plans.DateExpired = Convert.ToDateTime("21/09/2020");
            _context.ActivePlans.Update(plans);
            _context.SaveChanges();
            return RedirectToAction("PlanConfirm", "Home");
        }

        [Authorize]
        [HttpGet]
        public IActionResult BPLAN()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> BPLAN(ActivePlans plans)
        {
            var user = await _userManager.GetUserAsync(User);

            plans.PlanName = "B-PLAN";
            plans.UserID = user.Id;
            plans.DateExpired = Convert.ToDateTime("28/09/2020");
            _context.ActivePlans.Update(plans);
            _context.SaveChanges();
            return RedirectToAction("PlanConfirm", "Home");
        }

        [Authorize]
        [HttpGet]
        public IActionResult CPLAN()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CPLAN(ActivePlans plans)
        {
            var user = await _userManager.GetUserAsync(User);

            plans.PlanName = "C-PLAN";
            plans.UserID = user.Id;
            plans.DateExpired = Convert.ToDateTime("12/10/2020");
            _context.ActivePlans.Update(plans);
            _context.SaveChanges();
            return RedirectToAction("PlanConfirm", "Home");
        }

        [Authorize]
        [HttpGet]
        public IActionResult MYXPLAN()
        {
            return View ();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> MYXPLAN(ActivePlans plans)
        {
            var user = await _userManager.GetUserAsync(User);

            plans.PlanName = "MYXPLAN";
            plans.UserID = user.Id;
            plans.DateExpired = DateTime.Now.AddMonths(3);
            _context.ActivePlans.Update(plans);
            _context.SaveChanges();
            return RedirectToAction("PlanConfirm", "Home");
        }

        [Authorize]
        [HttpGet]
        public IActionResult PlanConfirm()
        {
            
            return View();
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

        public IActionResult ForExCourse()
        {
            return View();
        }
        public IActionResult ForExVideos()
        {
            return View();
        }
        public IActionResult ForExContent()
        {
            return View();
        }

        public IActionResult AboutUs()
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
