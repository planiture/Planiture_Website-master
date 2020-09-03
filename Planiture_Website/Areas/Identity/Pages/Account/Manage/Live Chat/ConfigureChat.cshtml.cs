using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Planiture_Website.Models;

namespace Planiture_Website.Areas.Identity.Pages.Account.Manage.Live_Chat
{
    public class ConfigureChatModel : PageModel
    {
        private readonly ILogger<ConfigureChatModel> _logger;
        private ApplicationDbContext _context;

        public ConfigureChatModel(ILogger<ConfigureChatModel> logger,
                    ApplicationDbContext context,
                    RoleManager<ApplicationRole> roleManager)
        {
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public ConfigFileModel conFile { get; set; }
        public string ReturnUrl { get; set; }

        public class ConfigFileModel
        {
            public string AgentPass { get; set; }
            public string AdminPass { get; set; }
            [DataType(DataType.EmailAddress)]
            public string Email { get; set; }
            public string login_pass {get; set;}
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var tempstore = from a in _context.ConfigFiles select a;
                var provider = new SHA1CryptoServiceProvider();
                var encoding = new UnicodeEncoding();

                _context.ConfigFiles.RemoveRange(tempstore);
                _context.SaveChanges();

                

                var store = new ConfigFile
                {
                    AdminPass = Convert.ToBase64String(provider.ComputeHash(encoding.GetBytes(conFile.AdminPass))),
                    AgentPass = Convert.ToBase64String(provider.ComputeHash(encoding.GetBytes(conFile.AgentPass))),
                    Email = conFile.Email
                };

                _context.ConfigFiles.Add(store);
                await _context.SaveChangesAsync();

                return RedirectToPage("AgentLogin");
            }
            return Page();
        }

    }
}
