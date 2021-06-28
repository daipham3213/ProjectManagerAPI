using ProjectManagerAPI.Core.Models;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Services
{
    public interface ITokenParser
    {
        public Task<User> LoginResponse(string token);
    }
}
