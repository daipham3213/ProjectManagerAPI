using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Models
{
    public class Group : BaseModel
    {
        public GroupType GroupType{ get; set; }
        public Guid GroupTypeFK { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<Report> Reports { get; set; }
    }
}
