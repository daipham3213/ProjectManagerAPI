using ProjectManagerAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagerAPI.Core.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        public Task<User> GetUser(string userName);
        public Task<User> GetUserProfile(string username);
        public Task<User> SearchUserByUsername(string username);
        public Task<User> SearchUserById(Guid id);
        public Task<List<User>> SearchUsersByDisplayName(string displayName);
        public Task LoadMainAvatar(string userName);
        Task Load(Expression<Func<User, bool>> predicate);
        Task LeaveGroup(Guid id);
    }
}
