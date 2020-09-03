using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planiture_Website.Hubs
{
    public class Agent
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsOnline { get; set; }

        public Agent()
        {
        }
    }
}
