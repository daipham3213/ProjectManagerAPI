using ProjectManagerAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Repositories
{
    public interface IUserRepository 
    {
        public Task<User> GetUser(string userName);
        public Task<User> GetUserProfile(string username);
        public Task<User> SearchUserByUsername(string username);
        public Task<User> SearchUserById(Guid id);
        public Task<List<User>> SearchUsersByDisplayName(string displayName);
        public System.Threading.Tasks.Task LoadMainAvatar(string userName);
        System.Threading.Tasks.Task Load(Expression<Func<User, bool>> predicate);
    }
}
