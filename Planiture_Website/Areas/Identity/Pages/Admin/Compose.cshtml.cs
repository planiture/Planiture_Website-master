using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Planiture_Website.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Planiture_Website.Areas.Identity.Pages.Admin
{
    public class ComposeModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly IEmailService _emailSender;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ComposeModel(
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            public string To { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
            public string From { get; set; }
            public List<IFormFile> files { get; set; }
        }

        private async Task<IActionResult> LoadAsync()
        {

            Input = new InputModel
            {
                //From = _userManager.GetUserAsync(User).Result.Email,
                From = "officialplanitureinvestments@gmail.com",
                files = new List<IFormFile>()

            };
            return Page();
        }

        public async Task OnGet()
        {
            await LoadAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var apiKey = "";
            var client = new SendGridClient(apiKey);
            //var from = new EmailAddress("akeamsmith41@gmail.com");
            var from = new EmailAddress(Input.From);
            var to = new EmailAddress(Input.To);
            string subject = Input.Subject;
            string htmlContent = Input.Body;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
            var response = client.SendEmailAsync(msg);



            await LoadAsync();
            return Page();
        }
    }
}
