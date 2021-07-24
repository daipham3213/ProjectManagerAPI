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
using ProjectManagerAPI.StaticValue;
using Task = System.Threading.Tasks.Task;

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

        [HttpPost("department")]
        public async Task<IActionResult> Post([FromBody] CreatedDepartment group)
        {
            if (!ModelState.IsValid)
                throw new Exception("Provided information is invalid");

            //Get user claims from token
            var user = await _tokenParser.GetUserByToken();
            //Create new group
            var groupType = _unitOfWork.GroupTypes
                .Find(u => u.Name == "Department")
                .FirstOrDefault();
            var parent = await this._unitOfWork.Groups.FindGroupByName("System Admin Group");
            var entity = new Group
            {
                Name = group.Name,
                Remark = group.Remark,
                UserCreated = user.Id,
                GroupTypeFk = groupType.Id,
                GroupType = groupType,
                IsActived = false,
                ParentN = parent,
                ParentNId = parent.Id,
            };
            if (group.LeaderId == Guid.Empty | group.LeaderId == null) entity.LeaderId = user.Id;
            else entity.LeaderId = group.LeaderId.Value;
            //validation
            await this._authorizationService.AuthorizeAsync(User, entity, Operations.GroupCreate);
            //add new
            await _unitOfWork.Groups.Add(entity);
            await _unitOfWork.Complete();

            var n = await this._unitOfWork.Groups.SingleOrDefault(u => u.Name == entity.Name);
            //Send request to admin to active group
            var request = new CreatedRequest()
            {
                Name = RequestNames.CreateGroup,
                Remark = groupType.Name,
                To = parent.Id, //Sent to Admin group
                Value = n.Id.ToString() //Group need to activate
            };
            await this._unitOfWork.Requests.CreateRequest(request, user);
            
            return Ok(new JsonResult(_mapper.Map<CreatedDepartment>(entity))
            {
                StatusCode = Ok().StatusCode
            });
        }

        [HttpPost("Team")]
        public async Task<IActionResult> Post([FromBody] CreatedTeam group)
        {
            if (!ModelState.IsValid)
                throw new Exception("Provided information is invalid");

            //Get user claims from token
            var user = await _tokenParser.GetUserByToken();
            //Create new group
            var groupType = _unitOfWork.GroupTypes
                .Find(u => u.Name == "Group")
                .FirstOrDefault();
            var parent = await this._unitOfWork.Groups.Get(group.parentNId);
            var entity = new Group
            {
                Name = group.Name,
                Remark = group.Remark,
                UserCreated = user.Id,
                GroupTypeFk = groupType.Id,
                GroupType = groupType,
                IsActived = false,
                ParentN = parent,
                ParentNId = parent.Id,
            };
            if (group.LeaderId == Guid.Empty | group.LeaderId == null) entity.LeaderId = user.Id;
            else entity.LeaderId = group.LeaderId.Value;
            //validation
            await this._authorizationService.AuthorizeAsync(User, entity, Operations.GroupCreate);
            //add new
            await _unitOfWork.Groups.Add(entity);
            await _unitOfWork.Complete();

            //Send request to admin to active group
            var request = new CreatedRequest()
            {
                Name = RequestNames.CreateGroup,
                Remark = entity.GroupType.Name,
                To = parent.Id, //Sent to Admin group
                Value = entity.Id.ToString() //Group need to activate
            };
            await this._unitOfWork.Requests.CreateRequest(request, user);
            
            return Ok(new JsonResult(_mapper.Map<CreatedDepartment>(entity))
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

        [HttpGet("type")]
        public async Task<IActionResult> GetGroupsValidated(string? key)
        {
            //Get user claims from token
            var user = await _tokenParser.GetUserByToken();
            await this._unitOfWork.Users.Load(u => u.Id == user.ParentNId);
            // await this._unitOfWork.Groups.Load(u => u.Id == user.GroupRef);
            var leader = user.ParentN?.Id ?? user.Id;

            var result = await _unitOfWork.Groups.GetGroupListValidated(leader);
            var adminType = await this._unitOfWork.GroupTypes.GetTypeByName("System Admin");
            result = result.Where(u => u.GroupTypeFk != adminType.Id).ToList();

            if (key != null & key != "")
            {
                var gt = await this._unitOfWork.GroupTypes.GetTypeByName(key);
               
                result = result.Where(u => u.GroupTypeFk == gt.Id).ToList();
                foreach (var res in result)
                {
                    await this._unitOfWork.Users.Load(u => u.Id == res.LeaderId);
                }
            }
            return Ok(_mapper.Map<IEnumerable<GroupViewResource>>(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGroupValidated(Guid id)
        {
            //Get user claims from token
            var group = await this._unitOfWork.Groups.Get(id);
            await this._unitOfWork.Users.Load(u => u.Id == group.LeaderId);

            await this._authorizationService.AuthorizeAsync(User, group, Operations.GroupRead);
            await this._unitOfWork.Users.Load(u => u.GroupRef == group.Id);
            await this._unitOfWork.GroupTypes.Load(u => u.Id == group.GroupTypeFk);
            if (group == null)
                throw new Exception("Invalid group ID");
            return Ok(_mapper.Map<GroupResource>(group));
        }

        [HttpGet]
        public async Task<IActionResult> GetGroupByUser()
        {
            var user = await _tokenParser.GetUserByToken();
            if (user.GroupRef == null)
                throw new Exception("You haven't join a group.");
            var group = await _unitOfWork.Groups.Get(user.GroupRef.Value);
            await this._unitOfWork.Users.Load(u => u.Id == group.LeaderId);
            if (group == null)
                throw new Exception("Invalid group ID");
            await this._unitOfWork.GroupTypes.Load(u => u.Id == group.GroupTypeFk);
            await this._unitOfWork.Users.Load(u => u.GroupRef == group.Id);
            return Ok(_mapper.Map<GroupResource>(group));
        }

        [HttpGet("{id}/members")]
        public async Task<IActionResult> GetMemberList(Guid id)
        {
            var group = await this._unitOfWork.Groups.Get(id);
            if (group == null)
                return NotFound(new {message = "Invalid group"});
            var members = this._unitOfWork.Users.Find(u =>
                u.GroupRef == group.Id && u.IsActived == true && u.IsDeleted == false);
            return Ok(_mapper.Map<IEnumerable<UserResource>>(members));
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
                var user = await _unitOfWork.Users.Get(username);
                if (user == null)
                    throw new Exception(username + " is an invalid username.");
                if (group.Users.Contains(user))
                    throw new Exception(username + " is already a member of " + group.Name);
                if (user.GroupRef != null)
                    throw new Exception(username + " is already a member of a group.");
                _unitOfWork.Groups.AddUserToGroup(user.Id, group.Id);
            }
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
                var user = await _unitOfWork.Users.Get(username);
                if (user == null)
                    throw new Exception(username + " is an invalid user.");
                if (!group.Users.Contains(user))
                    throw new Exception(username + " is not member of " + group.Name);
                if (user.Id == group.LeaderId)
                    throw new Exception("Can not remove leader from group.");
                _unitOfWork.Groups.RemoveUserFromGroup(user.Id, group.Id);
            }
            //await _unitOfWork.Complete();
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
            //await _unitOfWork.Complete();
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
