using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
        private readonly IWebHostEnvironment _hostEnvironment;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            RoleManager<ApplicationRole> roleManager,
            IEmailSender emailSender,
            IWebHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
            _emailSender = emailSender;
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
            [Display(Name = "Upload Proof of Identity")]
            public IFormFile Identity { get; set; }

            public string IdentityImageName { get; set; }

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

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync([Bind ("Identity, ProfileImage")] InputModel input, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                //add Proof of identity image
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(input.Identity.FileName);
                string extension = Path.GetExtension(input.Identity.FileName);
                input.IdentityImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/image", fileName);
                using(var fileStream = new FileStream(path,FileMode.Create))
                {
                    await input.Identity.CopyToAsync(fileStream);
                }

                //add Profile Image
                string wwwRootPath1 = _hostEnvironment.WebRootPath;
                string fileName1 = Path.GetFileNameWithoutExtension(input.ProfileImage.FileName);
                string extension1 = Path.GetExtension(input.ProfileImage.FileName);
                input.ProfileImageName = fileName1 = fileName1 + DateTime.Now.ToString("yymmssfff") + extension1;
                string path1 = Path.Combine(wwwRootPath1 + "/image", fileName1);
                using (var fileStream1 = new FileStream(path1, FileMode.Create))
                {
                    await input.ProfileImage.CopyToAsync(fileStream1);
                }

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
                    Identity = fileName,
                    ProfileImage = fileName1,
                    UserName = Input.Username,
                    Email = Input.Email,
                    FirstAccessed = true,
                    isProfile = false,
                    isAccount = true
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("New user account successfully created.");

                    //Create User/Customer role *Default role*
                    var role = new ApplicationRole();
                   role.Name = "Admin";
                   await _roleManager.CreateAsync(role);

                    //Add new user to the default role
                    var addrole = await _userManager.AddToRoleAsync(user, "Admin");
                    _logger.LogInformation("User role added.");

                    //Send User Proof of Identity information to company email
                    var apiKey = "";
                    var client = new SendGridClient(apiKey);
                    var from = new EmailAddress("planitureinvestments@gmail.com");
                    var to = new EmailAddress("planitureinvestments@gmail.com");
                    string subject = "Official Planiture Customer - "+Input.FirstName+"" +Input.LastName+"- Proof of Identity";
                    string htmlContent = "See attachment for customer's ID information";
                    var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
                    //get image location
                    var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "image", user.Identity);
                    //convert image to bytes
                    byte[] bytes = Encoding.ASCII.GetBytes(imagePath);
                    //attach image bytes to email and send
                    var trying = new Attachment()
                    {
                        Disposition = "attachment/image",
                        ContentId = null,
                        Filename = fileName,
                        Type ="*",
                        Content = Convert.ToBase64String(bytes)
                    };
                    msg.AddAttachment(trying);
                    var response = client.SendEmailAsync(msg);

                    //Email Verification Process
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);


                    //Send Email Confirmation Link

                    var apiKey1 = "";
                    var client1 = new SendGridClient(apiKey1);
                    var from1 = new EmailAddress("planitureinvestments@gmail.com");
                    var to1 = new EmailAddress(Input.Email);
                    string subject1 = "Official Planiture Email Address Confirmation";
                    string htmlContent1 = 
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
                        "Thanks, <br />The Planiture Family</p>";
                    var msg1 = MailHelper.CreateSingleEmail(from1, to1, subject1, null, htmlContent1);
                    var response1 = await client1.SendEmailAsync(msg1);

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
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
