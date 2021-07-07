using System;

namespace ProjectManagerAPI.Core.Resources
{
    public class CreatedGroupType
    {
        public string Name { get; set; }
#nullable enable
        public string? Remark { get; set; }
#nullable enable
        public Guid? ParentNid { get; set; }
#nullable enable
        public GroupTypeResource? ParentN { get; set; }
        public Guid IdentityRoleId { get; set; }
    }
}
