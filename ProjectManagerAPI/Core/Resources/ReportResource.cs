using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using ProjectManagerAPI.Core.Models;

namespace ProjectManagerAPI.Core.Resources
{
    public class ReportResource
    {
        
        public string Name { get; set; }
        public string Url { get; set; }
        public string Remark { get; set; }
        public string GroupName { get; set; }
        public string GroupUrl { get; set; }
        public string ProjectName { get; set; }
        public string ProjectUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public float Progress { get; set; }
        public ICollection<Phase> Phases { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string UserCreated { get; set; }
    }
}
