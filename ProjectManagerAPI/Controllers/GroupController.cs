using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagerAPI.Core;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Resources;
using ProjectManagerAPI.Core.ServiceResource;
using ProjectManagerAPI.Core.Services;

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

        public GroupController(IUnitOfWork unitOfWork, ITokenManager tokenParser, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _tokenParser = tokenParser;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreatedGroup group)
        {
            if (!ModelState.IsValid)
                throw new Exception("Provided information is invalid");

            //Get user claims from token
            var user = await _tokenParser.GetUserByToken();
            //Create new group
            var grouptype = await _unitOfWork.GroupTypes.Get(group.GroupTypeFk);
            if (grouptype == null)
                throw new Exception("Invalid Group type.");
           
            var entity = new Group
            {
                Name = group.Name,
                Remark = group.Remark,
                UserCreated = user.Id,
                GroupTypeFk = group.GroupTypeFk,
                GroupType = grouptype,
            };
            if (group.LeaderId == Guid.Empty | group.LeaderId == null) entity.LeaderId = user.Id;
            else
            {
                var leader = _unitOfWork.Users.SearchUserById(group.LeaderId.Value);
                if (leader.Result != null)
                {
                    if (leader.Result.GroupRef != null)
                        throw new Exception("You already have a group!");
                    entity.LeaderId = group.LeaderId.Value;
                }
                else throw new Exception("Leader ID is invalid.");

            }
            await _unitOfWork.Groups.Add(entity);
            await _unitOfWork.Complete();
            //Set user's group id
            var lead = await _unitOfWork.Users.SearchUserById(entity.LeaderId);
            entity = await _unitOfWork.Groups.FindGroupByName(group.Name);
            lead.Group = entity;
            lead.GroupRef = entity.Id;
            lead.DateModified = DateTime.Now;
            await _unitOfWork.Complete();
            return Ok(new JsonResult(_mapper.Map<CreatedGroup>(entity)) {
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
            var result = await _unitOfWork.Groups.GetGroupListValidated(user.Id);

            return Ok(_mapper.Map<IEnumerable<GroupViewResource>>(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGroupValidated(Guid id)
        {
            //Get user claims from token
            var userM = await _tokenParser.GetUserByToken();
            ICollection<Group> result;
            if (userM.ParentN != null)
                result = await _unitOfWork.Groups.GetGroupListValidated(userM.ParentN.Id);
            else result = await _unitOfWork.Groups.GetGroupListValidated(userM.Id);

            foreach (var group in result)
                if (group.Id == id)
                {
                    //foreach (var user in group.Users)
                    //    await this._unitOfWork.Users.Load(c => c.Id == user.Id);
                    return Ok(_mapper.Map<GroupResource>(group));
                }

            throw new Exception("Invalid Group ID.");
        }

        [HttpPost("addmember")]
        public async Task<IActionResult> AddMember([FromBody] AddMemberResource resource)
        {
            var group = await _unitOfWork.Groups.FindGroupByName(resource.GroupName);
            if (group == null)
                throw new Exception("Invalid group name.");
            //Get user claims from token
            var userM = await _tokenParser.GetUserByToken();
            var list = await _unitOfWork.Groups.GetGroupListValidated(userM.Id);
            if (!list.Contains(group))
                return BadRequest(new JsonResult("Permission not allowed.")
                {
                    StatusCode = BadRequest().StatusCode
                });
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
           
            //Get user claims from token
            var userM = await _tokenParser.GetUserByToken();
            var list = await _unitOfWork.Groups.GetGroupListValidated(userM.Id);
            if (!list.Contains(group))
                throw new Exception("Permission not allowed.");
            List<User> users = new List<User>();
            foreach (var username in resource.Usernames)
            {
                var user = await _unitOfWork.Users.SearchUserByUsername(username);
                if (user == null)
                    throw new Exception(username + " is an invalid username.");
                if (!group.Users.Contains(user))
                    throw new Exception(username + " is not member of " + group.Name);
                _unitOfWork.Groups.RemoveUserFromGroup(user.Id, group.Id);
            }
            await _unitOfWork.Complete();
            return Ok(new JsonResult(_mapper.Map<GroupResource>(group)) { StatusCode = Ok().StatusCode });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveGroup(Guid id)
        {
            var group = await _unitOfWork.Groups.Get(id);
            if (group == null)
                throw new Exception("Group id is invalid");
            //Get user claims from token
            var userM = await _tokenParser.GetUserByToken();
            var list = await _unitOfWork.Groups.GetGroupListValidated(userM.Id);
            if (!list.Contains(group))
                throw new Exception("Permission not allowed.");
            foreach (var user in group.Users)
                _unitOfWork.Groups.RemoveUserFromGroup(id, user.Id);
            _unitOfWork.Groups.Remove(group);
            await _unitOfWork.Complete();
            return Ok(new JsonResult(group.Name + " removed successfully.") { StatusCode = Ok().StatusCode });
        }
    }
}
