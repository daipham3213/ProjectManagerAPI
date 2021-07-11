﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectManagerAPI.Core.Models;

namespace ProjectManagerAPI.Core.Repositories
{
    public interface ITaskRepository : IRepository<Models.Task>
    {
        public Task<IEnumerable<Models.Task>> LoadValidated();
    }
}
