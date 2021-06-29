using Microsoft.EntityFrameworkCore;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Persistence.ReposMocks
{
    public class GroupRepository : Repository<Group>, IGroupRepository 
    {
        private readonly ProjectManagerDBContext _context;

        public GroupRepository(ProjectManagerDBContext context)
            :base(context)
        {
            _context = context;
        }

        public async Task<Group> FindGroupByName(string name)
        {
            return await this._context.Groups.FirstOrDefaultAsync(u => u.Name == name);
        }

        public async Task<User> GetLeader(string groupname)
        {
            var group = FindGroupByName(groupname);
            return await this._context.Users.FindAsync(group.Result.LeaderID);
        }

        public async Task<ICollection<User>> GetUserList(string groupname)
        {
            var group = FindGroupByName(groupname);
            return await this._context.Users.Where(u => u.GroupRef == group.Result.ID).ToListAsync();
        }
    }
}
