using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Planiture_Website.Models;

namespace Planiture_Website.Areas.Identity.Pages.Account.Manage
{
    public class TransactionsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<TransactionsModel> _logger;
        private readonly ApplicationDbContext _context;

        public TransactionsModel(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<TransactionsModel> logger,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        public IList<Account_Info> Accounts { get; set; }

        public async Task OnGet()
        {
            Accounts = await _context.UserAccount
                .Include(c => c.User)
                .AsNoTracking()
                .ToListAsync();
        }

    }
}
