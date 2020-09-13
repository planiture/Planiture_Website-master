using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Threading.Tasks;
using System.Transactions;

namespace Planiture_Website.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        [Display(Name ="First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy HH:mm}")]
        [Display(Name = "Member Since")]
        public DateTime MemberSince { get; set; }

        public string Gender { get; set; }
        public string Occupation { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Residency { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Identity { get; set; }
        public string ProfileImage { get; set; }
        public string Signature { get; set; }

        public bool FirstAccessed { get; set; }
        public bool isProfile { get; set; }
        public bool isAccount { get; set; }

        public IList<Investment_Info> UserInvestments { get; set; }
        public IList<Account_Info> UserAccount { get; set; }
        public IList<ActivePlans> ActivePlans { get; set; }

        //Provide Concurrency
        [Timestamp]
        public byte[] RowVersion { get; set; }

    }
}
