using System;
using System.ComponentModel;

namespace ProjectManagerAPI.Core.Resources
{
    public class CreatedReport
    {
        public string Name { get; set; }
        public string Remark { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        [DefaultValue(0)]
        public float Progress { get; set; }
        public Guid ProjectId { get; set; }
        public Guid GroupId { get; set; }
    }
}
