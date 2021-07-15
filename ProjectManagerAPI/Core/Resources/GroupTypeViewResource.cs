using System;
using ProjectManagerAPI.Core.Models;
using System.Collections.Generic;
using System.Security.AccessControl;

namespace ProjectManagerAPI.Core.Resources
{
    public class GroupTypeViewResource
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string? Remark { get; set; }
        public GroupTypeViewResource? ParentN { get; set; }
    }
}
