using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Planiture_Website.Models
{
    public class DepositWithdrawalClass
    {
        [Key]
        public int ID { get; set; }
        //Withdrawal Information
        [Display(Name = "Name on Account")]
        public string Name { get; set; }
        [Display(Name = "Account No.")]
        public int AccountNumber { get; set; }
        [Display(Name = "Account Type")]
        public string AccountType { get; set; }
        [Display(Name = "Withdrawal Amount")]
        public float WithdrawalAmount { get; set; }
        [Display(Name = "Notes")]
        public string Notes { get; set; }
    }
}
