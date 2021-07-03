using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.ServiceResource;
using System.Collections.Generic;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagerAPI.Core.Services
{
    public interface IUserService
    {
        public Task<LoginResponse> Authenticate(LoginRequest request);
        public Task<IList<string>> Register(RegisterRequest request);
        public Task<User> GetUser(string userName);
        public Task<List<User>> SearchUser(string key);
        public Task SendChangeEmailRequest(string username, string newEmail, string callbackurl);
        public Task<bool> ConfirmChangeEmail(string username, string newEmail, string token);
        public Task<bool> ChangePassword(string userName, string currentPassword, string newPassword);
        public Task<bool> CheckPassword(string userName, string password);
        Task SendActivationRequest(string username, string callbackurl);
        Task<bool> ConfirmActivation(string username, string token);
        Task<bool> PromotionBy(string lead_username, string promo_username);
        Task Promotion(string username);
        Task DePromotion(string username);
    }
}
