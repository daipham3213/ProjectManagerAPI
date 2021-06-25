using ProjectManagerAPI.Core.Models.ServiceResource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Models.Services
{
    public interface IUserService
    {
        public Task<LoginResponse> Authenticate(LoginRequest request);
        public Task<IList<string>> Register(RegisterRequest request);
        public Task<User> GetUser(string userName);
        public Task<List<User>> SearchUser(string key);
        public Task<bool> IsFollowed(string followerUsername, string followingUsername);
        public Task SendChangeEmailRequest(string username, string newEmail, string callbackurl);
        public Task<bool> ConfirmChangeEmail(string username, string newEmail, string token);
        public Task<bool> ChangePassword(string userName, string currentPassword, string newPassword);
        public Task<bool> CheckPassword(string userName, string password);
        public Task<IList<User>> GetFollowingSuggestion(string userName, int numFollowings);
    }
}
