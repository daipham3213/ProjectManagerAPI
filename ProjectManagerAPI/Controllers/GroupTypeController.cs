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
using ProjectManagerAPI.StaticValue;

namespace ProjectManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GroupTypeController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenManager _tokenParser;

        public GroupTypeController(IMapper mapper, IUnitOfWork unitOfWork, ITokenManager tokenParser)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _tokenParser = tokenParser;
        }

        [Authorize(Roles = RoleNames.RoleAdmin)]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var type = await _unitOfWork.GroupTypes.GetAll();
            return Ok(type);
        }
        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var type = await _unitOfWork.GroupTypes.LoadValidated();
            if (type == null)
            {
                return NotFound();
            }

            type = type.Where(u => u.IsActived == true);
            return Ok(_mapper.Map<IEnumerable<GroupTypeViewResource>>(type));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var type = await _unitOfWork.GroupTypes.Get(id);
            if (type == null)
            {
                return NotFound();
            }
            await _unitOfWork.GroupTypes.GetParents(type.Id);
            var result = _mapper.Map<GroupType, GroupTypeResource>(type);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = RoleNames.RoleAdmin)]
        public async Task<IActionResult> Post([FromBody] CreatedGroupType groupType)
        {
            if (!ModelState.IsValid)
                throw new Exception(ModelState.ToString());
            var user = await _tokenParser.GetUserByToken();

            if (user == null)
                throw new Exception("Authentication credentials is not provided");
            var parentN = _unitOfWork.GroupTypes.Find(c => c.Id == groupType.ParentN.Id);
            if (parentN == null & groupType.ParentNid != null)
                throw new Exception();

            //Init new entity
            var type = new GroupType();
            type.Name = groupType.Name;
            if (groupType.ParentNid != null)
                type.ParentN = await _unitOfWork.GroupTypes.SingleOrDefault(c => c.Id == groupType.ParentNid);
            type.Remark = groupType.Remark;
            type.UserCreated = user.Id;

            try
            {
                await _unitOfWork.GroupTypes.Add(type);
                await _unitOfWork.Complete();
                type = await _unitOfWork.GroupTypes.GetTypeByName(type.Name);
                await _unitOfWork.GroupTypes.GetParents(type.Id);
                var result = _mapper.Map<GroupType, CreatedGroupType>(type);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        [Authorize(Roles = RoleNames.RoleAdmin)]
        public async Task<IActionResult> DeleteType(Guid typeId)
        {
            var type = await _unitOfWork.GroupTypes.SingleOrDefault(c => c.Id == typeId);
            if (type == null)
                return BadRequest();

            _unitOfWork.GroupTypes.RemoveAllChildren(typeId);
            _unitOfWork.GroupTypes.Remove(type);
            await _unitOfWork.Complete();

            return Ok();
        }

    }
}
