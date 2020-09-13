using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Planiture_Website.Models
{
    public class ActivePlans
    {
        public int ID { get; set; }
        public string PlanName { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateExpired { get; set; }
        
        //User Foreign Key
        public int UserID { get; set; }
        public ApplicationUser User { get; set; }
        
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
