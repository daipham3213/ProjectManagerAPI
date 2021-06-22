using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Models
{
    public class UserType : BaseModel
    {
        public virtual HashSet<UserType> parent_n { get; set; }
    }
}
