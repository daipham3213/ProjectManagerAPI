using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Models
{
    public class User : IdentityUser<Guid>
    {
        public User()
        {
            this.IsActived = false;
            this.IsSuperuser = false;
            Tasks = new List<Task>();
        }
      
        public string Name { get; set; }
        public string? Bio { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsActived { get; set; }
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
        public string? Phone { get; set; }
        public UserType UserType { get; set; }
        public User? ParentN { get; set; }
        [DefaultValue(false)]
        public bool IsSuperuser { get; set; }
        public Guid? GroupRef { get; set; }
        public Group Group { get; set; }

        public ICollection<Task> Tasks { get; set; }
    }
}
