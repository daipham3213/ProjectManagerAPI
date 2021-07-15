using System;
using System.Collections.Generic;

namespace ProjectManagerAPI.Core.Resources
{
    public class AddMemberResource
    {
        public string GroupName { get; set; }
        public IList<Guid> Usernames { get; set; }
    }
}
