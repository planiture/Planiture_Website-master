using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using static Planiture_Website.Areas.Identity.Pages.Account.RegisterModel;

namespace Planiture_Website.Models
{
    public class Investment_Info 
    {
        //Invesment Questions

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Display(Name = "Form Type")]
        public string FormType { get; set; }

        [Display(Name = "Question 1")]
        public string Ques1 { get; set; }


        [Display(Name = "Question 2")]
        public string Ques2 { get; set; }

        [Display(Name = "Question 3")]
        public string Ques3 { get; set; }

        [Display(Name = "Question 4")]
        public string Ques4 { get; set; }

        [Display(Name = "Question 5")]
        public string Ques5 { get; set; }

        [Display(Name = "Question 6")]
        public string Ques6 { get; set; }

        //Customer Information
        [PersonalData]
        [Display(Name = "Signature")]
        public string Signature { get; set; }


        //Beneficiary Information
        [PersonalData]
        [Display(Name = "First Name")]

        public string Bene_FirstName { get; set; }

        [PersonalData]
        [Display(Name = "Last Name")]

        public string Bene_LastName { get; set; }

        [PersonalData]
        [Display(Name = "Contact")]
        public string Bene_Contact { get; set; }

        [PersonalData]
        [Display(Name = "Relationship")]

        public string Bene_Relationship { get; set; }

        [PersonalData]
        [EmailAddress]
        public string Bene_Email { get; set; }

        [PersonalData]
        [Display(Name = "Address")]
        public string BenAddress { get; set; }

        //User Foreign Key
        [Display(Name = "Address")]
        public int UserID { get; set; }
        public ApplicationUser User { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
