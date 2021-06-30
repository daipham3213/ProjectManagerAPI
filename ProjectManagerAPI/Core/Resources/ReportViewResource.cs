using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Resources
{
    public class ReportViewResource
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Remark { get; set; }
        public string GroupName { get; set; }
        public string ProjectName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public float Progress { get; set; }
    }
}
