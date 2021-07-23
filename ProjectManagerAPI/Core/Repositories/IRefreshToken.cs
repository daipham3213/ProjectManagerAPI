using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.ServiceResource;

namespace ProjectManagerAPI.Core.Repositories
{
    public interface IRefreshToken : IRepository<RefreshToken>
    {
        public Task<User> GetUserByRefreshToken(string token);
    }
}
