using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Resources
{
    public class CreatedTask
    {
        public string Name { get; set; }
        public string Remark { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? StartDate { get; set; }
        [DefaultValue(0)]
        public float Percent { get; set; }
        public Guid PhaseId { get; set; }
        public Guid UserId { get; set; }
    }
}
