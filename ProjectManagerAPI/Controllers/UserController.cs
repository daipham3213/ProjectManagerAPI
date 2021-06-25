using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectManagerAPI.Core;
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
    [Authorize(Roles = RoleNames.RoleUser)]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMailService _mailService;
        private readonly RoleManager<IdentityRole<Guid>> roleManager;

        public UserController(IUserService userService, IMapper mapper, IUnitOfWork unitOfWork, IMailService mailService)
        {
            _userService = userService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _mailService = mailService;
        }

        [HttpPost("authenticate")]
        [AllowAnonymous]
  
        public async Task<IActionResult> Authenticate([FromBody] LoginRequest request)
        {
            var response = await _userService.Authenticate(request);

            if (response == null)
                return BadRequest("Tài khoản hoặc mật khẩu không chính xác");

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
    }
}
