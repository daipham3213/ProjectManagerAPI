using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Models.Resources
{
    public class CreatedGroupType
    {
        public string Name { get; set; }
        public string? Remark { get; set; }
        public Guid UserCreated { get; set; }
        public GroupTypeResource? ParentN { get; set; }
    }
}
