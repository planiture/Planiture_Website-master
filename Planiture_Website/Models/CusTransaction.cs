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
    public class CusTransaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionID { get; set; }

        [PersonalData]
       // [Required]
        [Display(Name = "Transaction Amount")]
        public float TransactionAmount { get; set; }

        [PersonalData]
        // [Required]
        [Display(Name = "Most Recent")]
        public float MostRecent { get; set; }

        [PersonalData]
       // [Required]
        [Display(Name = "Transaction Type")]
        public string TransactionType { get; set; }

        [PersonalData]
       // [Required]
        [Display(Name = "Account Number")]
        public int Trans_AccountNumber { get; set; }

        [PersonalData]
        [Display(Name = "Commission")]
        public float Commission { get; set; }

        [PersonalData]
        [Display(Name = "Service Fee")]
        public float ServiceFee { get; set; }

        [PersonalData]
        //[Required]
        [Display(Name = "Transaction Status")]
        public string TransactionStatus { get; set; }

        [Display(Name = "Transaction Date")]
        public DateTime Date { get; set; }

        //User Foreign Key
        public int UserID { get; set; }
        public ApplicationUser User { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

    }
}
