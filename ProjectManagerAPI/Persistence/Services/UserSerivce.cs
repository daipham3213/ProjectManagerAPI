using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProjectManagerAPI.Core;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.ServiceResource;
using ProjectManagerAPI.Core.Services;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagerAPI.Persistence.Services
{
    public class UserSerivce : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMailService _mailService;
        private readonly IConfiguration _config;

        public UserSerivce(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IUnitOfWork unitOfWork,
            IMailService mailService,
            IConfiguration config
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _mailService = mailService;
            _config = config;
        }

        public async Task<LoginResponse> Authenticate(LoginRequest request)
        {
            // check user exitst
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
                return null;

            //check password
            var result = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, true);
            if (!result.Succeeded)
                return null;

            var isActivated = user.IsActived;
            if (!isActivated)
                return new LoginResponse{
                    IsActivated = false
                };

            //get roles
            var roles = await _userManager.GetRolesAsync(user);

            var name = user.Name;
            if (name == null)
                name = "undefined";
            //create claims

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.GivenName, user.Name),
                new Claim(ClaimTypes.Name,user.UserName)
            };
            foreach (var i in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, i));
            }

            //create token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity((claims)),
                Expires = DateTime.Now.AddMonths(2),
                SigningCredentials = creds,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var finalToken = tokenHandler.WriteToken(token);

            //Load Avatar
            await _unitOfWork.Avatars.Load(a => a.UserId == user.Id && a.IsMain);

            var avatar = user.Avatars.Where(a => a.IsMain).FirstOrDefault();
            string path = null;

            if (avatar != null)
                path = avatar.Path;

            var loginResponse = new LoginResponse
            {
                UserName = user.UserName,
                Token = finalToken,
                DisplayName = user.Name,
                AvatarUrl = path,
                RoleName = roles.FirstOrDefault(),
                IsActivated = user.IsActived
            };

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
            await this._userManager.AddToRoleAsync(user, user.Group.GroupType.IdentityRole.Name);
        }

        public async Task DePromotion(string username)
        {
            var leader = await _userManager.FindByNameAsync(username);
            await this._userManager.RemoveFromRoleAsync(leader, leader.Group.GroupType.IdentityRole.Name);
        }

        public async Task<bool> PromotionBy(string lead_username, string promo_username)
        {
            var leader = await this._userManager.FindByNameAsync(lead_username);
            var user = await this._userManager.FindByNameAsync(promo_username);
            var group = leader.Group;

            if (leader == null | user == null | group == null)
                throw new Exception("Invalid information.");
            if (leader.GroupRef != leader.Id)
                throw new Exception("Permission not allowed.");
            if (group.Id != user.GroupRef)
                throw new Exception(promo_username + "  is not a member of " + group.Name);

            leader.ParentN = user;
            group.LeaderId = user.Id;

            await this._userManager.RemoveFromRoleAsync(leader, group.GroupType.IdentityRole.Name);
            await this._userManager.AddToRoleAsync(user, group.GroupType.IdentityRole.Name);
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
    }
}