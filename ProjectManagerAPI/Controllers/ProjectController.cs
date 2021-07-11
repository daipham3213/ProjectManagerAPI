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
            if (type.Count() == 0)
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

            var result = _mapper.Map<Project, ProjectResource>(project);
            return Ok(result);
        }



        [HttpDelete]
        [Authorize(Roles = RoleNames.RoleAdmin)]
        public async Task<IActionResult> DeleteProject(Guid idPro)
        {
            var project = await this._unitOfWork.Projects.SingleOrDefault(c => c.Id == idPro);
            if (project == null)
                return BadRequest();

            await this._authorizationService.AuthorizeAsync(User, project, Operations.ProjectDelete);
            //RemoveAllChildren with Bug
            this._unitOfWork.Projects.RemoveAllChildren(idPro);
          
            this._unitOfWork.Projects.Remove(project);
            await this._unitOfWork.Complete();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProject project)
        {
            if (!ModelState.IsValid)
                throw new Exception("The information provided for creation is not valid");
            var user = await _tokenParser.GetUserByToken();

            var entity = new Project
            {
                Name = project.Name,
                Remark = project.Remark,
                StartDate = project.StartDate,
                DueDate = project.DueDate,
            };

            try
            {
          
                await this._authorizationService.AuthorizeAsync(User, entity, Operations.ProjectCreate);

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


    }
}
