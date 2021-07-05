using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using ProjectManagerAPI.Core.Models;

namespace ProjectManagerAPI.Core.ServiceResource
{
    public class LoginResponse
    {
        public string UserName { get; set; }
        public string Token { get; set; }
        public string DisplayName { get; set; }
        public string AvatarUrl { get; set; }
        public string RoleName { get; set; }
        public bool IsActivated { get; set; }
        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }

        public LoginResponse(User user, string token, string refreshToken, string avatarUrl , string roleName)
        {
            UserName = user.UserName;
            Token = token;
            DisplayName = user.Name;
            AvatarUrl = avatarUrl;
            RoleName = roleName;
            IsActivated = user.IsActived;
            RefreshToken = refreshToken;
        }

        public LoginResponse()
        {
            
        }
    }
}
