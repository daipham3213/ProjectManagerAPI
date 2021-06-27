using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Models.Resources
{
    public class CreatedUserType
    {
        public string Name { get; set; }
        public string Remark { get; set; }
        public Guid? ParentNID { get; set; }
        public CreatedUserType? ParentN { get; set; }
    }
}
