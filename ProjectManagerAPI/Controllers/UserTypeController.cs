using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    [Authorize]
    public class UserTypeController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenParser _tokenParser;

        public UserTypeController(IMapper mapper, IUnitOfWork unitOfWork, ITokenParser tokenParser)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _tokenParser = tokenParser;
        }

        [HttpPost]
        //[Authorize(Roles = RoleNames.RoleAdmin)]
        public async Task<IActionResult> Post([FromBody]CreatedUserType usertype)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string token = await HttpContext.GetTokenAsync("access_token");
            var user = await _tokenParser.LoginResponse(token);
            if (user == null)
                return BadRequest("Authentication credentials is not provided");
            var parent_n = this._unitOfWork.UserTypes.Find(c => c.ID == usertype.ParentNID);
            if (parent_n == null & usertype.ParentNID != null)
                return BadRequest();
            var type = new UserType();
            type.Name = usertype.Name;
            if (usertype.ParentNID != null)
                type.ParentN = await this._unitOfWork.UserTypes.SingleOrDefault(c => c.ID == usertype.ParentNID);
            type.Remark = usertype.Remark;
            type.IsActived = true;
            type.IsDeleted = false;
            type.DateCreated = DateTime.Now;
            type.DateModified = DateTime.Now;
            type.UserCreated = user.Id;
            try
            {
                await this._unitOfWork.UserTypes.Add(type);
                await this._unitOfWork.Complete();
                type = await this._unitOfWork.UserTypes.GetTypeByName(type.Name);
                await this._unitOfWork.UserTypes.GetAll();
                var result = this._mapper.Map<UserType, CreatedUserType>(type);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("all")]
        [Authorize(Roles = RoleNames.RoleAdmin)]
        public async Task<IActionResult> GetAll()
        {
            var types = await this._unitOfWork.UserTypes.GetAll();
            
            var result = this._mapper.Map<IEnumerable<UserTypeViewResource>>(types);
            return Ok(result);
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var types = await this._unitOfWork.UserTypes.LoadValidated();
            var result = this._mapper.Map<IEnumerable<UserTypeViewResource>>(types);
            return Ok(result);
        }
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(Guid id)
        {
            await this._unitOfWork.UserTypes.GetAll();
            var type = await this._unitOfWork.UserTypes.Get(id);
            if (type == null)
            {
                return NotFound();
            }
            this._unitOfWork.UserTypes.LoadParent(type);
            var result = this._mapper.Map<UserType,UserTypeResource>(type);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            var type = await this._unitOfWork.UserTypes.Get(id);
            if (type == null)
            {
                return BadRequest("This entity does not exist.");
            }
            this._unitOfWork.UserTypes.RemoveAllChild(id);
            this._unitOfWork.UserTypes.Remove(type);
            await this._unitOfWork.Complete();
            return Ok("Entity "+ id + " and its childs have been removed");
        }
    }
}
