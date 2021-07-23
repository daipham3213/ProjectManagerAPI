using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Repositories;
using ProjectManagerAPI.Core.ServiceResource;

namespace ProjectManagerAPI.Persistence.ReposMocks
{
    public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshToken
    {
        private readonly ProjectManagerDbContext _context;
        public RefreshTokenRepository(ProjectManagerDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User> GetUserByRefreshToken(string token)
        {
            var rToken = await _context.RefreshTokens.FirstOrDefaultAsync(u => u.Token == token);
            if (rToken == null)
                return null;
            return await this._context.Users.FindAsync(rToken.UserId);
        }
    }
}
