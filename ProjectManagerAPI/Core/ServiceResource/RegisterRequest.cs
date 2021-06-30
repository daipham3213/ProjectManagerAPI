namespace ProjectManagerAPI.Core.ServiceResource
{
    public class RegisterRequest
    {
        public string Username { set; get; }
        public string Password { set; get; }
        public string Email { set; get; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
    }
}
