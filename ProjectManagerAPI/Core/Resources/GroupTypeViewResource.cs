using System.Collections.Generic;
using ProjectManagerAPI.Core.Models;

namespace ProjectManagerAPI.Core.Resources
{
    public class GroupTypeViewResource
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public string? Remark { get; set; }
        public ICollection<Group> Group { get; set; }
        public GroupTypeViewResource? ParentN { get; set; }
    }
}
