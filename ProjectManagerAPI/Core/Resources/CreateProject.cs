using System;

namespace ProjectManagerAPI.Core.Resources
{
    public class CreateProject
    {
        public string Name { get; set; }
        public string Remark { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? StartDate { get; set; }
    }
}
