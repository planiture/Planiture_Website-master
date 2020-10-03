using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Planiture_Website.Models;

namespace Planiture_Website.Areas.Identity.Pages.Admin
{
    public class TransactionsModel : PageModel
    {
        public List<CusTransaction> GetTransaction { get; set; }
        //public List<Investment_Info> Invest { get; set; }

        private readonly ApplicationDbContext _context;

        public TransactionsModel(ApplicationDbContext context)
        {
            _context = context;
            this.GetTransaction = new List<CusTransaction>();
            //this.Invest = new List<Investment_Info>();
        }

        public void OnGet()
        {
            GetTransaction = _context.UserTransaction.ToList();
        }
    }
}
