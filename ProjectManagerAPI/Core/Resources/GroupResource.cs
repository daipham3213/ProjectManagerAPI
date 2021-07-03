using System;
using System.Collections.Generic;

namespace ProjectManagerAPI.Core.Resources
{
    public class GroupResource
    {
        public string Name { get; set; }
        public string Remark { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string Url { get; set; }
        public Guid GroupTypeFk { get; set; }
        public ICollection<SearchUserResource> Users { get; set; }
        public Guid LeaderId { get; set; }
    }
}
