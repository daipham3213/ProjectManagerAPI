namespace ProjectManagerAPI.Core.Models.ServiceResource
{
    public class LoginRequest
    {
        public string Username { set; get; }
        public string Password { set; get; }
        public bool RememberMe { set; get; }
    }
}
