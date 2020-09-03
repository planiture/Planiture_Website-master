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
        [Display(Name = "Transaction Type")]
        public string TransactionType { get; set; }

        [PersonalData]
       // [Required]
        [Display(Name = "Account Number")]
        public int Trans_AccountNumber { get; set; }

        [PersonalData]
        //[Required]
        [Display(Name = "Other Account")]
        public string Trans_OtherAccount { get; set; }

        [PersonalData]
        //[Required]
        [Display(Name = "Transaction Status")]
        public string Trans_TransactionStatus { get; set; }

        //User Foreign Key
        public int UserID { get; set; }
        public ApplicationUser User { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        //Customer_Info Foreign Key
        //public int CustomerID { get; set; }
        //[ForeignKey("CustomerID")]
        //public InputModel Customers { get; set; }

        //Account_Info Foreign Key
       // public int AccountNumber { get; set; }
       // [ForeignKey("AccountNumber")]
       // public Account_Info Accounts { get; set; }

        //Employee_Info Foreign Key
      //  public int EmployeeID { get; set; }
      //  [ForeignKey("EmployeeID")]
      //  public Employee_Info Employees { get; set; }


    }
}
