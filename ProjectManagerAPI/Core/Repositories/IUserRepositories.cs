using ProjectManagerAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Repositories
{
    interface IUserRepositories
    {
        public Task<User> GetUser(string userName);
        public Task<User> GetUserProfile(string username);
        System.Threading.Tasks.Task Load(Expression<Func<User, bool>> predicate);
    }
}
