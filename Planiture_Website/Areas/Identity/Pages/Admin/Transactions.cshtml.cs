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

        [BindProperty(SupportsGet = true)]
        public string searchTerm { get; set; }

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

        public async Task OnGetSearchAsync()
        {
            var list = from s in _context.UserTransaction
                       select s;

            if (!String.IsNullOrEmpty(searchTerm))
            {
                list = list.Where(m => m.TransactionType.Contains(searchTerm) || m.UserID.ToString().Contains(searchTerm) || m.TransactionID.ToString().Contains(searchTerm) || 
                    m.Trans_AccountNumber.ToString().Contains(searchTerm) || m.TransactionAmount.ToString().Contains(searchTerm));
            }

            GetTransaction = list.ToList();
        }
    }
}
