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
using Task = ProjectManagerAPI.Core.Models.Task;

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
        public async Task<IActionResult> GetListByUser()
        {
            var user =await _tokenParser.GetUserByToken();
            if (user == null)
                return NotFound();
            var list = await _unitOfWork.Tasks.LoadByUser(user);
           
            var now = DateTime.UtcNow.AddHours(7);
            list = list.Where(u => u.Percent < 100);
            foreach (var task in list)
            {
                if (task.ParentN == null || task.ParentN.Id == null || task.ParentN.Id == Guid.Empty) continue;
                if (!list.Any(u => u.Id == task.ParentN.Id)) task.ParentN = null;
            }
            return Ok(_mapper.Map<IEnumerable<TaskViewResource>>(list));
        }

        [HttpGet("report")]
        public async Task<IActionResult> GetListByReport(Guid reportId)
        {
            var user = await this._tokenParser.GetUserByToken();
            var report = await this._unitOfWork.Reports.Get(reportId);
            if (report == null)
                return NotFound();
            await this._unitOfWork.Phases.Load(u => u.ReportId == reportId);
            await this._unitOfWork.Tasks.LoadByReport(report);
            foreach (var phase in report.Phases)
            {
                await this._unitOfWork.Tasks.Load(u=> u.PhaseId == phase.Id);
            }
            return Ok(_mapper.Map<ReportResource>(report));
        }

        [HttpGet("phase")]
        public async Task<IActionResult> GetListByPhase(Guid phaseId)
        {
            var user = await this._tokenParser.GetUserByToken();
            var phase = await this._unitOfWork.Phases.Get(phaseId);
            if (phase == null)
                return NotFound();
            return Ok(_mapper.Map<IEnumerable<TaskViewResource>>(await this._unitOfWork.Tasks.LoadByPhase(phase)));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatedTask task)
        {
            if (!ModelState.IsValid)
                throw new Exception("The information provided for creation is not valid");
            var user = await _tokenParser.GetUserByToken();

            var phase = await _unitOfWork.Phases.Get(task.PhaseId);
            if (phase == null)
                throw new Exception("Invalid Phase id");
            if (task.StartDate < phase.StartDate)
                throw new Exception("Start date can not be smaller than " + phase.StartDate + "of Phase " + phase.Name);
            if (task.DueDate > phase.DueDate)
                throw new Exception("Start date can not be larger than " + phase.DueDate + "of Phase " + phase.Name);

            var checkUser = await _unitOfWork.Users.Get(task.UserId);
            if (checkUser == null)
            {
                throw new Exception("Invalid user id");
            }

            var checkP = await _unitOfWork.Tasks.Get(task.ParentNId ?? Guid.Empty);
            if (checkP == null && task.ParentNId != null)
                throw new Exception("Invalid parent task id");

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
                User = checkUser,
                ParentN = checkP,
                DateCreated = DateTime.UtcNow.AddHours(7),
            };

            if (checkP != null) entity.StartDate = checkP.DueDate;
            entity.StartDate ??= phase.StartDate;
            if (entity.DueDate == entity.StartDate || entity.DueDate == null) entity.DueDate = entity.StartDate.Value.AddDays(1);

            await this._authorizationService.AuthorizeAsync(User, entity, Operations.TaskCreate);

            await this._unitOfWork.Tasks.Add(entity);
            await this._unitOfWork.Complete();

            var tasked = this._unitOfWork.Tasks
                .Find(u => u.Name == entity.Name
                                    && u.UserCreated == entity.UserCreated
                                    && u.UserId == entity.UserId
                                    && u.DateCreated == entity.DateCreated).FirstOrDefault();
            if (checkP != null)
                if (checkP.ChildTasks != null)
                {
                    checkP.ChildTasks.Add(tasked);
                }
                else
                {
                    checkP.ChildTasks = new List<Task>();
                    checkP.ChildTasks.Add(tasked);
                }
            await this._unitOfWork.Complete();
            return Ok(new JsonResult(_mapper.Map<CreatedTask>(tasked))
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
            var result = _mapper.Map<Core.Models.Task, TaskResources>(task);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            var task = await this._unitOfWork.Tasks.Get(id);
            if(task == null)
                throw new Exception("Invalid id");
            await this._authorizationService.AuthorizeAsync(User, task, Operations.TaskDelete);
            await _unitOfWork.Tasks.RemoveChild(task);
            _unitOfWork.Tasks.Remove(task);
            await this._unitOfWork.Complete();
            return Ok(new { message = "Delete task: " + task.Name + " success." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(Guid id, [FromBody] CreatedTask task) 
        {
            if (!ModelState.IsValid) throw new Exception("Invalid information.");
            var result = await this._unitOfWork.Tasks.Get(id);
            result.PhaseId = task.PhaseId;
            result.Name = task.Name;
            result.DueDate = task.DueDate;
            result.StartDate = task.StartDate;
            result.Percent = task.Percent;
            result.UserId = task.UserId;
            result.DateModified = DateTime.UtcNow.AddHours(7);
            result.ParentN = await this._unitOfWork.Tasks.Get(task.ParentNId ?? Guid.Empty);

            await this._authorizationService.AuthorizeAsync(User, result, Operations.TaskUpdate);

            //Update progress of report
            var phase = await this._unitOfWork.Phases.Get(result.PhaseId);
            var report = await this._unitOfWork.Reports.Get(phase.ReportId);
            int totalTask = 0;
            float totalPercent = 0;
            foreach (var phaseReport in report.Phases)
            {
                totalTask += phaseReport.Tasks.Count;
                totalPercent += phaseReport.Tasks.Sum(taskPhase => task.Percent);
            }

            report.Progress = (totalPercent / (totalTask == 0 ? 1 : totalTask));
            await this._unitOfWork.Complete();
            return Ok(new { message = "Update task " + task.Name + " success." });
        }
    }
}
