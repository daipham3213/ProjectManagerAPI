using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ProjectManagerAPI.Core;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Models.Resources;
using ProjectManagerAPI.Core.Models.ServiceResource;
using ProjectManagerAPI.Core.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly RoleManager<IdentityRole<Guid>> roleManager;

        public UserController(IUserService userService, IMapper mapper, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _userService = userService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        [HttpPost("authenticate")]
        [AllowAnonymous]
  
        public async Task<IActionResult> Authenticate([FromBody] LoginRequest request)
        {
            var response = await _userService.Authenticate(request);
            var result = new NotifyResult();
            if (response == null)
            {
                result.result = "Username or Password incorrect.";
                result.code = "400";
                return BadRequest(Json(result)); 
            }
            if (!response.IsActivated)
            {
                var callbackurl = _configuration["HostUrl:local"]+ "/api/User/sendActivationEmail?username="+request.Username;
                result.detail = callbackurl;
                result.result = "Account have not activated. Please click the link below to recive your activation email.";
                result.code = "400";
                return BadRequest(Json(result));
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
            else
            {
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
            }

            return BadRequest(error);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Search(string key)
        {
            var users = await this._userService.SearchUser(key);

            var result = this._mapper.Map<IList<User>, IList<SearchUserResource>>(users);

            return Ok(result);
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetUserProfile(string targetUsername)
        {
            var user = await this._unitOfWork.Users.GetUserProfile(targetUsername);
            
            if (user == null)
            {
                JsonResult res = new JsonResult(new NotifyResult { 
                    detail = "User could not be found.",
                    result = "Bad request.",
                    code = BadRequest().StatusCode.ToString()
            });
                res.StatusCode = BadRequest().StatusCode;
            }
            var result = this._mapper.Map<User, UserResource>(user);

            return Ok(result);
        }

        [HttpGet("sendChangeEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> SendChangeEmailRequest(string username, string newEmail)
        {
            var callbackurl = _configuration["HostUrl:local"] +"/api/User/confirmChangeEmail";

            await this._userService.SendChangeEmailRequest(username, newEmail, callbackurl);

            return Ok();
        }

        [HttpGet("sendActivationEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> SendActivationEmail(string username)
        {
            var callbackurl = _configuration["HostUrl:local"] + "/api/User/confirmActivation";

            await this._userService.SendActivationRequest(username, callbackurl);

            return Ok();
        }
        [HttpGet("confirmChangeEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmChangeEmail(string username, string newEmail, string token)
        {
            var user = await this._unitOfWork.Users.GetUser(username);
            if (user == null)
                return BadRequest();

            var result = await this._userService.ConfirmChangeEmail(username, newEmail, token);
            
            if (result)
                return Ok(Json(new NotifyResult { 
                    result = "Success change account email.",
                    code = "200"
                }));

            return Problem(detail: "Failed.", statusCode: 400);
        }

        [HttpGet("confirmActivation")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmActivation(string username, string token)
        {
            var user = await this._unitOfWork.Users.GetUser(username);
            if (user == null)
                return BadRequest();

            var result = await this._userService.ConfirmActivation(username, token);

            if (result)
            {
                user.IsActived = true;
                await this._unitOfWork.Complete();
                return Ok(Json(new NotifyResult
                {
                    result = "Account Activation Success.",
                    code = "200"
                }));
            }

            return Problem(detail: "Failed.", statusCode: 400);
        }
    }
}
