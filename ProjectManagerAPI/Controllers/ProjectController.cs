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
    public class ProjectController : ControllerBase
    {

        private readonly ITokenManager _tokenParser;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthorizationService _authorizationService;

        public ProjectController(ITokenManager tokenParser, IMapper mapper, IUnitOfWork unitOfWork, IAuthorizationService authorizationService)
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
            var projects = await _unitOfWork.Projects.GetAll();
            return Ok(projects);
        }


        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var type = await _unitOfWork.Projects.LoadValidated();
            if (!type.Any())
            {
                return NotFound();
            }
            return Ok(_mapper.Map<IEnumerable<ProjectViewResource>>(type));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPJ(Guid id)
        {
            await this._unitOfWork.Projects.GetAll();
            var project = await _unitOfWork.Projects.Get(id);
            if (project == null)
            {
                return NotFound();
            }
            var auth = await this._authorizationService.AuthorizeAsync(User, project, Operations.ProjectRead);
            if (!auth.Succeeded)
                throw new Exception("You don't have permission.");
            var result = _mapper.Map<Project, ProjectResource>(project);
            return Ok(result);
        }



        [HttpDelete]
        public async Task<IActionResult> DeleteProject(Guid idPro)
        {
            var project = await this._unitOfWork.Projects.SingleOrDefault(c => c.Id == idPro);
            if (project == null)
                return BadRequest();
            var auth = await this._authorizationService.AuthorizeAsync(User, project, Operations.ProjectDelete);
            if (!auth.Succeeded)
                throw new Exception("You don't have permission.");
            await this._unitOfWork.Reports.Load(u => u.ProjectId == idPro);
            foreach (var report in project.Reports)   
            {
                foreach (var phase in report.Phases)
                {
                    foreach (var task in phase.Tasks)
                    {
                        await this._unitOfWork.Tasks.RemoveChild(task);
                        this._unitOfWork.Tasks.Remove(task);
                    }
                    this._unitOfWork.Phases.Remove(phase);
                }
                this._unitOfWork.Reports.Remove(report);
            }

            this._unitOfWork.Projects.Remove(project);
            await this._unitOfWork.Complete();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProject project)
        {
            if (!ModelState.IsValid)
                throw new Exception("The information provided for creation is not valid.");
            var user = await _tokenParser.GetUserByToken();

            var entity = new Project
            {
                Name = project.Name,
                Remark = project.Remark,
                StartDate = project.StartDate,
                DueDate = project.DueDate,
                UserCreated = user.Id,
            };
            if (project.StartDate > project.DueDate)
                throw new Exception("Start date can't not be larger than end date.");
            try
            {
                var auth = await this._authorizationService.AuthorizeAsync(User, entity, Operations.ProjectCreate);
                if (!auth.Succeeded)
                    throw new Exception("You don't have permission.");

                await this._unitOfWork.Projects.Add(entity);

                entity = await this._unitOfWork.Projects.SearchProjectByName(entity.Name);
                await this._unitOfWork.Complete();

                var result = this._mapper.Map<Project, CreateProject>(entity);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(Guid id, [FromBody] CreateProject project)
        {
            if (!ModelState.IsValid) throw new Exception("Invalid information");
            var result = await this._unitOfWork.Projects.Get(id);

            var auth = await this._authorizationService.AuthorizeAsync(User, result, Operations.ProjectUpdate);
            if (!auth.Succeeded)
                throw new Exception("You don't have permission.");

            if (project.StartDate > project.DueDate)
                throw new Exception("Start date can't not be larger than end date.");
            var task = await this._unitOfWork.Projects.UpdateProject(id, project);
            if (task)
                return Ok(new { message = "Update " + project.Name + " success." });
            throw new Exception("Forbidden");
        }

    }
}
