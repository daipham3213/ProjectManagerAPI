using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Resources
{
    public class CreatePhase
    {
        public string Name { get; set; }
        public string Remark { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? StartDate { get; set; }
    }
}
