using ProjectManagerAPI.Core.Models;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Services
{
    public interface ITokenManager
    {
        public Task<User> GetUserByToken();
    }
}
