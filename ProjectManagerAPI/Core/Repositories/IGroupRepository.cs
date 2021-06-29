using ProjectManagerAPI.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Repositories
{
    public interface IGroupRepository : IRepository<Group>
    {
        public Task<Group> FindGroupByName(string name);
        public Task<ICollection<User>> GetUserList(string groupname);
        public Task<User> GetLeader(string groupname);
    }
}
