using ProjectManagerAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Resources
{
    public class CreatedGroup
    {
        public Guid ID { get; set; }
        public string  Name { get; set; }
        public string Remark { get; set; }
        public Guid? LeaderID { get; set; }
        public Guid GroupTypeFK { get; set; }
    }
}
