using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Models
{
    public class Request : BaseModel
    {
        public Request()
        {
            IsAccepted = false;
            IsDenied = false;
        }
        public bool IsAccepted { get; set; }
        public bool IsDenied { get; set; }
        public bool IsGroup{ get; set; }
        public Guid To { get; set; }
        public string Value { get; set; }
    }
}
