using ProjectManagerAPI.Core.Models;
using System;
using System.Collections.Generic;

namespace ProjectManagerAPI.Core.Resources
{
    public class ProjectViewResource
    {
        public Guid Id { get; set; }
        public string url { get; set; }
        public string Name { get; set; }
        public string? Remark { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? StartDate { get; set; }
   
    }
}
