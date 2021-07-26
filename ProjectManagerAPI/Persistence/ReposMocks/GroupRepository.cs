using Microsoft.EntityFrameworkCore;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectManagerAPI.Core.Services;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagerAPI.Persistence.ReposMocks
{
    public class GroupRepository : Repository<Group>, IGroupRepository
    {
        private readonly ProjectManagerDbContext _context;

        public GroupRepository(ProjectManagerDbContext context )
            : base(context)
        {
            _context = context;
        }

        public async Task<Group> FindGroupByName(string name)
        {
            return await _context.Groups
                .FirstOrDefaultAsync(u => u.Name.ToLower().Trim().Contains(name.ToLower().Trim()) 
                                          & u.IsActived);
        }

        public async Task<Group> GetGroupByLeaderId(Guid id)
        {
            return await _context.Groups
                .FirstOrDefaultAsync(u => u.LeaderId == id
                                          & !u.IsDeleted
                                          & u.IsActived);
        }

        public async Task<ICollection<Group>> GetGroupListValidated(Guid leaderId)
        {
           // var user = await this._context.Users.FindAsync(leaderId);
           
            var mainGroup = await GetGroupByLeaderId(leaderId);
            if (mainGroup == null)
                return null;
            var childGroup = await _context.Groups
                .Where(u => u.ParentNId == mainGroup.Id 
                            & u.IsDeleted == false 
                            & u.IsActived)
                .ToListAsync();
            List<Group> result = new List<Group>();
            result.Add(mainGroup);
            result.AddRange(childGroup);
            foreach (var child in childGroup)
            {
                var chiList = await _context.Groups
                    .Where(u => u.ParentNId == child.Id
                                & u.IsDeleted == false
                                & u.IsActived)
                    .ToListAsync();
                result.AddRange(chiList);
            }

            return result;
        }

        public async Task<User> GetLeader(string groupName)
        {
            var group = await FindGroupByName(groupName);
            return await _context.Users.FindAsync(group.LeaderId);
        }

        public async Task<ICollection<User>> GetUserList(string groupname)
        {
            var group = FindGroupByName(groupname);
            return await _context.Users.Where(u => u.GroupRef == group.Result.Id & u.IsDeleted == false).ToListAsync();
        }

        public async Task AddUserToGroup(Guid userId, Guid groupId)
        {
            var group = await Get(groupId);
            if (!group.IsActived)
                throw new Exception("This group has not been activated.");
            var user = await _context.Users.FindAsync(userId);
            var leader = await _context.Users.FindAsync(group.LeaderId);
            group.Users.Add(user);
            group.DateModified = DateTime.Now;
            user.Group = group;
            user.GroupRef = groupId;
            user.ParentN = leader;
            user.DateModified = DateTime.Now;
            await this._context.SaveChangesAsync();
        }

        public async Task RemoveUserFromGroup(Guid userId, Guid groupId)
        {
            var group = await Get(groupId);
            var user = await _context.Users.FindAsync(userId);
            if(user != null)
            {
                group.Users.Remove(user);
                group.DateModified = DateTime.Now;
                user.Group = null;
                user.GroupRef = null;
                user.ParentN = null;
                user.DateModified = DateTime.Now;
                await this._context.SaveChangesAsync();
            }
        }

        
    }
}
