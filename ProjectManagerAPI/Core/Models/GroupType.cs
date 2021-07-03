using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace ProjectManagerAPI.Core.Models
{
    public class GroupType : BaseModel
    {
        public GroupType? ParentN { get; set; }
        public Guid? ParentNid { get; set; }
        public ICollection<Group> Group { get; set; }

        public IdentityRole<Guid> IdentityRole { get; set; }
        public Guid IdentityRoleId { get; set; }
        public GroupType()
        {
            Group = new List<Group>();
            //base.DateCreated = DateTime.Now;
            //base.DateModified = DateTime.Now;
            IsActived = true;
        }
    }
}
