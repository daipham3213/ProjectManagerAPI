using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Models
{
    public class GroupType : BaseModel
    {
        
        public virtual HashSet<GroupType> parent_n { get; set; }
    }
}
