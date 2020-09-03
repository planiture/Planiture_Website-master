using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Planiture_Website.Models;

namespace Planiture_Website.Areas.Identity.Pages.Account.Manage
{
    public class CreateRoleModel : PageModel
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<CreateRoleModel> _logger;

        public CreateRoleModel(RoleManager<ApplicationRole> roleManager,
            ILogger<CreateRoleModel> logger)
        {
            _roleManager = roleManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public string ReturnUrl { get; set; }


        public class InputModel
        {
            [Required]
            public string RoleName { get; set; }
        }

        public IActionResult OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            return Page();
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if(ModelState.IsValid)
            {
                ApplicationRole role = new ApplicationRole
                {
                    Name = Input.RoleName
                };

                IdentityResult result = await _roleManager.CreateAsync(role);

                if(result.Succeeded)
                {
                    _logger.LogInformation("New role added");
                    //return RedirectToAction("/Account/Manage/Dashboard");
                    //ViewBag.message = "Success";
                }
                foreach(IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            

            return Page();
        }
    }
}
