using ProjectManagerAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Repositories
{
    public interface IGroupRepository : IRepository<Group>
    {
        public Task<Group> FindGroupByName(string name);
        public Task<ICollection<User>> GetUserList(string groupname);
        public Task<User> GetLeader(string groupname);
        public Task<Group> GetGroupByLeaderID(Guid id);
        public Task<ICollection<Group>> GetGroupListValidated(Guid id);
        public void AddUserToGroup(Guid userID, Guid groupID);
        public void RemoveUserFromGroup(Guid userID, Guid groupID);
    }
}
