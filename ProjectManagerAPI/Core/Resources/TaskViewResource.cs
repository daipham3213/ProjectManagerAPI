using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Resources
{
    public class TaskViewResource
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Remark { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public float Percent { get; set; }
        public string PhaseName { get; set; }
        public string UserName { get; set; }
    }
}
