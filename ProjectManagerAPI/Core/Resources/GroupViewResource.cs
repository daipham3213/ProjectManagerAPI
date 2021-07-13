using System;
using ProjectManagerAPI.Core.Models;

namespace ProjectManagerAPI.Core.Resources
{
    public class GroupViewResource
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
        public string Url { get; set; }
        public Guid GroupTypeFk { get; set; }
        public GroupTypeViewResource GroupType { get; set; }
        public int Users { get; set; }
        public User Leader { get; set; }
        public Guid LeaderId { get; set; }
    }
}
