using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Models.Resources
{
    public class GroupTypeViewResource
    {
        public string url { get; set; }
        public string Name { get; set; }
        public string? Remark { get; set; }
        public ICollection<Group> Group { get; set; }
        public GroupTypeViewResource? ParentN { get; set; }
    }
}
