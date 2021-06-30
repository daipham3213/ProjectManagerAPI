using ProjectManagerAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Resources
{
    public class GroupViewResource
    {
        public string Name { get; set; }
        public string Remark { get; set; }
        public string Url { get; set; }
        public Guid GroupTypeFK { get; set; }
        public int Users { get; set; }
        public Guid LeaderID { get; set; }
    }
}
