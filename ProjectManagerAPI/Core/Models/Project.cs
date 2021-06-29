using System;
using System.Collections.Generic;

namespace ProjectManagerAPI.Core.Models
{
    public class Project : BaseModel
    {
        public ICollection<Report> Reports { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? StartDate { get; set; }
        public Project()
        {
            Reports = new List<Report>();
            base.DateCreated = DateTime.Now;
            base.DateModified = DateTime.Now;
            base.IsActived = true;
        }
    }
}
