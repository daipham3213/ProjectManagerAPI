using Microsoft.EntityFrameworkCore;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Persistence.ReposMocks
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly ProjectManagerDBContext _context;

        public UserRepository(ProjectManagerDBContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<User> GetUser(string userName)
        {
            return await this._context.Users.SingleOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<User> GetUserProfile(string username)
        {
            var user = await this._context.Users
                .Include(u => u.Tasks)
                .Include(u => u.Avatars)
                .SingleOrDefaultAsync(u => u.UserName == username);
            //await this._context.UserRoles.SingleOrDefaultAsync(u => u.UserId == user.Id);
            return user;
        }

        public async System.Threading.Tasks.Task LoadMainAvatar(string userName)
        {
            await this._context.Avatars.Where(a => a.IsMain && a.User.UserName == userName).LoadAsync();
        }

        public async Task<User> SearchUserById(string id)
        {
            var user = await this._context.Users.SingleOrDefaultAsync(u => u.Id.ToString() == id);
            return user;
        }

        public async Task<User> SearchUserByUsername(string username)
        {
            var user = await this._context.Users.SingleOrDefaultAsync(u => u.UserName == username);

            return user;
        }

        public async Task<List<User>> SearchUsersByDisplayName(string displayName)
        {
            var users = await this._context.Users.Where(u => u.Name.Contains(displayName)).ToListAsync();

            return users;
        }
    }
}
