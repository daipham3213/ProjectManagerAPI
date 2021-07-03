using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Resources
{
    public class PhaseViewResource
    {
        public string Name { get; set; }
        public string Remark { get; set; }
        public string Url { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? StartDate { get; set; }
        public ICollection<Task> Tasks { get; set; }
        public Guid ReportID { get; set; }
    }
}
