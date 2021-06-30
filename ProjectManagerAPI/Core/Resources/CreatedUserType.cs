using System;

namespace ProjectManagerAPI.Core.Resources
{
    public class CreatedUserType
    {
        public string Name { get; set; }
        public string Remark { get; set; }
        public Guid? ParentNid { get; set; }
        public CreatedUserType? ParentN { get; set; }
    }
}
