using Microsoft.AspNetCore.Mvc;
using ProjectManagerAPI.Core;
using ProjectManagerAPI.Core.Repositories;
using System;
using System.Threading.Tasks;


namespace ProjectManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProjectController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var projects = await _unitOfWork.Projects.GetAll();
            return Ok(projects);
        }

        [HttpGet]
        public async Task<IActionResult> GetProjectID(Guid idPro)
        {
            var projects = await _unitOfWork.Project.Get(idPro);
            return Ok(projects);
        }


    }
}
