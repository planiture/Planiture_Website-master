using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using Planiture_Website.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using SendGrid.Helpers.Mail.Model;

namespace Planiture_Website.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            RoleManager<ApplicationRole> roleManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
            _emailSender = emailSender;
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
            [Display(Name = "DOB")]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
            public DateTime DOB { get; set; }

            [PersonalData]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy HH:mm}")]
            public DateTime MemberSince { get; set; }


            [PersonalData]
            [Display(Name = "Gender")]
            public string Gender { get; set; }

            [PersonalData]
            //[Required]
            [Display(Name = "Occupation")]
            public string Occupation { get; set; }

            /*[PersonalData]
            [Display(Name = "Area Code")]
            public string AreaCode { get; set; }*/

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
            [Display(Name = "Upload Proof of Identity")]
            public string Identity { get; set; }
            
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

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    DOB = Input.DOB,
                    Address1 = Input.Address1,
                    Address2 = Input.Address2,
                    PhoneNumber = Input.Mobile,
                    Gender = Input.Gender,
                    Occupation = Input.Occupation,
                    Residency = Input.Residency,
                    City = Input.City,
                    State = Input.State,
                    ZipCode = Input.ZipCode,
                    Identity = Input.Identity,
                    UserName = Input.Username,
                    Email = Input.Email,
                    FirstAccessed = true
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("New user account successfully created.");

                    //Create User/Customer role *Default role*
                    var role = new ApplicationRole();
                   role.Name = "Customer";
                   await _roleManager.CreateAsync(role);

                    //Add new user to the default role
                    var addrole = await _userManager.AddToRoleAsync(user, "Customer");
                    _logger.LogInformation("User role added.");

                    //Email Verification Process
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    /*_logger.Log(LogLevel.Warning, callbackUrl);
                    ViewBag.ErrorTitle = "Registration Successful";*/


                    //Send Email Confirmation Link

                    var apiKey = "key here";
                    var client = new SendGridClient(apiKey);
                    var from = new EmailAddress("akeamsmith41@gmail.com");
                    var to = new EmailAddress(Input.Email);
                    string subject = "Planiture Email Address Confirmation";
                    string htmlContent = 
                        "<p>Hi there, " +
                        "<br />" +
                        "<br />" +
                        "This message is to confirm that the Planiture account with the username "+Input.Username+" " +
                        "belongs to you. Verifying your email address helps you to secure your account. If you forget your " +
                        "password, you will now be able to reset it by email." +
                        "<br />" +
                        "<br />" +
                        "<br />" +
                        "To confirm that this is your Planiture account, click here:" +
                        "<br />" +
                        "<br>" +
                        "<a href='"+callbackUrl+"' style='background-color: red; border: 1px solid black; color: white;" +
                        " padding: 10px; width: 60%; font-weight: 500; border-radius: 10px;text-decoration: none;'>Confirm Email</a>" +
                        "<br>" +
                        "<br>" +
                        "If this is not your Planiture account or you did not sign up for Planiture, please ignore this email." +
                        "<br />" +
                        "<br />" +
                        "Thanks, <br />Team Planiture</p>";
                    var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
                    var response = await client.SendEmailAsync(msg);

                    _logger.LogInformation("Email Sent");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("Login");
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
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
