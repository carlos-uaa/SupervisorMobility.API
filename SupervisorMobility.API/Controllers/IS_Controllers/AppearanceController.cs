using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Entities.IS;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.AppearanceDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelDtos;

namespace SupervisorMobility.API.Controllers.IS_Controllers
{
    [Route("api/IS/Appearance")]
    [ApiController]
    public class AppearanceController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IStampingRepository _stampingRepository;
        private readonly IWebHostEnvironment _env;
        public AppearanceController(IStampingRepository stampingRepository, IWebHostEnvironment env, IMapper mapper)
        {
            _stampingRepository = stampingRepository ??
                throw new ArgumentNullException(nameof(stampingRepository));
            _mapper = mapper ??
                  throw new ArgumentNullException(nameof(mapper));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        [HttpPost]
        public async Task<ActionResult<DataPanelDto>> CreateAppearance(AppearanceForCreateDto AppearanceForCreate)
        {
            Appearance AppearanceEntity = _mapper.Map<Appearance>(AppearanceForCreate);

            var createdResult = await _stampingRepository.AddAppearance(AppearanceEntity);
            if (createdResult != null)
                return Ok(AppearanceEntity);
            else
                return BadRequest(); ;

        }

        [HttpGet("{appearance_id}", Name = "GetAppearance")]
        public async Task<ActionResult<AppearanceDto>> GetAppearance(int appearance_id, bool includeDataPanelItems = false, bool includeProblemDefectItems = false, bool includeLogBookAppearance = false)
        {

            var appearanceEntity = await _stampingRepository.GetAppearance(appearance_id, includeDataPanelItems, includeProblemDefectItems, includeLogBookAppearance);
            if (appearanceEntity == null)
            {
                return NotFound("Appearance not found!");
            }

            return Ok(_mapper.Map<AppearanceDto>(appearanceEntity));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppearanceDto>>> GetAllAppearances(bool includeDataPanelItems = false, bool includeProblemDefectItems = false, bool includeLogBookAppearance = false)
        {

            var appearanceEntity = await _stampingRepository.GetAllAppearances(includeDataPanelItems, includeProblemDefectItems, includeLogBookAppearance);
            if (appearanceEntity == null)
            {
                return NotFound("Appearance not found!");
            }

            return Ok(_mapper.Map<IEnumerable<AppearanceDto>>(appearanceEntity));
        }

        [HttpPut("{appearance_id}")]
        public async Task<ActionResult<AppearanceDto>> UpdateAppearance(int appearance_id, AppearanceForUpdateDto _AppearanceForUpdate)
        {

            var appearanceEntity = await _stampingRepository.GetAppearance(appearance_id, true);
            if (appearanceEntity == null)
            {
                return NotFound("Appearance not found!");
            }

            var result = await _stampingRepository.UpdateAppearance(_AppearanceForUpdate, appearanceEntity);

            if (result > 0)
                return Ok(appearanceEntity);
            else
                return BadRequest();
        }

        [HttpDelete("{appearance_id}")]
        public async Task<ActionResult> DeleteAppearance(int appearance_id)
        {
            Appearance? entityDataPanel = await _stampingRepository.GetAppearance(appearance_id);

            var result = await _stampingRepository.DeleteAppearance(entityDataPanel);

            if (result > 0)
                return Ok();
            else
                return BadRequest();
        }


    }
}
