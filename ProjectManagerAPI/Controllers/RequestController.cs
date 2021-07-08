using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using ProjectManagerAPI.Core;
using ProjectManagerAPI.Core.Policy;
using ProjectManagerAPI.Core.Resources;
using ProjectManagerAPI.Core.Services;

namespace ProjectManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RequestController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthorizationService _authorizationService;
        private readonly IMapper _mapper;
        private readonly ITokenManager _tokenManager;
        private readonly IUserService _userService;

        public RequestController(
            IUnitOfWork unitOfWork,
            IAuthorizationService authorizationService,
            IMapper mapper,
            ITokenManager tokenManager,
            IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _authorizationService = authorizationService;
            _tokenManager = tokenManager;
            _userService = userService;
        }

        [HttpPut("activegroup")]
        public async Task<IActionResult> ActiveGroup(Guid requestId, bool isAccept)
        {
            var user = await this._tokenManager.GetUserByToken();
            var request = await this._unitOfWork.Requests.Get(requestId);
            //Authorization
            await this._authorizationService.AuthorizeAsync(User, request, Operations.RequestGroupActivation);
            var entity = await this._unitOfWork.Groups.Get(Guid.Parse(request.Value));
            //Update request
            if (isAccept)
                request.IsAccepted = true;
            else
            {
                request.IsDenied = true;
                this._unitOfWork.Groups.Remove(entity);
                await this._unitOfWork.Complete();
                return Ok(new { messsgae = "Group Activation Denied Success" });
            }
            //Set user's group id
            var lead = await _unitOfWork.Users.SearchUserById(entity.LeaderId);
            lead.Group = entity;
            lead.GroupRef = entity.Id;
            lead.DateModified = DateTime.Now;
            await this._userService.Promotion(lead.UserName);
            entity.IsActived = true;
            await _unitOfWork.Complete();
            return Ok(new {messsgae = "Group Activation Accept Success" });
        }

        [HttpGet]
        public async Task<IActionResult> GetRequest(string userName)
        {
            var user = await this._userService.GetUser(userName);
            if (user == null)
                throw new Exception("Username invalid.");
            var requests = await this._unitOfWork.Requests.GetNewRequestList(user.Id);
            return Ok(_mapper.Map<IEnumerable<RequestResource>>(requests));
        }
    }
}
