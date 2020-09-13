using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using EllipticCurve.Utils;
using IronPdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Planiture_Website.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

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

        public SetPasswordViewModel SetNewPassword { get; set; }
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

        public List<ActivePlans> Plans()
        {
            var userid = _userManager.GetUserId(HttpContext.User);

            var list = from user in _context.ActivePlans.ToList()
                       where user.UserID == Convert.ToInt32(userid)
                       select user;

            return list.ToList();
        }

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

            var model = new EditUserViewModel
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
        public async Task<IActionResult> Edit(EditUserViewModel model)
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

        /*[HttpGet]
        public async Task<IActionResult> UploadPhoto()
        {

        }*/

        [HttpGet]
        public async Task<IActionResult> DeletePhoto(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var currentuser = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);

            var image = (from m in _context.Users
                         where m.Id == id
                         select m.Identity).FirstOrDefault();

            if (image == null)
            {
                return NotFound();
            }

            return View(currentuser);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var userimage = await _context.Users.FindAsync(id);

            //delete from wwwRoot image folder
            var imagePath = Path.Combine(_HostEnvironment.WebRootPath, "image", userimage.Identity);
            //check if image exists then delete
            if(System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            var user = await _userManager.GetUserAsync(User);
            var image = "";

            

            user.Identity = image;
            _context.Users.Update(user);
            _context.SaveChanges();

            return View();
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
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel change)
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

        [Authorize]
        [HttpGet]
        public IActionResult Withdrawal()
        {
            //Check if this is a Deposit or Withdrawal

            //verify user information

            //verify user account information

            //Store Transaction to database

            //Email a copy of the Transaction 

            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Withdrawal(DepositWithdrawalClass withdrawal, CusTransaction transaction)
        {
            //Get the existing user from the database
            var user = await _userManager.GetUserAsync(User);

            //check user against database
            List<string> str = new List<string>();

            using (SqlConnection con = new SqlConnection(@"Data Source=MSI;Initial Catalog=Planiture_Records;Integrated Security=True"))
            {

                SqlCommand cmd = new SqlCommand($"select AccountNumber, ActualBalance, AccountName from UserAccount where UserID='{user.Id}'", con);
                con.Open();



                using (SqlDataReader da = cmd.ExecuteReader())
                {
                    while (da.Read())
                    {
                        str.Add(da[0].ToString());
                        str.Add(da[1].ToString());
                        str.Add(da[2].ToString());
                    }

                }

                con.Close();
            }


            if (withdrawal.Name != str[2])
            {
                ViewBag.NameMsg = "Invalid Account Name";
                return View();
            }
            if (withdrawal.AccountNumber != Convert.ToInt32(str[0]))
            {
                ViewBag.NumMsg = "Invalid Account Number";
                return View();
            }
            if(withdrawal.WithdrawalAmount > Convert.ToInt32(str[1]))
            {
                ViewBag.AmtMsg = "Insufficient Funds on account";
                return View();
            }

            //store copy of user transaction to database
            transaction.TransactionAmount = withdrawal.WithdrawalAmount;
            transaction.TransactionType = "Withdrawal";
            transaction.Trans_AccountNumber = withdrawal.AccountNumber;
            transaction.UserID = user.Id;
            transaction.TransactionStatus = "Pending";
            transaction.MostRecent = withdrawal.WithdrawalAmount;

            _context.UserTransaction.Add(transaction);
            _context.SaveChanges();
            _logger.LogInformation("User withdrawal processing");

            //email copy of user transaction to company email
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("akeamsmith41@gmail.com");
            var to = new EmailAddress("akeamsmith41@gmail.com");
            string subject = "User Withdrawal Transaction Information";
            string htmlContent = "<style> table,tr,td{ border:1px solid black;} tr,td{padding:20px; }</style>" +
                "<img src='~/css/website logo.jpg'/>" +
                "<h1>"+withdrawal.Name+ ", WITHDRAWAL Information </h1>" +
                "<hr /> " +
                "<table> " +
                "<tr> " +
                "<td>User ID<td>" +
                "<td>User Name<td>" +
                "<td>Account Number<td>" +
                "<td>Withdrawal Amount<td>" +
                "<td>Account Type<td>" +
                "<td>Transaction Type<td>" +
                "<td>Transaction Status<td>" +
                "</tr>" +
                "<tr> " +
                "<td>"+user.Id+"</td>" +
                "<td>"+withdrawal.Name+"</td>" +
                "<td>"+withdrawal.AccountNumber+"</td>" +
                "<td>"+withdrawal.WithdrawalAmount+"</td>" +
                "<td>"+withdrawal.AccountType+"</td>" +
                "<td>Withdrawal</td>" +
                "<td>Pending</td>" +
                "</tr>" +
                "</table>";

            //create pdf file
            var pdf = HtmlToPdf.StaticRenderHtmlAsPdf(htmlContent);
            pdf.SaveAs("" + withdrawal.Name + "_Withdrawal_Request_Information.pdf");
            byte[] bytes = Encoding.ASCII.GetBytes(htmlContent);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
            msg.AddAttachment(withdrawal.Name+"_Withdrawal_Request_Information.pdf",Convert.ToBase64String(bytes), "application/pdf", "attachment",null);
            
            var response = client.SendEmailAsync(msg);

            return RedirectToAction("WithdrawalConfirmed", "Profile");
        }

        public IActionResult WithdrawalConfirmed()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> MyPlans()
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
                dy.planlist = Plans();

                return View(dy);
            }
        }


    }
}
