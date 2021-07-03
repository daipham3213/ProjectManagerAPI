using System.Collections.Generic;

namespace ProjectManagerAPI.Core.Resources
{
    public class AddMemberResource
    {
        public string GroupName { get; set; }
        public IList<string> Usernames { get; set; }
    }
}
