using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using ProjectManagerAPI.Core.ServiceResource;

namespace ProjectManagerAPI.Core.Models
{
    public class User : IdentityUser<Guid>
    {
        public User()
        {
            IsActived = false;
            IsSuperuser = false;
            Tasks = new List<Task>();
            Avatars = new List<Avatar>();
            RefreshTokens = new List<RefreshToken>();

            //DateCreated = DateTime.Now;
            //DateModified = DateTime.Now;
        }
        public override Guid Id
        {
            get => base.Id;
            set => base.Id = value;
        }
        public string Name { get; set; }
        public string? Bio { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public bool IsActived { get; set; }
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
        public User? ParentN { get; set; }
        public Guid? ParentNId { get; set; }
        [DefaultValue(false)]
        public bool IsSuperuser { get; set; }
        public Guid? GroupRef { get; set; }
        public Group? Group { get; set; }
        public ICollection<Task> Tasks { get; set; }
        public ICollection<Avatar> Avatars { get; set; }
        public virtual ICollection<IdentityUserClaim<Guid>> Claims { get; set; }
        public virtual ICollection<IdentityUserLogin<Guid>> Logins { get; set; }
        public virtual ICollection<IdentityUserToken<Guid>> Tokens { get; set; }
        public virtual ICollection<IdentityUserRole<Guid>> UserRoles { get; set; }
        [JsonIgnore]
        public List<RefreshToken> RefreshTokens { get; set; }

    }
}
