using System;

namespace ProjectManagerAPI.Core.Resources
{
    public class SearchUserResource
    {
        public Guid Id { set; get; }
        public string Name { set; get; }
        public string AvatarUrl { get; set; }
    }
}
