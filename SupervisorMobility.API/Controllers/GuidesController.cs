using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.DistributionDtos;
using SupervisorMobility.API.Models.GuidesDtos;
using SupervisorMobility.API.Models.PlantDtos;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Controllers
{


    [Route("api/guides")]
    [ApiController]

    public class GuidesController : ControllerBase
    {

        private readonly IMapper _mapper;
        private readonly IAssyChartService _assyChartService;
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;

        public GuidesController(ISupervisorMobilityRepository supervisorMobilityRepository,
            IMapper mapper, IAssyChartService assyChartService)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _assyChartService = assyChartService;
        }

        [HttpPost]
        public async Task<ActionResult<GuideWithoutFileDto>> CreateGuide(GuideForCreationDto guide)
        {

            var finalguide = await _assyChartService.CreateGuideAsync(guide);

            var createGuideToReturn = _mapper.Map<GuideWithoutFileDto>(finalguide);

            return CreatedAtRoute("GetGuide",
                 new
                 {
                     guideId = finalguide.GuideId
                 },
                 createGuideToReturn);
        }

        [HttpGet("{guideId}", Name = "GetGuide")]
        public async Task<IActionResult> GetGuide(int guideId, bool includeFile = false)
        {
            if (includeFile)
            {
                var guide = await _assyChartService.FetchGuideAsync(guideId, includeFile);
                if (guide == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<GuideWithFileInfoDto>(guide));
            }
            else
            {
                var guide = await _assyChartService.FetchGuideAsync(guideId);
                if (guide == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<GuideWithoutFileDto>(guide));
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GuideWithFileInfoDto>>> GetAllGuides(bool includeFiles = false)
        {

            if (includeFiles)
            {
                var allguides = await _supervisorMobilityRepository.GetAllGuides(includeFiles);
                return Ok(_mapper.Map<IEnumerable<GuideWithFileInfoDto>>(allguides));

            }
            else
            {
                var allguides = await _supervisorMobilityRepository.GetAllGuides();
                return Ok(_mapper.Map<IEnumerable<GuideWithoutFileDto>>(allguides));

            }

        }

        
        [HttpDelete("{lupId}")]
        public async Task<ActionResult> DeleteLup(int lupId)
        {
            var lup = await _supervisorMobilityRepository.GetLupAsync(lupId);

            if (lup == null)
            {
                return NotFound();
            }

            _supervisorMobilityRepository.DeleteLup(lup);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();
        }


    }
}
