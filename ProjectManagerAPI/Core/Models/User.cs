using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Models
{
    public class User : BaseModel
    {
        public User()
        {
            this.IsActived = false;
            this.IsSuperuser = false;
        }

        public User(string username, string email, string phone, UserType userType, User parentN, bool isSuperuser, Guid? groupRef, Group group)
        {
            Username = username;
            Email = email;
            Phone = phone;
            UserType = userType;
            ParentN = parentN;
            IsSuperuser = isSuperuser;
            GroupRef = groupRef;
            Group = group;
        }

        public string Username { get; set; }
        public string? Email { get; set; }
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
