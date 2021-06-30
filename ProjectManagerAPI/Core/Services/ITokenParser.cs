using System.Threading.Tasks;
using ProjectManagerAPI.Core.Models;

namespace ProjectManagerAPI.Core.Services
{
    public interface ITokenParser
    {
        public Task<User> GetUserByToken(string token);
    }
}
