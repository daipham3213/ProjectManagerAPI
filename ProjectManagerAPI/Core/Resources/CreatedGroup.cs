using System;

namespace ProjectManagerAPI.Core.Resources
{
    public class CreatedGroup
    {
        public string Name { get; set; }
        public string Remark { get; set; }
        public Guid? LeaderId { get; set; }
        public Guid GroupTypeFk { get; set; }
    }
}
