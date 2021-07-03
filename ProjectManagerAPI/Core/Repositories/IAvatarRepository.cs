using ProjectManagerAPI.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Repositories
{
    public interface IAvatarRepository : IRepository<Avatar>
    {
        public Task<List<Avatar>> GetAvatars(string userName);
    }
}
