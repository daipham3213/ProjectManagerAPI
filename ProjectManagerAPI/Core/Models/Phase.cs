using System;
using System.Collections.Generic;

namespace ProjectManagerAPI.Core.Models
{
    public class Phase : BaseModel
    {
        public virtual Report Report { get; set; }
        public Guid ReportId { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? StartDate { get; set; }
        public ICollection<Task> Tasks { get; set; }
        public Phase()
        {
            //base.DateCreated = DateTime.Now;
            //base.DateModified = DateTime.Now;
            IsActived = true;
            Tasks = new List<Task>();
        }
    }
}
