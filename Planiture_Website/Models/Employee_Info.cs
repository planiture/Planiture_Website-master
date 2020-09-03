using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Planiture_Website.Models
{
    public class Employee_Info
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeID { get; set; }

        [PersonalData]
        [Required]
        [Display(Name = "Emp_FirstName")]
        public string Emp_FirstName { get; set; }

        [PersonalData]
        [Required]
        [Display(Name = "Emp_LastName")]
        public string Emp_LastName { get; set; }

        [PersonalData]
        [Required]
        [Display(Name = "Emp_UserName")]
        public string Emp_UserName { get; set; }

        [PersonalData]
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Emp_Password")]
        public string Emp_Password { get; set; }

        [PersonalData]
        [Required]
        [Display(Name = "Emp_Old_Password")]
        public string Emp_Old_Password { get; set; }

        [PersonalData]
        [Required]
        [Display(Name = "ReportsTo")]
        public string ReportsTo { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

    }
}
