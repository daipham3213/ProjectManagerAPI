using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Models
{
    public class GroupType : BaseModel
    {
        public GroupType? ParentN { get; set; }
        public ICollection<Group> Group { get; set; }
    }
}
