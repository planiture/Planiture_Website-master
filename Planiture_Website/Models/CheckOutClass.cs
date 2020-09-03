using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Planiture_Website.Models
{
    public class CheckOutClass
    {
        [Key]
        public int ID { get; set; }
        public string Card_Holder_Name { get; set; }
        public int Card_Number { get; set; }
        public DateTime Exp { get; set; }
        public int CVV { get; set; }
    }
}
