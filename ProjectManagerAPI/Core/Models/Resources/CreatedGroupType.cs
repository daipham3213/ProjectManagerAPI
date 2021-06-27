using System;

namespace ProjectManagerAPI.Core.Models.Resources
{
    public class CreatedGroupType
    {
        public string Name { get; set; }
        #nullable enable
        public string? Remark { get; set; }
        #nullable enable
        public Guid? ParentNID { get; set; }
        #nullable enable
        public GroupTypeResource? ParentN { get; set; }
    }
}
