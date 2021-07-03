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
            var response = await _userService.Authenticate(request);
            if (response == null)
                throw new Exception("Username or Password incorrect.");
            if (!response.IsActivated)
            {
                var callbackurl = _configuration["HostUrl:local"] + "/api/User/sendActivationEmail?username=" + request.Username;
                throw new Exception(
                    "Account have not activated. Please click the link below to recive your activation email.\n url: " +
                    callbackurl
                );
            }

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
                return Ok(new JsonResult("Account Email Changed Successfully.") { StatusCode = 200, ContentType = "application/json" });

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
                return Ok(new JsonResult("Account Activation Success.") { StatusCode = 200, ContentType = "application/json" });
            }

            return Problem(detail: "Failed.", statusCode: 400);
        }
    }
}
