using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sprotify.API.Models
{
    public abstract class LinkedResourceBase
    {
        public List<Link> Links { get; set; }
        = new List<Link>();
    }
}
