using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Edm;
using Planiture_Website.Models;

namespace Planiture_Website.Areas.Identity.Pages.Account.Manage.Live_Chat
{
    public class AgentLoginModel : PageModel
    {
        private readonly ILogger<AgentLoginModel> _logger;
        private ApplicationDbContext _context;

        public AgentLoginModel(ILogger<AgentLoginModel> logger,
            ApplicationDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string username { get; set; }
            [DataType(DataType.Password)]
            public string password { get; set; }
        }

        public IActionResult OnGet()
        {
            return Page();
        }
    }
}
