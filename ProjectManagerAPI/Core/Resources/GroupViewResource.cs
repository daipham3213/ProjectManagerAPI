using System;

namespace ProjectManagerAPI.Core.Resources
{
    public class GroupViewResource
    {
        public string Name { get; set; }
        public string Remark { get; set; }
        public string Url { get; set; }
        public Guid GroupTypeFk { get; set; }
        public int Users { get; set; }
        public Guid LeaderId { get; set; }
    }
}
