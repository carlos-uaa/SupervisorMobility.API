using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Entities.IS;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.AppearanceDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.LogbookAppearanceDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.PartDtos;

namespace SupervisorMobility.API.Controllers.IS_Controllers
{
    [Route("api/IS/Appearance/Logbook")]
    [ApiController]
    public class AppearanceLogbookController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IStampingRepository _stampingRepository;
        private readonly IWebHostEnvironment _env;
        public AppearanceLogbookController(IStampingRepository stampingRepository, IWebHostEnvironment env, IMapper mapper)
        {
            _stampingRepository = stampingRepository ??
                throw new ArgumentNullException(nameof(stampingRepository));
            _mapper = mapper ??
                  throw new ArgumentNullException(nameof(mapper));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        [HttpPost]
        public async Task<ActionResult<DataPanelDto>> CreateLogbookAppearance(LogbookAppearanceForCreateDto LogbookAppearanceForCreate)
        {
            LogbookAppearance LogbookAppearanceEntity = _mapper.Map<LogbookAppearance>(LogbookAppearanceForCreate);

            var createdResult = await _stampingRepository.AddLogbookAppearance(LogbookAppearanceEntity);
            if (createdResult != null)
                return Ok(LogbookAppearanceEntity);
            else
                return BadRequest(); ;

        }

        [HttpGet("{logbookAppearance_id}", Name = "GetLogbookAppearance")]
        public async Task<ActionResult<LogbookAppearanceDto>> GetLogbookAppearance(int logbookAppearance_id, bool includePanelResults = false, bool includeProblemDefectResults = false)
        {

            var logbookAppearanceEntity = await _stampingRepository.GetLogbookAppearance(logbookAppearance_id, includePanelResults, includeProblemDefectResults);
            if (logbookAppearanceEntity == null)
            {
                return NotFound("LogbookAppearance not found!");
            }

            return Ok(_mapper.Map<LogbookAppearanceDto>(logbookAppearanceEntity));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LogbookAppearanceDto>>> GetAllLogbookAppearances(bool includePanelResults = false, bool includeProblemDefectResults = false)
        {

            var logbookAppearanceEntity = await _stampingRepository.GetAllLogbookAppearances(includePanelResults, includeProblemDefectResults);
            if (logbookAppearanceEntity == null)
            {
                return NotFound("LogbookAppearance not found!");
            }

            return Ok(_mapper.Map<IEnumerable<LogbookAppearanceDto>>(logbookAppearanceEntity));
        }

        [HttpPut("{logbookAppearance_id}")]
        public async Task<ActionResult<LogbookAppearanceDto>> UpdateLogbookAppearance(int logbookAppearance_id, LogbookAppearanceForUpdateDto _LogbookAppearanceForUpdate)
        {

            var logbookAppearanceEntity = await _stampingRepository.GetLogbookAppearance(logbookAppearance_id, true);
            if (logbookAppearanceEntity == null)
            {
                return NotFound("LogbookAppearance not found!");
            }

            var result = await _stampingRepository.UpdateLogbookAppearance(_LogbookAppearanceForUpdate, logbookAppearanceEntity);

            if (result > 0)
                return Ok(logbookAppearanceEntity);
            else
                return BadRequest();
        }

        [HttpDelete("{logbookAppearance_id}")]
        public async Task<ActionResult> DeleteLogbookAppearance(int logbookAppearance_id)
        {
            LogbookAppearance? entityDataPanel = await _stampingRepository.GetLogbookAppearance(logbookAppearance_id);

            var result = await _stampingRepository.DeleteLogbookAppearance(entityDataPanel);

            if (result > 0)
                return Ok();
            else
                return BadRequest();
        }
    }
}
