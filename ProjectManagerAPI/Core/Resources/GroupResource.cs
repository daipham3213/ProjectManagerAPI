using ProjectManagerAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Resources
{
    public class GroupResource
    {
        public string Name { get; set; }
        public string Remark { get; set; }
        public DateTime? DateCreated { get; set; } 
        public DateTime? DateModified { get; set; }
        public string Url { get; set; }
        public Guid GroupTypeFK { get; set; }
        public ICollection<SearchUserResource> Users { get; set; }
        public Guid LeaderID { get; set; }
    }
}
