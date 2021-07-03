using ProjectManagerAPI.Core.Models;
using System.Collections.Generic;

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
