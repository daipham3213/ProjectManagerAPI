using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Models.ServiceResource;
using ProjectManagerAPI.Core.Models.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Persistence.Services
{
    public class UserSerivce : IUserService
    {
        public Task<LoginResponse> Authenticate(LoginRequest request)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> ChangePassword(string userName, string currentPassword, string newPassword)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> CheckPassword(string userName, string password)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> ConfirmChangeEmail(string username, string newEmail, string token)
        {
            throw new System.NotImplementedException();
        }

        public Task<IList<User>> GetFollowingSuggestion(string userName, int numFollowings)
        {
            throw new System.NotImplementedException();
        }

        public Task<User> GetUser(string userName)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> IsFollowed(string followerUsername, string followingUsername)
        {
            throw new System.NotImplementedException();
        }

        public Task<IList<string>> Register(RegisterRequest request)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<User>> SearchUser(string key)
        {
            throw new System.NotImplementedException();
        }

        public Core.Models.Task SendChangeEmailRequest(string username, string newEmail, string callbackurl)
        {
            throw new System.NotImplementedException();
        }
    }
}