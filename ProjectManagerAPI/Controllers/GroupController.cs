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
            if (user.IsSuperuser)
            {
                await AdminGroupCreate(n, parent);
                return Ok(_mapper.Map<CreatedDepartment>(entity));
            }
            //Send request to admin to active group
            var request = new CreatedRequest()
            {
                Name = RequestNames.CreateGroup,
                Remark = groupType.Name,
                To = parent.Id, //Sent to Admin group
                Value = n.Id.ToString() //Group need to activate
            };
            await this._unitOfWork.Requests.CreateRequest(request, user);
            
            return Ok(_mapper.Map<CreatedDepartment>(entity));
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
            var parent = await this._unitOfWork.Groups.Get(group.ParentNId);
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
            if (user.IsSuperuser)
            {
                await AdminGroupCreate(n, parent);
                return Ok(_mapper.Map<CreatedDepartment>(entity));
            }
            //Send request to admin to active group
            var adminGroup = await this._unitOfWork.Groups.FindGroupByName("System Admin Group");
            var request = new CreatedRequest()
            {
                Name = RequestNames.CreateGroup,
                Remark = n.GroupType.Name +" - "+ n.Name,
                To = adminGroup.Id, //Sent to Admin group
                Value = n.Id.ToString() //Group need to activate
            };
            await this._unitOfWork.Requests.CreateRequest(request, user);
            
            return Ok(_mapper.Map<CreatedDepartment>(entity));
        }

        [HttpGet("all")]
        [Authorize(Roles = RoleNames.RoleAdmin)]
        public async Task<IActionResult> GetGroups()
        {
            //Get user claims from token
            var result = await _unitOfWork.Groups.GetAll();
            return Ok(_mapper.Map<IEnumerable<GroupViewResource>>(result));
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetList(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new Exception("Invalid keyword.");
            var gt = await this._unitOfWork.GroupTypes.GetTypeByName(key);
            var result = this._unitOfWork.Groups.Find(u => u.GroupTypeFk == gt.Id && u.IsActived);
            if (!result.Any())
                throw new Exception("Invalid keyword.");
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
            }

            var list = _mapper.Map<IEnumerable<GroupViewResource>>(result);
            foreach (var resource in list)
            {
                var gtype = (await this._unitOfWork.GroupTypes.Get(resource.GroupTypeFk)).Name;
                var leaderName = (await this._unitOfWork.Users.Get(resource.LeaderId)).Name;
                resource.GroupType = gtype;
                resource.Leader = leaderName;
            }
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGroupValidated(Guid id)
        {
            //Get user claims from token
            var group = await this._unitOfWork.Groups.Get(id);
            await this._unitOfWork.Users.Load(u => u.Id == group.LeaderId);

            await this._authorizationService.AuthorizeAsync(User, group, Operations.GroupRead);
            await this._unitOfWork.Users.Load(u => u.GroupRef == group.Id);
           
            if (group == null)
                throw new Exception("Invalid group ID");
            var result = _mapper.Map<GroupResource>(group);
            result.GroupType = (await this._unitOfWork.GroupTypes.Get(group.GroupTypeFk)).Name;
            return Ok(result);
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
            var result = _mapper.Map<GroupResource>(group);
            result.GroupType = (await this._unitOfWork.GroupTypes.Get(group.GroupTypeFk)).Name;
            foreach (var resultUser in result.Users)
            {
                resultUser.Contrib = await this._unitOfWork.Tasks.GetContrib(resultUser.Id); ;
            }
            return Ok(result);
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
                await _unitOfWork.Groups.AddUserToGroup(user.Id, group.Id);
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
       
            var child = this._unitOfWork.Groups.Find(u => u.ParentNId == group.Id);
            if (child.Any())
                throw new Exception("Please remove child groups before remove this group.");

            var leader = this._unitOfWork.Users.Find(u => u.Id == group.LeaderId).FirstOrDefault();
            await this._userService.DePromotion(leader.UserName);

            foreach (var user in group.Users.ToList())
                await _unitOfWork.Groups.RemoveUserFromGroup(user.Id, group.Id);
            
            foreach (var report in group.Reports.ToList())
            {
                foreach (var phase in report.Phases.ToList())
                {
                    foreach (var task in phase.Tasks.ToList())
                    {
                        await this._unitOfWork.Tasks.RemoveChild(task);
                        this._unitOfWork.Tasks.Remove(task);
                    }
                    this._unitOfWork.Phases.Remove(phase);
                }
                this._unitOfWork.Reports.Remove(report);
            }
            _unitOfWork.Groups.Remove(group);
            await _unitOfWork.Complete();
            return Ok(new{message = group.Name + " removed successfully." });
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

        private async Task AdminGroupCreate(Group entity, Group parent)
        {
            var lead = await _unitOfWork.Users.SearchUserById(entity.LeaderId);
            lead.Group = entity;
            lead.GroupRef = entity.Id;
            lead.DateModified = DateTime.Now;
            lead.ParentNId = parent.LeaderId;
            await this._userService.Promotion(lead.UserName);
            entity.IsActived = true;
            await _unitOfWork.Complete();
        }
    }
}
