using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Resources
{
    public class TaskResources
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public float Percent { get; set; }
        public string PhaseName { get; set; }
        public string UserName { get; set; }
        public string PhaseId { get; set; }  
        public string UserId { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string UserCreated { get; set; }
        public Guid ParentNId { get; set; }
        public ICollection<TaskViewResource> ChildTasks { get; set; }
    }
}
