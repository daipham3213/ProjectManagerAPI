using ProjectManagerAPI.Core.Models;
using System;
using System.Collections.Generic;

namespace ProjectManagerAPI.Core.Resources
{
    public class ReportResource
    {
        public Guid Id { get; set; }
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
