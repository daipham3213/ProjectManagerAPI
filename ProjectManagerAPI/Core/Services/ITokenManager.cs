using ProjectManagerAPI.Core.Models;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagerAPI.Core.Services
{
    public interface ITokenManager
    {
        public Task<User> GetUserByToken();
    }
}
