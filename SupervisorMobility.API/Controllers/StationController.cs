using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Models.StationDtos;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/station")]
    [ApiController]
    public class StationController : ControllerBase
    {
        readonly ISupervisorMobilityRepository _supervisorMobilityRepository;
        readonly IMapper _mapper;
        public StationController(ISupervisorMobilityRepository supervisorMobilityRepository,
            IMapper mapper)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StationDto>>> GetStations()
        {
            var StationEntity = await _supervisorMobilityRepository.GetStationsAsync();
            return Ok(_mapper.Map<IEnumerable<StationDto>>(StationEntity));
        }

        [HttpGet("{StationId}", Name = "GetStation")]
        public async Task<ActionResult> GetStation(int StationId)
        {
            //Find Job Observation type
            var Station = await _supervisorMobilityRepository
                .GetStationAsync(StationId);
            if (Station == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<StationDto>(Station));
        }

        [HttpPost]
        public async Task<ActionResult<StationDto>> CreateStation(
            StationForCreationDto Station)
        {
            //Mpa the pbject
            var finalStation = _mapper.Map<Entities.Station>(Station);
            _supervisorMobilityRepository.AddStation(finalStation);
            await _supervisorMobilityRepository.SaveChangesAsync();

            var createStationToReturn =
                _mapper.Map<StationDto>(finalStation);

            return CreatedAtRoute("GetStation",
                new
                {
                    StationId = createStationToReturn.StationId
                },
                createStationToReturn);
        }


        [HttpPut("{StationID}")]
        public async Task<ActionResult> UpdateStation(int StationId,
            StationForUpdateDto deparment)
        {
            var StationEntity = await _supervisorMobilityRepository.GetStationAsync(StationId);
            if (StationEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(deparment, StationEntity);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();

        }


        [HttpDelete("{StationId}")]
        public async Task<ActionResult> DeleteStation(int StationId)
        {
            var StationEntity = await _supervisorMobilityRepository.GetStationAsync(StationId);
            if (StationEntity == null)
            {
                return NotFound();
            }

            _supervisorMobilityRepository.DeleteStation(StationEntity);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();
        }
    }
}
