using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using ProjectManagerAPI.Core;
using ProjectManagerAPI.Core.Models;
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

        [HttpGet("activegroup")]
        public async Task<IActionResult> ActiveGroup(Guid requestId, bool isAccept)
        {
            var user = await this._tokenManager.GetUserByToken();
            var request = await this._unitOfWork.Requests.Get(requestId);
            //Authorization
            await this._authorizationService.AuthorizeAsync(User, request, Operations.RequestGroupActivation);
            var entity = await this._unitOfWork.Groups.Get(Guid.Parse(request.Value));
            //Update request
            var isAccepted = true && isAccept;
            await this._unitOfWork.Requests.ProcessRequest(requestId, isAccepted, !isAccepted);

            //Set user's group id
            var lead = await _unitOfWork.Users.SearchUserById(entity.LeaderId);
            lead.Group = entity;
            lead.GroupRef = entity.Id;
            lead.DateModified = DateTime.Now;
            await this._unitOfWork.Groups.Load(u => u.Id == entity.ParentNId);
            lead.ParentNId = entity.ParentN?.LeaderId;
            await this._userService.Promotion(lead.UserName);
            entity.IsActived = true;
            await _unitOfWork.Complete();
            return Ok(new {messsgae = "Group Activation Accept Success" });
        }

        [HttpGet]
        public async Task<IActionResult> GetRequest()
        {
            var user = await this._userService.GetUser(User.Identity.Name);
            if (user == null)
                throw new Exception("Username invalid.");
            var requests = await this._unitOfWork.Requests.GetNewRequestList(user.Id);
            foreach (var request in await _unitOfWork.Requests.GetOldRequestList(user.Id))
                requests.Append<Request>(request);

            if (!requests.Any())
                throw new Exception("No new request");
            await this._authorizationService.AuthorizeAsync(User, requests.First(), Operations.RequestRead);
            return Ok(_mapper.Map<IEnumerable<RequestResource>>(requests));
        }
    }
}
