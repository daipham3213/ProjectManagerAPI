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
            return await this._context.Groups.FirstOrDefaultAsync(u => u.Name.ToLower().Trim().Contains(name.ToLower().Trim()));
        }

        public async Task<Group> GetGroupByLeaderID(Guid id)
        {
            return await this._context.Groups.FirstOrDefaultAsync(u => u.LeaderID == id & u.IsDeleted == false);
        }

        public async Task<ICollection<Group>> GetGroupListValidated(Guid leader_id)
        {
            var main_group = GetGroupByLeaderID(leader_id);
            if (main_group == null)
                return null;
            var type = await this._context.GroupTypes.FirstOrDefaultAsync(u => u.ParentN == main_group.Result.GroupType & u.IsDeleted == false);
            var child_group = await this._context.Groups.Where(u => u.GroupType == type & u.IsDeleted == false).ToListAsync();
            List<Group> result = new List<Group>();
            result.Add(main_group.Result);
            result.AddRange(child_group);
            return result;
        }

        public async Task<User> GetLeader(string groupname)
        {
            var group = FindGroupByName(groupname);
            return await this._context.Users.FindAsync(group.Result.LeaderID);
        }

        public async Task<ICollection<User>> GetUserList(string groupname)
        {
            var group = FindGroupByName(groupname);
            return await this._context.Users.Where(u => u.GroupRef == group.Result.ID & u.IsDeleted == false).ToListAsync();
        }

        public async void AddUserToGroup(Guid userID, Guid groupID)
        {
            var group = await Get(groupID);
            var user = await this._context.Users.FindAsync(userID);
            var leader = await this._context.Users.FindAsync(group.LeaderID);
            group.Users.Add(user);
            group.DateModified = DateTime.Now;
            user.Group = group;
            user.GroupRef = groupID;
            user.ParentN = leader;
            user.DateModified = DateTime.Now;
        }

        public async void RemoveUserFromGroup(Guid userID, Guid groupID)
        {
            var group = await Get(groupID);
            var user = await this._context.Users.FindAsync(userID);
            group.Users.Remove(user);
            group.DateModified = DateTime.Now;
            user.Group = null;
            user.GroupRef = null;
            user.ParentN = null;
            user.DateModified = DateTime.Now;
        }
    }
}
