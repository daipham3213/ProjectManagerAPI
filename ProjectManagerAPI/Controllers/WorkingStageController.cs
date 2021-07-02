using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagerAPI.Core;
using ProjectManagerAPI.Core.Resources;
using ProjectManagerAPI.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WorkingStageController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenManager _tokenParser;
        private readonly IMapper _mapper;

        public WorkingStageController(IUnitOfWork unitOfWork, ITokenManager tokenParser, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _tokenParser = tokenParser;
            _mapper = mapper;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetPhase()
        {
            var result = await _unitOfWork.Phases.GetAll();
            return Ok(_mapper.Map<IEnumerable<PhaseViewResource>>(result));
        }

    }
}
