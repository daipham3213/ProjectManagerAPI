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
    }
}
