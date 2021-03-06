using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagerAPI.Core;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Policy;
using ProjectManagerAPI.Core.Resources;
using ProjectManagerAPI.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectManagerAPI.StaticValue;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PhaseController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenManager _tokenParser;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;


        public PhaseController(IUnitOfWork unitOfWork, ITokenManager tokenParser, IMapper mapper, IAuthorizationService authorizationService)
        {
            _unitOfWork = unitOfWork;
            _tokenParser = tokenParser;
            _mapper = mapper;
            _authorizationService = authorizationService;
        }

        [HttpGet("all")]
        [Authorize(Roles = RoleNames.RoleAdmin)]
        public async Task<IActionResult> GetPhase()
        {
            var result = await _unitOfWork.Phases.GetAll();
            return Ok(_mapper.Map<IEnumerable<PhaseViewResource>>(result));
        }

        [HttpGet]
        public async Task<IActionResult> GetValidated(Guid? reportId)
        {
            if (reportId == null)
            {
                var user = await this._tokenParser.GetUserByToken();
                var reports = await this._unitOfWork.Reports.FindReportByGroupId(user.GroupRef ?? Guid.Empty);
                List<Phase> phases = new List<Phase>();
                foreach (var report in reports)
                {
                    var temp = this._unitOfWork.Phases.Find(u => u.ReportId == report.Id);
                    phases.AddRange(temp);
                }

                return Ok(_mapper.Map<IEnumerable<PhaseViewResource>>(phases));
            }

            var result = this._unitOfWork.Phases.Find(u => reportId.GetValueOrDefault() == u.ReportId);
            return Ok(_mapper.Map<IEnumerable<PhaseViewResource>>(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPhase(Guid id)
        {
            await this._unitOfWork.Phases.GetAll();
            var phase = await _unitOfWork.Phases.Get(id);
            if (phase == null)
            {
                throw new Exception("Provided information is invalid");
            }

            var result = _mapper.Map<Phase, PhaseResource>(phase);
            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreatedPhase workingStage)
        {
            if (!ModelState.IsValid)
                throw new Exception("Provided information is invalid");

            var user = await _tokenParser.GetUserByToken();

            var report = await _unitOfWork.Reports.Get(workingStage.ReportID);
            if (report == null)
            {
                throw new Exception("Invalid Report");
            }

            if (report.DueDate < workingStage.DueDate)
                throw new Exception("Phase end date is larger than " + report.DueDate);
            if (report.StartDate > workingStage.StartDate)
                throw new Exception("Phase start date is smaller than " + report.StartDate);
            if(workingStage.DueDate < workingStage.StartDate)
                throw new Exception("Phase end date is smaller than start date");

            var entity = new Phase
            {
                Name = workingStage.Name,
                Remark = workingStage.Remark,
                UserCreated = user.Id,
                StartDate = workingStage.StartDate,
                DueDate = workingStage.DueDate,
                ReportId = workingStage.ReportID,
                Report = report,
            };

            if (entity.StartDate == entity.DueDate) entity.DueDate.Value.AddDays(1);
            //validation
            var auth = await this._authorizationService.AuthorizeAsync(User, entity, Operations.PhaseCreate);
            if (!auth.Succeeded)
                throw new Exception("You don't have permission");

            await _unitOfWork.Phases.Add(entity);

            entity = await this._unitOfWork.Phases.SearchPhaseByName(entity.Name);
            await _unitOfWork.Complete();
            return Ok(new JsonResult(_mapper.Map<CreatedPhase>(entity))
            {
                StatusCode = Ok().StatusCode
            });
        }



        [HttpDelete()]
        public async Task<IActionResult> RemovePhase(Guid id)
        {
            var phase = await this._unitOfWork.Phases.SingleOrDefault(c => c.Id == id);
            if (phase == null)
            {
                throw new Exception("Phase id is invalid");
            }
            //validation
            var auth = await this._authorizationService.AuthorizeAsync(User, phase, Operations.PhaseDelete);
            if (!auth.Succeeded)
                throw new Exception("You don't have permission");
            foreach (var task in phase.Tasks.ToList())
            {
                await this._unitOfWork.Tasks.RemoveChild(task);
                this._unitOfWork.Tasks.Remove(task);
            }
            this._unitOfWork.Phases.Remove(phase);
            await this._unitOfWork.Tasks.UpdateProgress(id);
            await _unitOfWork.Complete();
            return Ok(new JsonResult(phase.Name + "removed successfully")
            {
                StatusCode = Ok().StatusCode
            });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(Guid id, [FromBody] CreatedPhase phase)
        {
            if (!ModelState.IsValid) throw new Exception("Invalid information.");
            var result = await this._unitOfWork.Phases.Get(id);
            result.ReportId = phase.ReportID;
            result.DueDate = phase.DueDate;
            result.StartDate = phase.StartDate;
            result.Remark = phase.Remark;
            if (phase.DueDate < phase.StartDate)
                throw new Exception("Phase end date is smaller than start date");

            var auth = await this._authorizationService.AuthorizeAsync(User, result, Operations.PhaseUpdate);
            if (!auth.Succeeded)
                throw new Exception("You don't have permission");

            await this._unitOfWork.Complete();
            return Ok(new { message = "Update " + phase.Name + " success." });
        }

    }
}
