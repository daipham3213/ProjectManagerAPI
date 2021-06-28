using ProjectManagerAPI.Core.Models;
using System;
using System.Collections.Generic;

namespace ProjectManagerAPI.Core.Resources
{
    public class GroupTypeResource
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string? Remark { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public Guid UserCreated { get; set; }
        public bool IsActived { get; set; }
        public bool IsDeleted { get; set; }
        public GroupTypeResource? ParentN { get; set; }
        public ICollection<Group> Group { get; set; }
    }
}
