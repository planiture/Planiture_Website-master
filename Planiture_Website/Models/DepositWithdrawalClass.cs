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

        //Deposit Information
        [Display(Name = "Cardholder's Name")]
        public string CardName { get; set; }
        [Display(Name = "Card Number")]
        public int CardNumber {get; set;}
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/yyyy}")]
        [Display(Name = "Exp. Date")]
        public DateTime ExpDate { get; set; }
        [Display(Name = "Deposit Amount")]
        public float DepositAmount { get; set; }
        [Display(Name = "CVV")]
        public int CVV { get; set; }

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
