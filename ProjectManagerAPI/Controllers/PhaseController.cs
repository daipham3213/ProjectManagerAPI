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

namespace ProjectManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PhaseController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenManager _tokenParser;
        private readonly IMapper _mapper;

        public PhaseController(IUnitOfWork unitOfWork, ITokenManager tokenParser, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _tokenParser = tokenParser;
            _mapper = mapper;
        }

        [HttpGet("all")]
       // [Authorize(Roles = RoleNames.RoleAdmin)]
        public async Task<IActionResult> GetPhase()
        {
            var result = await _unitOfWork.Phases.GetAll();
            return Ok(_mapper.Map<IEnumerable<PhaseViewResource>>(result));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreatedPhase WorkingStage)
        {
            if (!ModelState.IsValid)
                throw new Exception("Provided information is invalid");

            var user = await _tokenParser.GetUserByToken();

            var report = await _unitOfWork.Reports.Get(WorkingStage.ReportID);
            if (report == null) {
                throw new Exception("Invalid Report");
            }
            var entity = new Phase
            {
                Name = WorkingStage.Name,
                Remark = WorkingStage.Remark,
                UserCreated = user.Id,
                StartDate = WorkingStage.StartDate,
                DueDate = WorkingStage.DueDate,

                ReportId = WorkingStage.ReportID,
                Report = report,
            };

            await _unitOfWork.Phases.Add(entity);
            await _unitOfWork.Complete();

            entity = await this._unitOfWork.Phases.SearchPhaneByName(entity.Name);
            await _unitOfWork.Complete();
            return Ok(new JsonResult(_mapper.Map<CreatedPhase>(entity)) {
                StatusCode = Ok().StatusCode
            });
        }




        [HttpDelete()]
        public async Task<IActionResult> RemovePhase(Guid id) {
            var phase = await this._unitOfWork.Phases.SingleOrDefault( c => c.Id == id);
            if (phase == null) {
                throw new Exception("Phase id is invalid");
            }
            this._unitOfWork.Phases.Remove(phase);
            await _unitOfWork.Complete();
            return Ok(new JsonResult(phase.Name + "removed successfully")
            {
                StatusCode = Ok().StatusCode
            });
        }



        }
}
