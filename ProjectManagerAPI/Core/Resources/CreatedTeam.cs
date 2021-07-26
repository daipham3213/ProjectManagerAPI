using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectManagerAPI.Core.Models;

namespace ProjectManagerAPI.Core.Resources
{
    public class CreatedTeam
    {
        public string Name { get; set; }
        public string Remark { get; set; }
        public Guid? LeaderId { get; set; }
        public Guid ParentNId { get; set; }
    }
}
