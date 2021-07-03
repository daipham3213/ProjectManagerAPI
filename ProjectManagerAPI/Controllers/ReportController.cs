using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using ProjectManagerAPI.Core;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Resources;
using ProjectManagerAPI.Core.Services;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportController : Controller
    {
        public ReportController(ITokenManager tokenParser, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _tokenParser = tokenParser;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        private readonly ITokenManager _tokenParser;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreatedReport report)
        {
            if (!ModelState.IsValid)
                throw new Exception("Provided information is invalid");

            //Get user claims from token
            var user = await _tokenParser.GetUserByToken();
            var projectRp = await this._unitOfWork.Projects.Get(report.ProjectId);
            if (projectRp == null)
                throw new Exception("Invalid project id.");
            var groupRp = await this._unitOfWork.Groups.Get(report.GroupId);
            if (groupRp == null)
                throw new Exception("Invalid group id.");

            if (user.Id != groupRp.LeaderId)
                throw new Exception("Permission not allowed.");

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
            try
            {
                await this._unitOfWork.Reports.Add(newRp);
                await this._unitOfWork.Complete();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
            {
                throw new Exception("This name is already taken.");
            }
            var result = _mapper.Map<CreatedReport>(newRp);
            return Ok(new JsonResult(result) { StatusCode = 200, ContentType = "application/json"});
        }

        [HttpGet]
        public async Task<IActionResult> GetValidated(Guid? projectId)
        {
            var user = await _tokenParser.GetUserByToken();
            IEnumerable<Group> groups;
            if (user == null)
                throw new Exception("Credential information not provided.");
            if (user.ParentN != null)
                groups = await this._unitOfWork.Groups.GetGroupListValidated(user.ParentN.Id);
            else groups = await this._unitOfWork.Groups.GetGroupListValidated(user.Id);
            if (!groups.Any())
                throw new Exception("No group information.");
            if (projectId != null & projectId != Guid.Empty)
            {
                var project = await this._unitOfWork.Projects.Get(projectId.Value);
                if (project == null)
                    throw new Exception("Project ID invalid.");
            }
            else await this._unitOfWork.Projects.LoadValidated();
            List<Report> listRp = new List<Report>();
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
            return Ok(new JsonResult(result){StatusCode = 200, ContentType = "application/json"});
        }
    }
}
