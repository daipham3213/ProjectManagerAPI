using ProjectManagerAPI.Core.Models;
using System;
using System.Collections.Generic;

namespace ProjectManagerAPI.Core.Resources
{
    public class ProjectResource
    {
        public Guid Id { get; set; }
        public string url { get; set; }
        public string Name { get; set; }
        public string? Remark { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public Guid UserCreated { get; set; }
        public bool IsActived { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? StartDate { get; set; }
        public ICollection<Report> Reports { get; set; }
    }
}
