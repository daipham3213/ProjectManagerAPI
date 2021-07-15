using System;
using System.Collections.Generic;
using ProjectManagerAPI.Core.Models;

namespace ProjectManagerAPI.Core.Resources
{
    public class GroupResource
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string Url { get; set; }
        public Guid GroupTypeFk { get; set; }
        public GroupType GroupType  {get; set; }
        public ICollection<SearchUserResource> Users { get; set; }
        public UserResource Leader { get; set; }
        public Guid LeaderId { get; set; }
    }
}
