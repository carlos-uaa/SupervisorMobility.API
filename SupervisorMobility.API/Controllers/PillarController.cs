using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Models.PillarDtos;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/pillars")]
    [ApiController]
    public class PillarController : ControllerBase
    {
        readonly ISupervisorMobilityRepository _supervisorMobilityRepository;
        readonly IMapper _mapper;
        public PillarController(ISupervisorMobilityRepository supervisorMobilityRepository,
            IMapper mapper)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PillarDto>>> GetPillars()
        {
            var pillarEntities = await _supervisorMobilityRepository.GetPillarsAsync();
            return Ok(_mapper.Map<IEnumerable<PillarDto>>(pillarEntities));
        }

        [HttpGet("{pillarId}", Name = "GetPillar")]
        public async Task<ActionResult> GetPillar(int pillarId)
        {
            //Find Job Observation type
            var pillar = await _supervisorMobilityRepository
                .GetPillarAsync(pillarId);
            if (pillar == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PillarDto>(pillar));
        }

        [HttpPost]
        public async Task<ActionResult<PillarDto>> CreatePillar(
            PillarForCreationDto pillar)
        {
            //Mpa the pbject
            var finalPillar = _mapper.Map<Entities.Pillar>(pillar);
            _supervisorMobilityRepository.AddPillar(finalPillar);
            await _supervisorMobilityRepository.SaveChangesAsync();

            var createPillarToReturn =
                _mapper.Map<PillarDto>(finalPillar);

            return CreatedAtRoute("GetPillar",
                new
                {
                    pillarId = createPillarToReturn.PillarId
                },
                createPillarToReturn);
        }


        [HttpPut("{pillarId}")]
        public async Task<ActionResult> UpdatePillar(int pillarId,
            PillarForUpdateDto pillar)
        {
            var pillarEntity = await _supervisorMobilityRepository.GetPillarAsync(pillarId);
            if (pillarEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(pillar, pillarEntity);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();

        }

        [HttpDelete("{pillarId}")]
        public async Task<ActionResult> DeletePillar(int pillarId)
        {
            var pillarEntity = await _supervisorMobilityRepository.GetPillarAsync(pillarId);
            if (pillarEntity == null)
            {
                return NotFound();
            }

            _supervisorMobilityRepository.DeletePillar(pillarEntity);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();
        }
    }
}
