using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Planiture_Website.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Planiture_Website.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                //Send Email Confirmation Link

                var apiKey = "";
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress("akeamsmith41@gmail.com");
                var to = new EmailAddress(Input.Email);
                string subject = "Planiture Password Reset";
                string htmlContent =
                    "<p>Hey "+Input.Email+", " +
                    "<br />" +
                    "<br />" +
                    "To reset your password for Planiture, please click the following link:" +
                    "<br />" +
                    "<br>" +
                    "<a href='" + callbackUrl + "' style='background-color: red; border: 1px solid black; color: white;" +
                    " padding: 10px; width: 60%; font-weight: 500; border-radius: 10px;text-decoration: none;'>Reset Password</a>" +
                    "<br>" +
                    "<br>" +
                    "If you don't want to reset your password, you can ignore this message -  someone probably typed in your username or email address " +
                    "by mistake." +
                    "<br />" +
                    "<br />" +
                    "Thanks!, <br />The Planiture Family</p>";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
                var response = await client.SendEmailAsync(msg);

                //await _emailSender.SendEmailAsync(
                //  Input.Email,
                //  "Reset Password",
                //  $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
