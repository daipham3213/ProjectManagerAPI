using System;

namespace ProjectManagerAPI.Core.Resources
{
    public class CreatedDepartment
    {
        public string Name { get; set; }
        public string Remark { get; set; }
        public Guid? LeaderId { get; set; }
    }
}
