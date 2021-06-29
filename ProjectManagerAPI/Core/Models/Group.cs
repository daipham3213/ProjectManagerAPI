using System;
using System.Collections.Generic;

namespace ProjectManagerAPI.Core.Models
{
    public class Group : BaseModel
    {
        public GroupType GroupType { get; set; }
        public Guid GroupTypeFK { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<Report> Reports { get; set; }
        public Guid LeaderID { get; set; }

        public Group()
        {
            Users = new List<User>();
            Reports = new List<Report>();
            base.DateCreated = DateTime.Now;
            base.DateModified = DateTime.Now;
            base.IsActived = true;
        }
    }
}
