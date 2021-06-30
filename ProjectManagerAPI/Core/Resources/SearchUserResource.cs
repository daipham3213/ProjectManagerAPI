﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Resources
{
    public class SearchUserResource
    {
        public Guid Id { set; get; }
        public string Name { set; get; }
        public string AvatarUrl { get; set; }
        public Guid UserTypeID { get; set; }
    }
}
