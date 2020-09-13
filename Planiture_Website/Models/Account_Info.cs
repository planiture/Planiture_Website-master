using Microsoft.AspNetCore.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using static Planiture_Website.Areas.Identity.Pages.Account.RegisterModel;

namespace Planiture_Website.Models
{
    public class Account_Info : List<Account_Info>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AccountNumber { get; set; }

        [PersonalData]
        [Display(Name = "Account Name")]
        public string AccountName { get; set; }

        [PersonalData]
        [Display(Name = "Account Type")]
        public string AccountType { get; set; }

        [PersonalData]
        [Display(Name = "Account Version")]
        public string AccountVersion { get; set; }

        [PersonalData]
        [Display(Name = "Available Balance")]
        public float AvailableBalance { get; set; }

        [PersonalData]
        [Display(Name = "Actual Balance")]
        public float ActualBalance { get; set; }

        [PersonalData]
        [Display(Name = "Withdrawal Amount")]
        public float WithdrawalAmount { get; set; }

        [PersonalData]
        [Display(Name = "Deposit Amount")]
        public float DepositAmount { get; set; }

        [PersonalData]
        [Display(Name = "Account Status")]
        public string AccountStatus { get; set; }

        //User Foreign Key
        public int UserID { get; set; }
        public ApplicationUser User { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

    }

}
