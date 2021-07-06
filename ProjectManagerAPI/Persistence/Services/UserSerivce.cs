using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProjectManagerAPI.Core;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.ServiceResource;
using ProjectManagerAPI.Core.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagerAPI.Persistence.Services
{
    public class UserSerivce : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMailService _mailService;
        private readonly IConfiguration _config;

        public UserSerivce(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IUnitOfWork unitOfWork,
            IMailService mailService,
            IConfiguration config,
            RoleManager<IdentityRole<Guid>> roleManager
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _mailService = mailService;
            _config = config;
            _roleManager = roleManager;
        }

        public async Task<LoginResponse> Authenticate(LoginRequest request,string ipAddress)
        {
            // check user exitst
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
                throw new Exception("Username don't exist.");

            //check password
            var result = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, true);
            if (!result.Succeeded)
                throw new Exception("Wrong password.");

            var isActivated = user.IsActived;
            if (!isActivated)
                return new LoginResponse(user, null, null, null, null);


            //get roles
            var roles = await _userManager.GetRolesAsync(user);

            var name = user.Name;
            if (name == null)
                name = "undefined";
            //create claims
            var claims = await GetClaims(user);

            //create tokens
            var finalToken = generateJwtToken(user, claims);
            var refreshToken = generateRefreshToken(ipAddress);
            await this._userManager.RemoveAuthenticationTokenAsync(user, "Bearer", user.UserName);
            user.Tokens ??= new List<IdentityUserToken<Guid>>();
            
            await _unitOfWork.Complete();

            //Save token to DB
            user.Tokens.Add(new IdentityUserToken<Guid>()
            {
                LoginProvider = "Bearer",
                Name = user.UserName,
                UserId = user.Id,
                Value = finalToken
            });
            user.RefreshTokens.Add(refreshToken);
            await _unitOfWork.Complete();
            //Load Avatar
            await _unitOfWork.Avatars.Load(a => a.UserId == user.Id && a.IsMain);

            var avatar = user.Avatars.FirstOrDefault(a => a.IsMain);
            string path = null;

            if (avatar != null)
                path = avatar.Path;

            var loginResponse = new LoginResponse(user, finalToken, refreshToken.Token, path, roles.FirstOrDefault());
            return loginResponse;
        }

        public async Task<bool> ChangePassword(string userName, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByNameAsync(userName);

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            return result.Succeeded;
        }

        public async Task<bool> CheckPassword(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);

            var result = await _userManager.CheckPasswordAsync(user, password);

            return result;
        }

        public async Task<bool> ConfirmChangeEmail(string username, string newEmail, string token)
        {
            var user = await _userManager.FindByNameAsync(username);

            var result = await _userManager.ChangeEmailAsync(user, newEmail, token);

            return result.Succeeded;
        }

        public async Task<bool> ConfirmActivation(string username, string token)
        {
            var user = await _userManager.FindByNameAsync(username);

            var result = await _userManager.ConfirmEmailAsync(user, token);

            return result.Succeeded;
        }

        public async Task Promotion(string username)
        {
            var user = await this._userManager.FindByNameAsync(username);
            if (user == null)
                throw new Exception("Invalid information.");
            if (user.Group == null)
                throw new Exception("User's group is invalid.");
            var role = await  this._roleManager.FindByIdAsync(user.Group.GroupType.IdentityRoleId.ToString());
            await this._userManager.AddToRoleAsync(user, role.Name);
        }

        public async Task DePromotion(string username)
        {
            var leader = await _userManager.FindByNameAsync(username);
            var role = await this._roleManager.FindByIdAsync(leader.Group.GroupType.IdentityRoleId.ToString());
            await this._userManager.RemoveFromRoleAsync(leader, role.Name);
        }

        public async Task<bool> PromotionBy(string lead_username, string promo_username)
        {
            var leader = await this._userManager.FindByNameAsync(lead_username);
            var user = await this._userManager.FindByNameAsync(promo_username);
            var group = await this._unitOfWork.Groups.GetGroupByLeaderId(leader.Id);

            if (leader == null | user == null | group == null)
                throw new Exception("Invalid information.");
            if (group.LeaderId != leader.Id)
                throw new Exception("Permission not allowed.");
            if (group.Id != user.GroupRef)
                throw new Exception(promo_username + "  is not a member of " + group.Name);

            await this._unitOfWork.Users.Load(u => u.ParentNId == leader.ParentNId | u.ParentNId == user.ParentNId);

            var temp = leader.ParentN;
            leader.ParentN = user;
            user.ParentN = temp;
            group.LeaderId = user.Id;
            leader.DateModified = DateTime.UtcNow;
            user.DateModified = DateTime.UtcNow;
            group.DateModified = DateTime.UtcNow;
            await this._unitOfWork.GroupTypes.Load(u => u.Id == group.GroupTypeFk);
            var role = await this._roleManager.FindByIdAsync(group.GroupType.IdentityRoleId.ToString());
            await this._userManager.RemoveFromRoleAsync(leader, role.Name);
            await this._userManager.AddToRoleAsync(user, role.Name);
            await this._unitOfWork.Complete();
            return true;
        }


        public async Task<User> GetUser(string userName)
        {
            return await _unitOfWork.Users.GetUser(userName);
        }


        public async Task<IList<string>> Register(RegisterRequest request)
        {
            var listError = new List<string>();

            var user = await _userManager.FindByNameAsync(request.Username);

            //check email
            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                listError.Add("Email already in use");
            }

            //check username
            if (user != null)
            {
                listError.Add("Username already exists");
            }
            user = new User
            {
                UserName = request.Username,
                Name = request.Name,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            if (listError.Count != 0)
                return listError;

            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                var u = await _unitOfWork.Users.GetUser(user.UserName);
                await _userManager.AddToRoleAsync(u, RoleNames.RoleUser);
                return null;
            }

            throw new Exception("Error while creating user account.");
        }

        public async Task<List<User>> SearchUser(string key)
        {
            var users = new List<User>();

            Guid guidOutput;
            User user;
            bool isValid = Guid.TryParse(key, out guidOutput);
            if (isValid)
            {
                user = await _unitOfWork.Users.SearchUserById(guidOutput);
            }

            user = await _unitOfWork.Users.SearchUserByUsername(key);
            if (user != null)
            {
                users.Add(user);
            }

            if (users.Count == 1)
            {
                if (await IsOnlyRoleUser(users[0]))
                {
                    await _unitOfWork.Users.LoadMainAvatar(users[0].UserName);

                    return users;
                }

            }

            users = await _unitOfWork.Users.SearchUsersByDisplayName(key);

            var usersRoleUser = new List<User>();
            foreach (var user1 in users)
            {
                if (await IsOnlyRoleUser(user1))
                {
                    usersRoleUser.Add(user1);
                }
            }

            foreach (var userRoleUser in usersRoleUser)
            {
                await _unitOfWork.Users.LoadMainAvatar(userRoleUser.UserName);
            }


            return usersRoleUser;
        }
        private async Task<bool> IsOnlyRoleUser(User user)
        {
            //Only role User or not has any roles

            var roles = await _userManager.GetRolesAsync(user);

            bool flag = true;
            foreach (var role in roles)
            {
                if (role != RoleNames.RoleUser)
                    flag = false;
            }

            return flag;
        }
        public async Task SendChangeEmailRequest(string username, string newEmail, string callbackurl)
        {
            var user = await _userManager.FindByNameAsync(username);

            var token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);

            // append userId and confirmation code as parameters to the url
            callbackurl += String.Format("?username={0}&newEmail={1}&token={2}", user.UserName, newEmail, HttpUtility.UrlEncode(token));

            var htmlContent = String.Format(
                    @"To change your email. Please confirm the email by clicking this link: 
                    <br><a href='{0}'>Confirm new email</a>",
                    callbackurl);

            // send email to the user with the confirmation link
            MailRequest mailRequest = new MailRequest
            {
                ToEmail = user.Email,
                Subject = "Change email",
                Body = htmlContent
            };

            try
            {
                await _mailService.SendEmailAsync(mailRequest);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public async Task SendActivationRequest(string username, string callbackurl)
        {
            var user = await _userManager.FindByNameAsync(username);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // append userId and confirmation code as parameters to the url
            callbackurl += String.Format("?username={0}&token={1}", user.UserName, HttpUtility.UrlEncode(token));

            var htmlContent = String.Format(
                    @"Activate your account. Please confirm the email by clicking this link: 
                    <br><a href='{0}'>Confirm new email</a>",
                    callbackurl);

            // send email to the user with the confirmation link
            MailRequest mailRequest = new MailRequest
            {
                ToEmail = user.Email,
                Subject = "Account Activation",
                Body = htmlContent
            };

            try
            {
                await _mailService.SendEmailAsync(mailRequest);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        private string generateJwtToken(User user, IEnumerable<Claim> claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Tokens:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("groupId", user.GroupRef.ToString()),
                    new Claim("leaderId", user.ParentNId.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            tokenDescriptor.Subject.AddClaims(claims);
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private RefreshToken generateRefreshToken(string ipAddress)
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddDays(Convert.ToInt32(_config["Token:TimeExp"])),
                    Created = DateTime.UtcNow,
                    CreatedByIp = ipAddress
                };
            }
        }
        public async  Task<LoginResponse> RefreshToken(string token, string ipAddress)
        {
            var user =await _unitOfWork.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));

            // return null if no user found with token
            if (user == null) return null;

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            // return null if token is no longer active
            if (!refreshToken.IsActive) return null;

            // replace old refresh token with a new one and save
            var newRefreshToken = generateRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            user.RefreshTokens.Add(newRefreshToken);
            await this._unitOfWork.Complete();

            var avatar = user.Avatars.FirstOrDefault(a => a.IsMain);
            string path = null;

            if (avatar != null)
                path = avatar.Path;

            var roles = await _userManager.GetRolesAsync(user);

            // generate new jwt
            var jwtToken = generateJwtToken(user,await GetClaims(user));

            return new LoginResponse(user, jwtToken, newRefreshToken.Token, path, roles.FirstOrDefault());
        }

        public async Task<bool> RevokeToken(string token, string ipAddress)
        {
            var user = await _unitOfWork.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));

            // return false if no user found with token
            if (user == null)
                throw new Exception("Invalid token.");

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            // return false if token is not active
            if (!refreshToken.IsActive) return false;

            // revoke token and save
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;

            await _unitOfWork.Complete();
            return true;
        }

        public async Task<IList<Claim>> GetRoleClaimsAsync(IList<string> roles)
        {
            List<Claim> claims = new List<Claim>();
            foreach (var identityRole in roles)
            {
                var role =await _roleManager.FindByNameAsync(identityRole);
                claims.AddRange(await this._roleManager.GetClaimsAsync(role).ConfigureAwait(false));
            }
            return claims;
        }

        public async Task<IEnumerable<Claim>> GetClaims(User user)
        {
            //get roles
            var roles = await _userManager.GetRolesAsync(user);
            var userRoles = roles.Select(r => new Claim(ClaimTypes.Role, r)).ToArray();
            var userClaims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
            var roleClaims = await GetRoleClaimsAsync(roles).ConfigureAwait(false);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.GivenName, user.Name),
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim("ID",user.Id.ToString()),
            }.Union(userClaims).Union(roleClaims).Union(userRoles);
            return claims;
        }
    }
}
    