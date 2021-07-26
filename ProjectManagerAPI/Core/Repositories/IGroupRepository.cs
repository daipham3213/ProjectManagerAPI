using ProjectManagerAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagerAPI.Core.Repositories
{
    public interface IGroupRepository : IRepository<Group>
    {
        public Task<Group> FindGroupByName(string name);
        public Task<ICollection<User>> GetUserList(string groupname);
        public Task<User> GetLeader(string groupName);
        public Task<Group> GetGroupByLeaderId(Guid id);
        public Task<ICollection<Group>> GetGroupListValidated(Guid id);
        public Task AddUserToGroup(Guid userId, Guid groupId);
        public Task RemoveUserFromGroup(Guid userId, Guid groupId);
    }
}
