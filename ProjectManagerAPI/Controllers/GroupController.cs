using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagerAPI.Core;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Resources;
using ProjectManagerAPI.Core.ServiceResource;
using ProjectManagerAPI.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.JsonPatch.Operations;
using ProjectManagerAPI.Core.Permission;
using ProjectManagerAPI.Core.Policy;

namespace ProjectManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenManager _tokenParser;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IAuthorizationService _authorizationService;

        public GroupController(IUnitOfWork unitOfWork, ITokenManager tokenParser, IMapper mapper, IUserService userService, IAuthorizationService authorizationService)
        {
            _unitOfWork = unitOfWork;
            _tokenParser = tokenParser;
            _mapper = mapper;
            _userService = userService;
            _authorizationService = authorizationService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreatedGroup group)
        {
            if (!ModelState.IsValid)
                throw new Exception("Provided information is invalid");

            //Get user claims from token
            var user = await _tokenParser.GetUserByToken();
            //Create new group
            var groupType = await _unitOfWork.GroupTypes.Get(group.GroupTypeFk);
            if (groupType == null)
                throw new Exception("Invalid Group type.");

            var entity = new Group
            {
                Name = group.Name,
                Remark = group.Remark,
                UserCreated = user.Id,
                GroupTypeFk = group.GroupTypeFk,
                GroupType = groupType,
            };
            if (group.LeaderId == Guid.Empty | group.LeaderId == null) entity.LeaderId = user.Id;
            else entity.LeaderId = group.LeaderId.Value;
            //validation
            await this._authorizationService.AuthorizeAsync(User, entity, Operations.GroupCreate);
            //add new
            await _unitOfWork.Groups.Add(entity);
            await _unitOfWork.Complete();

            //Set user's group id
            var lead = await _unitOfWork.Users.SearchUserById(entity.LeaderId);
            entity = await _unitOfWork.Groups.FindGroupByName(group.Name);
            lead.Group = entity;
            lead.GroupRef = entity.Id;
            lead.DateModified = DateTime.Now;
            await this._userService.Promotion(lead.UserName);

            await _unitOfWork.Complete();
            return Ok(new JsonResult(_mapper.Map<CreatedGroup>(entity))
            {
                StatusCode = Ok().StatusCode
            });
        }
        [HttpGet("all")]
        [Authorize(Roles = RoleNames.RoleAdmin)]
        public async Task<IActionResult> GetGroups()
        {
            //Get user claims from token
            var user = await _tokenParser.GetUserByToken();
            var result = await _unitOfWork.Groups.GetAll();
            return Ok(_mapper.Map<IEnumerable<GroupViewResource>>(result));
        }
        [HttpGet]
        public async Task<IActionResult> GetGroupsValidated()
        {
            //Get user claims from token
            var user = await _tokenParser.GetUserByToken();
            await this._unitOfWork.Users.Load(u => u.IsActived & !u.IsDeleted);
            var leader = user.ParentN?.Id ?? user.Id;

            var result = await _unitOfWork.Groups.GetGroupListValidated(leader);
            return Ok(_mapper.Map<IEnumerable<GroupViewResource>>(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGroupValidated(Guid id)
        {
            //Get user claims from token
            var userM = await _tokenParser.GetUserByToken();
            await this._unitOfWork.Users.Load(u => u.Id == userM.ParentNId);
            
            var group = await this._unitOfWork.Groups.Get(id);

            await this._authorizationService.AuthorizeAsync(User, group, Operations.GroupRead);
            if (group == null)
                throw new Exception("Invalid group ID");
            return Ok(_mapper.Map<GroupResource>(group));
        }

        [HttpPost("addmember")]
        public async Task<IActionResult> AddMember([FromBody] AddMemberResource resource)
        {
            var group = await _unitOfWork.Groups.FindGroupByName(resource.GroupName);
            if (group == null)
                throw new Exception("Invalid group name.");
            //Check perm
            await this._authorizationService.AuthorizeAsync(User, group, Operations.GroupMemberUpdate);
            List<User> users = new List<User>();
            foreach (var username in resource.Usernames)
            {
                var user = await _unitOfWork.Users.SearchUserByUsername(username);
                if (user == null)
                    throw new Exception(username + " is an invalid username.");
                if (group.Users.Contains(user))
                    throw new Exception(username + " is already a member of " + group.Name);
                if (user.GroupRef != null)
                    throw new Exception(username + " is already a member of a group.");
                _unitOfWork.Groups.AddUserToGroup(user.Id, group.Id);
            }
            await _unitOfWork.Complete();
            return Ok(new JsonResult(_mapper.Map<GroupResource>(group)) { StatusCode = Ok().StatusCode });
        }

        [HttpPost("removemember")]
        public async Task<IActionResult> RemoveMember([FromBody] AddMemberResource resource)
        {
            var group = await _unitOfWork.Groups.FindGroupByName(resource.GroupName);
            if (group == null)
                throw new Exception("Invalid group name.");

            //Check perm
            await this._authorizationService.AuthorizeAsync(User, group, Operations.GroupMemberUpdate);
            List<User> users = new List<User>();
            foreach (var username in resource.Usernames)
            {
                var user = await _unitOfWork.Users.SearchUserByUsername(username);
                if (user == null)
                    throw new Exception(username + " is an invalid username.");
                if (!group.Users.Contains(user))
                    throw new Exception(username + " is not member of " + group.Name);
                if (user.Id == group.LeaderId)
                    throw new Exception("Can not remove leader from group.");
                _unitOfWork.Groups.RemoveUserFromGroup(user.Id, group.Id);
            }
            await _unitOfWork.Complete();
            return Ok(new JsonResult(_mapper.Map<GroupResource>(group)) { StatusCode = Ok().StatusCode });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveGroup(Guid id)
        {
            var group = await _unitOfWork.Groups.Get(id);
            await this._authorizationService.AuthorizeAsync(User, group, Operations.GroupDelete);

            var leader = this._unitOfWork.Users.Find(u => u.Id == group.LeaderId).FirstOrDefault();
            foreach (var user in group.Users)
                _unitOfWork.Groups.RemoveUserFromGroup(id, user.Id);
            _unitOfWork.Groups.Remove(group);
            await this._userService.DePromotion(leader.UserName);
            await _unitOfWork.Complete();
            return Ok(new JsonResult(group.Name + " removed successfully.") { StatusCode = Ok().StatusCode });
        }

        [HttpPut("promotion")]
        public async Task<IActionResult> Promotion(string username)
        {
            var user = await this._tokenParser.GetUserByToken();
            var new_leader = await this._userService.GetUser(username);
            var group = await this._unitOfWork.Groups.GetGroupByLeaderId(user.Id);
            await this._authorizationService.AuthorizeAsync(User, group, Operations.GroupLeaderUpdate);
            await this._userService.PromotionBy(user.UserName, new_leader.UserName);
            return Ok();
        }

        [HttpPost("leave")]
        public async Task<IActionResult> LeaveGroup()
        {
            var user = await this._tokenParser.GetUserByToken();
            await this._unitOfWork.Users.LeaveGroup(user.Id);
            return Ok();
        }
    }
}
