using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Resources
{
    public class RequestResource
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
        public string Value { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsDenied { get; set; }
        public bool IsGroup { get; set; }
        public Guid To { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
