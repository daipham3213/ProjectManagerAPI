using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Repositories;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagerAPI.Persistence.ReposMocks
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly ProjectManagerDbContext _context;

        public UserRepository(ProjectManagerDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<User> GetUser(string userName)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<User> GetUserProfile(string username)
        {
            var user = await _context.Users
                .Include(u => u.Tasks)
                .Include(u => u.Avatars)
                .SingleOrDefaultAsync(u => u.UserName == username);
            //await this._context.UserRoles.SingleOrDefaultAsync(u => u.UserId == user.Id);
            return user;
        }

        public async Task LoadMainAvatar(string userName)
        {
            await _context.Avatars.Where(a => a.IsMain && a.User.UserName == userName).LoadAsync();
        }

        public async Task LeaveGroup(Guid id)
        {
            var user = await this._context.Users.FindAsync(id);
            if (user == null)
                throw new Exception("User is invalid.");
            if (user.Group == null & user.GroupRef == null)
                throw new Exception("User don't have group.");
            if (user.Id == user.Group.LeaderId)
                throw new Exception("Leader can't leave group. Promo others to leader before leave.");
            user.Group = null;
            user.GroupRef = null;
            user.ParentN = null;
            user.DateModified = DateTime.UtcNow;
            await this._context.SaveChangesAsync();
        }

        public async Task<User> SearchUserById(Guid id)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<User> SearchUserByUsername(string username)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);

            return user;
        }

        public async Task<List<User>> SearchUsersByDisplayName(string displayName)
        {
            var users = await _context.Users.Where(u => u.Name.Contains(displayName)).ToListAsync();

            return users;
        }


    }
}
