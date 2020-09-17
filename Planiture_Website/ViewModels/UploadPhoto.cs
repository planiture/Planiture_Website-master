using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Planiture_Website.ViewModels
{
    public class UploadPhoto
    {
        [PersonalData]
        [NotMapped]
        [Display(Name = "Profile Image")]
        public IFormFile ProfileImage { get; set; }

        public string ProfileImageName { get; set; }

    }
}
