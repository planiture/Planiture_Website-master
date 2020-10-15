using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Planiture_Website.Models
{
    public class GoogleEmail
    {
        public int Id { get; set; }
        [Required,MaxLength(512)]
        public string Subject { get; set; }
        [MaxLength]
        public string Body { get; set; }
        [Required, MaxLength(256)]
        public string From { get; set; }
        [Required, MaxLength(128)]
        public string Date { get; set; }
    }
}
