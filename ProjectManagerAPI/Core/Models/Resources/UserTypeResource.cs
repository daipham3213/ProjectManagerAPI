using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Models.Resources
{
    public class UserTypeResource
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string? Remark { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public Guid UserCreated { get; set; }  
        public UserTypeResource? ParentN { get; set; }
    }
}
