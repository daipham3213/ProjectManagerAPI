using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectManagerAPI.Core.Models;

namespace ProjectManagerAPI.Core.Resources
{
    public class PhaseResource
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
        public string Url { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? StartDate { get; set; }
        public bool IsActived { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<Models.Task> Tasks { get; set; }
        public Guid ReportID { get; set; }
        public Guid UserCreated { get; set; }
    }
}
