using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagerAPI.Core;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Models.Resources;
using ProjectManagerAPI.Core.Models.Services;
using ProjectManagerAPI.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupTypeController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGroupTypeService _groupTypeService;
        public GroupTypeController(IUnitOfWork unitOfWork, IMapper mapper,IGroupTypeService groupTypeService)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._groupTypeService = groupTypeService;
        }
        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var type = await _unitOfWork.GroupType.GetAll();
            return Ok(type);
        }
        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var type = await _unitOfWork.GroupType.LoadValidated();
            if (type == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<IEnumerable<GroupTypeViewResource>>(type));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            await this._unitOfWork.GroupType.GetAll();
            var type = await _unitOfWork.GroupType.Get(id);
            if (type == null)
            {
                return NotFound();
            }
            this._unitOfWork.GroupType.LoadParent(type);
            var result = _mapper.Map<GroupType, GroupTypeResource>(type);
            return Ok(result);
        }
    }
}
