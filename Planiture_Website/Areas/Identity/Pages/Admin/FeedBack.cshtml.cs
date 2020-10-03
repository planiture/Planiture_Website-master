using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Planiture_Website.Models;

namespace Planiture_Website.Areas.Identity.Pages.Admin
{
    public class FeedBackModel : PageModel
    {
        public List<Feedback> GetFeedback { get; set; }

        private readonly ApplicationDbContext _context;

        public FeedBackModel(ApplicationDbContext context)
        {
            _context = context;
            this.GetFeedback = new List<Feedback>();
            //this.Invest = new List<Investment_Info>();
        }

        public void OnGet()
        {
            GetFeedback = _context.UserFeedback.ToList();
        }
    }
}
