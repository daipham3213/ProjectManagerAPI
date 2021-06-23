using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Models
{
    public class Phase : BaseModel
    {
        public Report Report { get; set; }
        public Guid ReportID { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? StartDate { get; set; }
        public ICollection<Task> Tasks { get; set; }
    }
}
