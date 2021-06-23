using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Models
{
    public class Task : BaseModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        [DefaultValue(0)]   
        public float Percent { get; set; }
        public Phase Phase { get; set; }
        public Guid PhaseID { get; set; }
        public User User { get; set; }
        public Guid UserID { get; set; }
    }
}
