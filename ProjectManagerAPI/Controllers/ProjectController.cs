using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagerAPI.Core;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Repositories;
using ProjectManagerAPI.Core.Resources;
using ProjectManagerAPI.Core.ServiceResource;
using ProjectManagerAPI.Core.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace ProjectManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly ITokenParser _tokenParser;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ProjectController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }


        [HttpGet("all")]
        [HttpGet]
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
            if (type == null)
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
        [HttpGet("{id}")]
        [Authorize(Roles = RoleNames.RoleAdmin)]
        public async Task<IActionResult> DeleteProject(Guid idPro)
        {
            var project = await this._unitOfWork.Projects.SingleOrDefault(c => c.ID == idPro);
            if (project == null)
                return BadRequest();

            this._unitOfWork.Projects.Remove(project);
            await this._unitOfWork.Complete();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> create([FromBody] CreateProject project) {
            if (!ModelState.IsValid)
                return new JsonResult("The information provided for creation is not valid")
                {
                    StatusCode = BadRequest().StatusCode,
                };
            string token = await HttpContext.GetTokenAsync("access_token");
            var user = await _tokenParser.GetUserByToken(token);

            var entity = new Project
            {
                Name = project.Name,
                Remark = project.Remark,
                //DueDate 
                StartDate = project.StartDate,
                DueDate = project.DueDate,
            };

            try
            {
                await this._unitOfWork.Projects.Add(entity);
                await this._unitOfWork.Complete();


                entity = await this._unitOfWork.Projects.SearcProjectByName(entity.Name);
             
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
