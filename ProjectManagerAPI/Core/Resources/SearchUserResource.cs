using System;

namespace ProjectManagerAPI.Core.Resources
{
    public class SearchUserResource
    {
        public Guid Id { set; get; }
        public string Name { set; get; }
        public string Username { get; set; }
        public string AvatarUrl { get; set; }
    }
}
