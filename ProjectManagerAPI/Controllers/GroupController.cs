using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectManagerAPI.Core;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Resources;
using ProjectManagerAPI.Core.Services;
using System;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenParser _tokenParser;

        public GroupController(IUnitOfWork unitOfWork, ITokenParser tokenParser)
        {
            _unitOfWork = unitOfWork;
            _tokenParser = tokenParser;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreatedGroup group)
        {
            if (!ModelState.IsValid)
                return new JsonResult("Provided infomation is invalid") {
                    StatusCode = BadRequest().StatusCode,
                };
            string token = await HttpContext.GetTokenAsync("access_token");
            var user = await _tokenParser.LoginResponse(token);

            var entity = new Group
            {
                Name = group.Name,
                Remark = group.Remark,
                UserCreated = user.Id,
                GroupTypeFK = group.GroupTypeFK
            };
            if (group.LeaderID == Guid.Empty) entity.LeaderID = user.Id;
            else
            {
                var leader = this._unitOfWork.Users.SearchUserById(group.LeaderID);
                if (leader.Result != null)
                    entity.LeaderID = group.LeaderID;
                else return BadRequest(new JsonResult("Leader ID is invalid.")
                {
                    StatusCode = BadRequest().StatusCode
                });
            }

            GroupType groupType = await this._unitOfWork.GroupTypes.Get(group.GroupTypeFK);   
            user = await this._unitOfWork.Users.SearchUserById(entity.LeaderID);
            
            await this._unitOfWork.Groups.Add(entity);
            await this._unitOfWork.Complete(); 
            return Ok(new JsonResult("Sucess.") { 
                StatusCode = Ok().StatusCode
            });
        }
    }
}
