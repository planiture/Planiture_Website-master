using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Planiture_Website.Areas.Identity.Pages.Account.Manage.Live_Chat
{
    public class ChatModel : PageModel
    {
        public IActionResult OnGet()
        {
            string key = "AgentCookie";
            var cookieValue = Request.Cookies[key];

            //check if agent as already logged in
            /*if(cookieValue == "AlreadyLoggedIn")
            {
                return Page();
            }
            else
            {
                return RedirectToPage("AgentLogin");
            }*/
            return Page();

        }

        public IActionResult OnPost()
        {
            return Page();
        }
    }
}
