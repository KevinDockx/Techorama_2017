using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sprotify.API.Models
{
    public class LinkedCollectionResourceWrapper<T> : LinkedResourceBase
        where T : LinkedResourceBase
    {
        public IEnumerable<T> Value { get; set; }

        public LinkedCollectionResourceWrapper(IEnumerable<T> value)
        {
            Value = value;
        }
    }
}
