using Microsoft.AspNetCore.Mvc;
using ProjectManagerAPI.Core;
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
        public async Task<IActionResult> GetAll(string postId)
        {
            var projects = await _unitOfWork.Projects.GetAll();
            return Ok(projects);
        }


    }
}
