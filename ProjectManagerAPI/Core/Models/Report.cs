using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ProjectManagerAPI.Core.Models
{
    public class Report : BaseModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        [DefaultValue(0)]
        public float Progress { get; set; }
        public Project Project { get; set; }
        public Guid ProjectID { get; set; }
        public Group Group { get; set; }
        public Guid GroupID { get; set; }

        public ICollection<Phase> Phases { get; set; }

        public Report()
        {
            base.DateCreated = DateTime.Now;
            base.DateModified = DateTime.Now;
            base.IsActived = true;
            Phases = new List<Phase>();
        }
    }
}
