using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Resources
{
    public class AddMemberResource
    {
        public string GroupName { get; set; }
        public IList<string> Usernames { get; set; }
    }
}
