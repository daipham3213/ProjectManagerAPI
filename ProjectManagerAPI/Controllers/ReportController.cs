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

namespace ProjectManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportController : Controller
    {
        public ReportController(ITokenParser tokenParser, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _tokenParser = tokenParser;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        private readonly ITokenParser _tokenParser;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreatedReport report)
        {
            if (!ModelState.IsValid)
                return new JsonResult("Provided information is invalid")
                {
                    StatusCode = BadRequest().StatusCode,
                };

            //Get token from client
            string token = await HttpContext.GetTokenAsync("access_token");
            //Get user claims from token
            var user = await _tokenParser.GetUserByToken(token);
            var projectRp = await this._unitOfWork.Projects.Get(report.ProjectId);
            if (projectRp == null)
                return BadRequest(new JsonResult("Invalid project id.") { StatusCode = 400 });
            var groupRp = await this._unitOfWork.Groups.Get(report.GroupId);
            if (groupRp == null)
                return BadRequest(new JsonResult("Invalid group id.") { StatusCode = 400 });

            if ((user.ParentN.Id != groupRp.LeaderId & user.ParentN != null)|
                (user.ParentN == null & user.Id == groupRp.LeaderId))
                return BadRequest(new JsonResult("Permission not allowed."){StatusCode = 400});
            var newRp = new Report
            {
                Name = report.Name,
                Remark = report.Remark,
                StartDate = report.StartDate,
                DueDate = report.DueDate,
                GroupId = report.GroupId,
                Group = groupRp,
                ProjectId = report.ProjectId,
                Project = projectRp
            };
            await this._unitOfWork.Reports.Add(newRp);
            await this._unitOfWork.Complete();
            return Ok(new JsonResult(newRp) { StatusCode = 200 });
        }
    }
}
