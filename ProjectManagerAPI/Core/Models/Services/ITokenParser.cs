using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Models.Services
{
    public interface ITokenParser
    {
        public Task<User> LoginResponse(string token);
    }
}
