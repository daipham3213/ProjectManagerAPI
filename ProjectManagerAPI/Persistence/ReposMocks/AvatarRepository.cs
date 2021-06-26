using Microsoft.EntityFrameworkCore;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Persistence.ReposMocks
{
    public class AvatarRepository : Repository<Avatar>, IAvatarRepository
    {
        private readonly ProjectManagerDBContext _context;

        public AvatarRepository(ProjectManagerDBContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<Avatar>> GetAvatars(string userName)
        {
            var avatars = await this._context.Avatars.Where(a => a.User.UserName == userName).ToListAsync();

            return avatars;
        }
    }
}
