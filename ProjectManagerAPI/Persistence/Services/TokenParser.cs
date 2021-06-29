using Microsoft.Extensions.Configuration;
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
    public class TokenParser : ITokenParser
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        public TokenParser(IUnitOfWork unitOfWork, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _config = config;
        }

        public async Task<User> GetUserByToken(string tokenString)
        {
            LoginResponse response = new LoginResponse();
            response = GetValues(tokenString);
            var user = await this._unitOfWork.Users.GetUser(response.UserName);
            
            return user;
        }
        protected LoginResponse GetValues(string token)
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
            var claims = handler.ValidateToken(token, validations, out var tokenSecure);
            
            LoginResponse response = new LoginResponse();
            response.UserName = claims.Claims.First(c => c.Type == ClaimTypes.Name).Value;
            response.DisplayName = claims.Claims.First(c => c.Type == ClaimTypes.GivenName).Value;
            response.RoleName = claims.Claims.First(c => c.Type == ClaimTypes.Role).Value;
            return response;
        }
    }
}
