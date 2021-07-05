using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ProjectManagerAPI.Core;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Resources;
using ProjectManagerAPI.Core.ServiceResource;
using ProjectManagerAPI.Core.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(Roles = RoleNames.RoleUser)]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ITokenManager _tokenManager;
        public UserController(IUserService userService, IMapper mapper, IUnitOfWork unitOfWork, IConfiguration configuration, ITokenManager tokenManager)
        {
            _userService = userService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _tokenManager = tokenManager;
        }

        [HttpPost("authenticate")]
        [AllowAnonymous]

        public async Task<IActionResult> Authenticate([FromBody] LoginRequest request)
        {
            var response = await _userService.Authenticate(request, IpAddress());
            if (response == null)
                throw new Exception("Username or Password incorrect.");
            if (!response.IsActivated)
            {
                var callbackurl = _configuration["HostUrl:local"] + "/api/User/sendActivationEmail?username=" + request.Username;
                return BadRequest(new 
                    {message = "Account have not activated. Please click the link below to receive your activation email.", url = callbackurl}
                );
            }
            SetTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest user)
        {

            var errors = await _userService.Register(user);
            var error = new
            {
                EmailError = "Email already in use",
                UserNameError = "Username already exists",
            };

            if (errors == null)
            {
                await SendActivationEmail(user.Username);
                return Ok();
            }

            if (errors.Count == 1)
            {
                if (errors[0] == "Email already in use")
                {
                    error = new
                    {
                        EmailError = "Email already in use",
                        UserNameError = "",
                    };
                }
                else
                {
                    error = new
                    {
                        EmailError = "",
                        UserNameError = "Username already exists",
                    };
                }
            }

            return BadRequest(error);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Search(string key)
        {
            var users = await _userService.SearchUser(key);

            var result = _mapper.Map<IList<User>, IList<SearchUserResource>>(users);

            return Ok(result);
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetUserProfile(string key)
        {
            Guid guid = default;
            User user;
            if (Guid.TryParse(key, out guid))
                user = await _unitOfWork.Users.Get(guid);
            else user = await _unitOfWork.Users.GetUser(key);


            if (user == null)
                throw new Exception("Account could not be found");
            var result = _mapper.Map<User, UserResource>(user);

            return Ok(result);
        }

        [HttpGet("sendChangeEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> SendChangeEmailRequest(string username, string newEmail)
        {
            var callbackurl = _configuration["HostUrl:local"] + "/api/User/confirmChangeEmail";

            await _userService.SendChangeEmailRequest(username, newEmail, callbackurl);

            return Ok();
        }

        [HttpGet("sendActivationEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> SendActivationEmail(string username)
        {
            var callbackurl = _configuration["HostUrl:local"] + "/api/User/confirmActivation";

            await _userService.SendActivationRequest(username, callbackurl);

            return Ok();
        }
        [HttpGet("confirmChangeEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmChangeEmail(string username, string newEmail, string token)
        {
            var user = await _unitOfWork.Users.GetUser(username);
            if (user == null)
                return BadRequest();

            var result = await _userService.ConfirmChangeEmail(username, newEmail, token);

            if (result)
                return Ok(new {message = "Account Email Changed Successfully." });

            return Problem(detail: "Failed.", statusCode: 400);
        }

        [HttpGet("confirmActivation")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmActivation(string username, string token)
        {
            var user = await _unitOfWork.Users.GetUser(username);
            if (user == null)
                return BadRequest();

            var result = await _userService.ConfirmActivation(username, token);

            if (result)
            {
                user.IsActived = true;
                await _unitOfWork.Complete();
                return Ok(new {message = "Account Activation Success." });
            }

            return Problem(detail: "Failed.", statusCode: 400);
        }
        [HttpPut("changePassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ChangePassword([FromBody] UpdatePasswordResource updatePasswordResource)
        {
            var user = await this._unitOfWork.Users.GetUser(updatePasswordResource.UserName);
            if (user == null)
                throw new Exception("Username is invalid.");

            var currentPass = updatePasswordResource.CurrentPassword;
            var newPass = updatePasswordResource.NewPassword;

            if (!await this._userService.CheckPassword(user.UserName, currentPass))
                throw new Exception("Password is incorrect");

            if (!updatePasswordResource.NewPassword.Equals(updatePasswordResource.NewPasswordConfirm))
                throw new Exception("Password confirmation is incorrect.");
            if (currentPass == newPass)
                ModelState.AddModelError("NewPassword", "The new password and the current password cannot be the same");

            if (!ModelState.IsValid)
                throw new Exception(ModelState.ToString());

            var result = await this._userService.ChangePassword(user.UserName, updatePasswordResource.CurrentPassword, updatePasswordResource.NewPassword);

            if (result == true)
                return Ok(new {message = "Password changed successfully."});

            return Problem();
        }
        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = await _userService.RefreshToken(refreshToken, IpAddress());

            if (response == null)
                return Unauthorized(new { message = "Invalid token" });

            SetTokenCookie(response.RefreshToken);

            return Ok(response);
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest model)
        {
            // accept token from request body or cookie
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });

            var response = await _userService.RevokeToken(token, IpAddress());

            if (!response)
                return NotFound(new { message = "Token not found" });

            return Ok(new { message = "Token revoked" });
        }

        [HttpGet("{id}/refresh-tokens")]
        public async Task<IActionResult> GetRefreshTokens(Guid id)
        {
            var user =await this._unitOfWork.Users.SearchUserById(id);
            if (user == null) return NotFound();

            return Ok(user.RefreshTokens);
        }

        // helper methods

        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
        }
    }
}
