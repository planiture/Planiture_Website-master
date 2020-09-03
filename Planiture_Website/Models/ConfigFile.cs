using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Planiture_Website.Models
{
    public class ConfigFile
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string AgentPass { get; set; }
        public string AdminPass { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
