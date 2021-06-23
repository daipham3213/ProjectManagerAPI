﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Models
{
    public class UserType : BaseModel
    {
        public UserType? ParentN { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
