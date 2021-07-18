using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using ProjectManagerAPI.Core.Models;

namespace ProjectManagerAPI.Core.ServiceResource
{
    public class LoginResponse
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
        public string DisplayName { get; set; }
        public string AvatarUrl { get; set; }
        public string RoleName { get; set; }
        public bool IsActivated { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpTime { get; set; }

        public LoginResponse(User user, string token, string refreshToken, string avatarUrl , string roleName, DateTime time)
        {
            UserName = user.UserName;
            Token = token;
            DisplayName = user.Name;
            AvatarUrl = avatarUrl;
            RoleName = roleName;
            IsActivated = user.IsActived;
            RefreshToken = refreshToken;
            ExpTime = time;
        }

        public LoginResponse()
        {
            
        }
    }
}
