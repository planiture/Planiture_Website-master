using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Planiture_Website.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Planiture_Website.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<ProfileController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger<ProfileController> logger,
            ApplicationDbContext context,
            IWebHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            //Personal Information
            [PersonalData]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [PersonalData]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [PersonalData]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy HH:mm}")]
            public DateTime MemberSince { get; set; }
            
            [PersonalData]
            [Display(Name = "Mobile")]
            public string Mobile { get; set; }

            [PersonalData]
            [EmailAddress]
            public string Email { get; set; }

            [PersonalData]
            [Display(Name = "Address 1")]
            public string Address1 { get; set; }

            [PersonalData]
            [Display(Name = "Address 2")]
            public string Address2 { get; set; }

            [PersonalData]
            [Display(Name = "Residency")]
            public string Residency { get; set; }

            [PersonalData]
            [Display(Name = "City")]
            public string City { get; set; }

            [PersonalData]
            [Display(Name = "State")]
            public string State { get; set; }

            [PersonalData]
            [Display(Name = "Zip Code")]
            public string ZipCode { get; set; }

            [PersonalData]
            [NotMapped]
            [Display(Name = "Profile Image")]
            public IFormFile ProfileImage { get; set; }

            public string ProfileImageName { get; set; }

            [Display(Name = "Username")]
            public string Username { get; set; }


            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Timestamp]
            public byte[] RowVersion { get; set; }
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


            var list = from user in _context.Users.ToList()
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

        //Customer Profile Application
        [Authorize]
        [HttpGet]
        public IActionResult ProfileApplication()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ProfileApplication(Investment_Info invest, Account_Info account)
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
                    invest.Ques1 = invest.Ques1.ToUpper();
                    invest.Ques2 = invest.Ques2.ToUpper();

                    //Append User signature to User Record
                    user.Signature = invest.Signature;
                    user.FirstAccessed = false;
                    await _userManager.UpdateAsync(user);
                    _logger.LogInformation("User signature appended updated");

                    //Append UserID to User's investment Record
                    entity.FormType = "User Profile";

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
                    invest.FormType = "User Profile";
                    _context.UserInvestment.Add(invest);
                    _logger.LogInformation("User beneficiary and investment background added.");
                }

                return RedirectToAction("Index", "Account");
            }

            return View();
        }

        [HttpGet]
        public IActionResult RegisterProfile()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterProfile([Bind("Identity, ProfileImage")] InputModel input, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                //add Profile Image
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(input.ProfileImage.FileName);
                string extension = Path.GetExtension(input.ProfileImage.FileName);
                input.ProfileImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/image", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await input.ProfileImage.CopyToAsync(fileStream);
                }

                var user = new ApplicationUser
                {
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    Address1 = Input.Address1,
                    Address2 = Input.Address2,
                    PhoneNumber = Input.Mobile,
                    Residency = Input.Residency,
                    City = Input.City,
                    State = Input.State,
                    ZipCode = Input.ZipCode,
                    ProfileImage = fileName,
                    UserName = Input.Username,
                    Email = Input.Email,
                    FirstAccessed = true,
                    isAccount = false,
                    isProfile = true
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if(result.Succeeded)
                {
                    _logger.LogInformation("New user account successfully created.");

                    //Create User/Customer role *Default role*
                    var role = new ApplicationRole();
                    role.Name = "Customer-Profile";
                    await _roleManager.CreateAsync(role);

                    //Add new user to the default role
                    var addrole = await _userManager.AddToRoleAsync(user, "Customer-Profile");
                    _logger.LogInformation("User role added.");

                    //Email Verification Process
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    //Send Email Confirmation Link

                    var apiKey = "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";
                    var client = new SendGridClient(apiKey);
                    var from = new EmailAddress("planitureinvestments@gmail.com");
                    var to = new EmailAddress(Input.Email);
                    string subject = "Official Planiture Email Address Confirmation";
                    string htmlContent =
                        "<p>Hi there, " +
                        "<br />" +
                        "<br />" +
                        "This message is to confirm that the Planiture profile with the username " + Input.Username + " " +
                        "belongs to you. Verifying your email address helps you to secure your profile. If you forget your " +
                        "password, you will now be able to reset it by email." +
                        "<br />" +
                        "<br />" +
                        "<br />" +
                        "To confirm that this is your Planiture profile, click here:" +
                        "<br />" +
                        "<br>" +
                        "<a href='" + callbackUrl + "' style='background-color: red; border: 1px solid black; color: white;" +
                        " padding: 10px; width: 60%; font-weight: 500; border-radius: 10px;text-decoration: none;'>Confirm Email</a>" +
                        "<br>" +
                        "<br>" +
                        "If this is not your Planiture profile or you did not sign up for Planiture, please ignore this email." +
                        "<br />" +
                        "<br />" +
                        "Thanks, <br />The Planiture Family</p>";
                    var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
                    var response = await client.SendEmailAsync(msg);

                    _logger.LogInformation("Email Sent");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("EmailVerificationMessage");
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }
            return View();
        }

        [Authorize]
        public IActionResult Index()
        {
            var userid = _userManager.GetUserId(HttpContext.User);
            if (userid == null)
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

        public IActionResult DeletePhoto()
        {
            return View();
        }

        public IActionResult Edit()
        {
            return View();
        }

        public IActionResult DeactivateProfile()
        {
            return View();
        }

        public IActionResult Subscriptions()
        {
            return View();
        }

        public IActionResult Notifications()
        {
            return View();
        }
    }
}
