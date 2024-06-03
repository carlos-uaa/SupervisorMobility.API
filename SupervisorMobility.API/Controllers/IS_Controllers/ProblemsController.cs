using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Services.TreeServices;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Services;
using SupervisorMobility.API.DataAccess.Entities.IS;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.ProblemDefectDtos;

namespace SupervisorMobility.API.Controllers.IS_Controllers
{
    [Route("api/IS/Aparence/Problems")]
    [ApiController]
    public class ProblemsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IStampingRepository _stampingRepository;
        private readonly IWebHostEnvironment _env;
        public ProblemsController(IStampingRepository stampingRepository, IWebHostEnvironment env, IMapper mapper)
        {
            _stampingRepository = stampingRepository ??
                throw new ArgumentNullException(nameof(stampingRepository));
            _mapper = mapper ??
                  throw new ArgumentNullException(nameof(mapper));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        [HttpPost]
        public async Task<ActionResult<ProblemDefectDto>> CreateProblemDefect(ProblemDefectForCreateDto ProblemDefectForCreate)
        {
            ProblemDefect ProblemDefectEntity = _mapper.Map<ProblemDefect>(ProblemDefectForCreate);
            ProblemDefectEntity.ItemOrder = await _stampingRepository.ProblemDefectMaxItemOrderAsync();

            var createdResult = await _stampingRepository.AddProblemDefect(ProblemDefectEntity);
            if (createdResult != null)
                return Ok(ProblemDefectEntity);
            else
                return BadRequest(); ;

        }

        [HttpGet("{ProblemDefect_id}", Name = "GetProblemDefect")]
        public async Task<ActionResult<ProblemDefectDto>> GetProblemDefect(int ProblemDefect_id)
        {

            var ProblemDefectEntity = await _stampingRepository.GetProblemDefect(ProblemDefect_id);
            if (ProblemDefectEntity == null)
            {
                return NotFound("ProblemDefect not found!");
            }

            return Ok(_mapper.Map<ProblemDefectDto>(ProblemDefectEntity));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProblemDefectDto>>> GetAllProblemDefects()
        {

            var ProblemDefectEntity = await _stampingRepository.GetAllProblemDefects();
            if (ProblemDefectEntity == null)
            {
                return NotFound("ProblemDefect not found!");
            }

            return Ok(_mapper.Map<IEnumerable<ProblemDefectDto>>(ProblemDefectEntity));
        }

        [HttpPut("{ProblemDefect_id}")]
        public async Task<ActionResult<ProblemDefectDto>> UpdateProblemDefect(int ProblemDefect_id, ProblemDefectForUpdateDto _ProblemDefectForUpdate)
        {

            var ProblemDefectEntity = await _stampingRepository.GetProblemDefect(ProblemDefect_id);
            if (ProblemDefectEntity == null)
            {
                return NotFound("ProblemDefect not found!");
            }

            var result = await _stampingRepository.UpdateProblemDefect(_ProblemDefectForUpdate, ProblemDefectEntity);

            if (result > 0)
                return Ok(ProblemDefectEntity);
            else
                return BadRequest();
        }

        [HttpDelete("{ProblemDefect_id}")]
        public async Task<ActionResult> DeleteProblemDefect(int ProblemDefect_id)
        {
            ProblemDefect? entityDataPanel = await _stampingRepository.GetProblemDefect(ProblemDefect_id);

            var result = await _stampingRepository.DeleteProblemDefect(entityDataPanel);

            if (result > 0)
                return Ok();
            else
                return BadRequest();
        }


    }
}
