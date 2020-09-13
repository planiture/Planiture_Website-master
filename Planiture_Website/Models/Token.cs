using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planiture_Website.Models
{
    public class Token
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Status { get; set; }
        public string UsedBy { get; set; }

    }
}
