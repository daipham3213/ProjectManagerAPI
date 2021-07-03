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
        private readonly ProjectManagerDbContext _context;

        public GroupRepository(ProjectManagerDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<Group> FindGroupByName(string name)
        {
            return await _context.Groups.FirstOrDefaultAsync(u => u.Name.ToLower().Trim().Contains(name.ToLower().Trim()));
        }

        public async Task<Group> GetGroupByLeaderId(Guid id)
        {
            return await _context.Groups.FirstOrDefaultAsync(u => u.LeaderId == id & u.IsDeleted == false);
        }

        public async Task<ICollection<Group>> GetGroupListValidated(Guid leaderId)
        {
           // var user = await this._context.Users.FindAsync(leaderId);
           
            var mainGroup = await GetGroupByLeaderId(leaderId);
            if (mainGroup == null)
                throw new Exception("No group were found.");
            var type = await _context.GroupTypes.FirstOrDefaultAsync(u => u.ParentN == mainGroup.GroupType & u.IsDeleted == false);
            var childGroup = await _context.Groups.Where(u => u.GroupType == type & u.IsDeleted == false).ToListAsync();
            List<Group> result = new List<Group>();
            if (!result.Exists(u => u == mainGroup))
                result.Add(mainGroup);
            foreach (var child in childGroup)
            {
                if (!result.Exists(u => u == child))
                    result.Add(child);
            }
            return result;
        }

        public async Task<User> GetLeader(string groupname)
        {
            var group = FindGroupByName(groupname);
            return await _context.Users.FindAsync(group.Result.LeaderId);
        }

        public async Task<ICollection<User>> GetUserList(string groupname)
        {
            var group = FindGroupByName(groupname);
            return await _context.Users.Where(u => u.GroupRef == group.Result.Id & u.IsDeleted == false).ToListAsync();
        }

        public async void AddUserToGroup(Guid userId, Guid groupId)
        {
            var group = await Get(groupId);
            var user = await _context.Users.FindAsync(userId);
            var leader = await _context.Users.FindAsync(group.LeaderId);
            group.Users.Add(user);
            group.DateModified = DateTime.Now;
            user.Group = group;
            user.GroupRef = groupId;
            user.ParentN = leader;
            user.DateModified = DateTime.Now;
        }

        public async void RemoveUserFromGroup(Guid userId, Guid groupId)
        {
            var group = await Get(groupId);
            var user = await _context.Users.FindAsync(userId);

            group.Users.Remove(user);
            group.DateModified = DateTime.Now;
            user.Group = null;
            user.GroupRef = null;
            user.ParentN = null;
            user.DateModified = DateTime.Now;
        }
    }
}
