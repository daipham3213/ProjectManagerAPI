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
using ProjectManagerAPI.Core.Policy;

namespace ProjectManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ITokenManager _tokenParser;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthorizationService _authorizationService;

        public TaskController(ITokenManager tokenParser, IMapper mapper, IUnitOfWork unitOfWork, IAuthorizationService authorizationService)
        {
            _tokenParser = tokenParser;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _authorizationService = authorizationService;
        }


        [HttpGet("all")]
        [Authorize(Roles = RoleNames.RoleAdmin)]
        public async Task<IActionResult> GetAll()
        {
            var task = await _unitOfWork.Tasks.GetAll();
            return Ok(task);
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var task = await _unitOfWork.Tasks.LoadValidated();
            if (task.Count() == 0)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<IEnumerable<TaskViewResource>>(task));
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatedTask task)
        {
            if (!ModelState.IsValid)
                throw new Exception("The information provided for creation is not valid");
            var user = await _tokenParser.GetUserByToken();

            var phase = await _unitOfWork.Phases.Get(task.PhaseId);
            if (phase == null)
            {
                throw new Exception("Invalid Phase id");
            }

            var checkUser = await _unitOfWork.Users.Get(task.UserId);
            if (checkUser == null)
            {
                throw new Exception("Invalid user id");
            }

            var entity = new Core.Models.Task
            {
                Name = task.Name,
                Remark = task.Remark,
                UserCreated = user.Id,
                StartDate = task.StartDate,
                DueDate = task.DueDate,
                PhaseId = task.PhaseId,
                Phase = phase,
                UserId = task.UserId,
                User = checkUser
            };

            await this._authorizationService.AuthorizeAsync(User, entity, Operations.TaskCreate);

            await this._unitOfWork.Tasks.Add(entity);
            await this._unitOfWork.Complete();

            return Ok(new JsonResult(_mapper.Map<CreatedTask>(entity))
            {
                StatusCode = Ok().StatusCode
            });
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskId(Guid id) {
            await this._unitOfWork.Tasks.GetAll();
              var task = await this._unitOfWork.Tasks.Get(id);
            if (task == null)
                throw new Exception("Invalid Task ID.");
            await this._authorizationService.AuthorizeAsync(User, task, Operations.TaskRead);
            var result = _mapper.Map<Core.Models.Task, TaskResourcecs>(task);
            return Ok(result);
        }
    }
}
