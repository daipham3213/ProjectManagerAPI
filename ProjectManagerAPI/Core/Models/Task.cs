using System;
using System.ComponentModel;

namespace ProjectManagerAPI.Core.Models
{
    public class Task : BaseModel
    {
        public Task()
        {
            //base.DateCreated = DateTime.Now;
            //base.DateModified = DateTime.Now;
            base.IsActived = true;
        }

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
