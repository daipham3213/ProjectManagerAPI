using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectManagerAPI.Core.Models;

namespace ProjectManagerAPI.Core.Repositories
{
    public interface IAvatarRepository : IRepository<Avatar>
    {
        public Task<List<Avatar>> GetAvatars(string userName);
    }
}
