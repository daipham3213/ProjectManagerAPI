using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using ProjectManagerAPI.Core;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.ServiceResource;
using ProjectManagerAPI.Core.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Persistence.Services
{
    public class TokenManager : ITokenManager
    {
        public TokenManager(IUnitOfWork unitOfWork, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
        }

        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private string GetCurrentAsync()
        {
            var authorizationHeader = _httpContextAccessor
                .HttpContext.Request.Headers["authorization"];

            return authorizationHeader == StringValues.Empty
                ? string.Empty
                : authorizationHeader.Single().Split(" ").Last();
        }

        private static string GetKey(string token)
            => $"tokens:{token}:deactivated";

        public async Task<User> GetUserByToken()
        {
            LoginResponse response = new LoginResponse();
            response = GetValues();
            var user = await _unitOfWork.Users.GetUser(response.UserName);

            return user;
        }
        protected LoginResponse GetValues()
        {
            var secret = _config["Tokens:Key"];
            var key = Encoding.UTF8.GetBytes(secret);
            var handler = new JwtSecurityTokenHandler();
            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
            var claims = handler.ValidateToken(GetCurrentAsync(), validations, out var tokenSecure);

            LoginResponse response = new LoginResponse();
            response.UserName = claims.Claims.First(c => c.Type == ClaimTypes.Name).Value;
            response.DisplayName = claims.Claims.First(c => c.Type == ClaimTypes.GivenName).Value;
            response.RoleName = claims.Claims.First(c => c.Type == ClaimTypes.Role).Value;
            return response;
        }
    }
}
