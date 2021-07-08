using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Resources
{
    public class CreatedRequest
    {
        public string Name { get; set; }
        public string Remark { get; set; }
        public Guid To { get; set; }
        public string Value { get; set; }
    }
}
