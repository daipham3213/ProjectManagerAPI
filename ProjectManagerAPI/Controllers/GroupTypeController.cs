using Microsoft.AspNetCore.Mvc;
using ProjectManagerAPI.Core;
using ProjectManagerAPI.Core.Repositories;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupTypeController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        public GroupTypeController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var type = await _unitOfWork.GroupType.GetAll();
            return Ok(type);
        }
    }
}
