using System;

namespace ProjectManagerAPI.Core.Resources
{
    public class ReportViewResource
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
        public Guid GroupId { get; set; }
        public Guid ProjectId { get; set; }
        public string GroupName { get; set; }
        public string ProjectName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public float Progress { get; set; }
    }
}
