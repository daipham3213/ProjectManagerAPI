using System.Collections.Generic;

namespace ProjectManagerAPI.Core.Models
{
    public class GroupType : BaseModel
    {
        public GroupType? ParentN { get; set; }
        public ICollection<Group> Group { get; set; }
    }
}
