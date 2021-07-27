using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagerAPI.Core;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Resources;
using ProjectManagerAPI.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch.Operations;
using ProjectManagerAPI.Core.Policy;
using ProjectManagerAPI.Core.ServiceResource;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportController : Controller
    {
        private readonly ITokenManager _tokenParser;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorization;

        public ReportController(ITokenManager tokenParser, IUnitOfWork unitOfWork, IMapper mapper, IAuthorizationService authorization)
        {
            _tokenParser = tokenParser;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _authorization = authorization;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreatedReport report)
        {
            if (!ModelState.IsValid)
                throw new Exception("Provided information is invalid");

            //Get user claims from token
            var user = await _tokenParser.GetUserByToken();
            if (user.GroupRef == null || user.GroupRef == Guid.Empty)
                throw new Exception("Join a group first");
           
            var projectRp = await this._unitOfWork.Projects.Get(report.ProjectId);
            if (projectRp == null)
                throw new Exception("Invalid project id.");
            var groupRp = await this._unitOfWork.Groups.Get(report.GroupId);
            if (groupRp == null)
                throw new Exception("Invalid group id.");
            if (groupRp.LeaderId != user.Id)
                throw new Exception("You don't have permission.");
            var newRp = new Report
            {
                Name = report.Name,
                Remark = report.Remark,
                StartDate = report.StartDate,
                DueDate = report.DueDate,
                GroupId = report.GroupId,
                Group = groupRp,
                ProjectId = report.ProjectId,
                Project = projectRp,
                UserCreated = user.Id
            };
            if (report.StartDate > report.DueDate)
                throw new Exception("Start date can't not be larger than end date.");
            var auth = await this._authorization.AuthorizeAsync(User, newRp, Operations.ReportCreate);
            if (!auth.Succeeded)
                throw new Exception("You don't have permission");
            try
            {
                await this._unitOfWork.Reports.Add(newRp);
                await this._unitOfWork.Complete();
            }
            catch (Exception)
            {
                throw new Exception("This name is already taken.");
            }
            var result = _mapper.Map<CreatedReport>(newRp);
            return Ok(new JsonResult(result) { StatusCode = 200, ContentType = "application/json" });
        }

        [HttpGet]
        public async Task<IActionResult> GetValidated(Guid? projectId)
        {
            var user = await _tokenParser.GetUserByToken();
            if (user == null)
                throw new Exception("Credential information not provided.");
            await this._unitOfWork.Users.Load(u => u.Id == (user.ParentNId ?? Guid.Empty));
            if (projectId != null & projectId != Guid.Empty)
            {
                var project = await this._unitOfWork.Projects.Get(projectId.Value);
                if (project == null)
                    throw new Exception("Project ID invalid.");
            }
            else await this._unitOfWork.Projects.LoadValidated();

            var groups =
                await this._unitOfWork.Groups.GetGroupListValidated(user.ParentN?.Id ?? user.Id);

            List<Report> listRp = new List<Report>();
            var utils = new AuthorizeUtils(_unitOfWork);
            if (!await utils.IsLeader(user.UserName))
                groups = groups.Where(u => u.Id == user.GroupRef).ToList();
            foreach (var group in groups)
            {
                if (projectId == null | projectId == Guid.Empty)
                    listRp.AddRange(await this._unitOfWork.Reports.FindReportByGroupId(group.Id));
                else
                {
                    listRp.AddRange(await this._unitOfWork.Reports.FindReportByGroupIdAndProjectId(group.Id, projectId.Value));
                }
            }

            var result = this._mapper.Map<IEnumerable<ReportViewResource>>(listRp);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReport(Guid id)
        {
            var user = await _tokenParser.GetUserByToken();
            if (user == null)
                throw new Exception("Credential information not provided.");
            var report = await this._unitOfWork.Reports.Get(id);
            if (report == null)
                throw new Exception("Invalid Report ID.");
            var auth = await this._authorization.AuthorizeAsync(User, report, Operations.ReportRead);
            if (!auth.Succeeded)
                throw new Exception("You don't have permission");
            var result = _mapper.Map<ReportResource>(report);
            var group = await this._unitOfWork.Groups.Get(report.GroupId);
            var project = await this._unitOfWork.Projects.Get(report.ProjectId);
            result.ProjectName = project.Name;
            result.GroupName = group.Name;
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            var report = await this._unitOfWork.Reports.Get(id);
            if (report == null)
                throw new Exception("Invalid id");
            var auth = await this._authorization.AuthorizeAsync(User, report, Operations.ReportDelete);
            if (!auth.Succeeded)
                throw new Exception("You don't have permission");

            foreach (var phase in report.Phases.ToList())
            {
                foreach (var task in phase.Tasks.ToList())
                {
                    await this._unitOfWork.Tasks.RemoveChild(task);
                    this._unitOfWork.Tasks.Remove(task);
                }
                this._unitOfWork.Phases.Remove(phase);
            }
            _unitOfWork.Reports.Remove(report);
            await this._unitOfWork.Complete();
            return Ok(new {message = "Delete " + report.Name + "success."});
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(Guid id,[FromBody] ReportViewResource report)
        {
            if (ModelState.IsValid)
            {
                var result = await this._unitOfWork.Reports.Get(id);
                result.GroupId = report.GroupId;
                result.DueDate = report.DueDate;
                result.StartDate = report.StartDate;
                result.Progress = report.Progress;
                result.ProjectId = report.ProjectId;
                result.DateModified = DateTime.UtcNow;
                result.Remark = report.Remark;
                if (report.StartDate > report.DueDate)
                    throw new Exception("Start date can't not be larger than end date.");
                await this._authorization.AuthorizeAsync(User, result, Operations.ReportUpdate);
                await this._unitOfWork.Complete();
                return Ok(new { message = "Update " + report.Name + "success." });
            }

            throw new Exception("Invalid information.");
        }
    }
}
