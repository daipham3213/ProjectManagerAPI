using System;

namespace ProjectManagerAPI.Core.Resources
{
    public class UserResource
    {
        public Guid Id { set; get; }
        public string UserName { get; set; }
        public string Name { set; get; }
        public string Bio { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string AvatarUrl { get; set; }
        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupType { get; set; }
        public bool IsActived { get; set; }
        public DateTime DateCreated { get; set; }
        public float Contrib { get; set; }
    }
}
