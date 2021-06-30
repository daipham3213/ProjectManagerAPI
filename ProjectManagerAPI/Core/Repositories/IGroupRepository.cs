using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectManagerAPI.Core.Models;

namespace ProjectManagerAPI.Core.Repositories
{
    public interface IGroupRepository : IRepository<Group>
    {
        public Task<Group> FindGroupByName(string name);
        public Task<ICollection<User>> GetUserList(string groupname);
        public Task<User> GetLeader(string groupname);
        public Task<Group> GetGroupByLeaderId(Guid id);
        public Task<ICollection<Group>> GetGroupListValidated(Guid id);
        public void AddUserToGroup(Guid userId, Guid groupId);
        public void RemoveUserFromGroup(Guid userId, Guid groupId);
    }
}
