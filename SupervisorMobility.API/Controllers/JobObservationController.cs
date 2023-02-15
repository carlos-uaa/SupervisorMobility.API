using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.AssyChart;
using SupervisorMobility.API.Models.JobObservationDtos;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Models.PlantDtos;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/jobobservations")]
    [ApiController]
    public class JobObservationController : ControllerBase
    {
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;
        private readonly IMapper _mapper;

        public JobObservationController(ISupervisorMobilityRepository supervisorMobilityRepository, IMapper mapper)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost]
        public async Task<ActionResult<JobObservationWithoutNavigationPropertiesDto>> CreateJobObservation(
            JobObservationForCreationDto jobObservation)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(jobObservation.PlantId))
            {
                return NotFound();
            }

            if (!await _supervisorMobilityRepository.AreaExistAsync(jobObservation.AreaId))
            {
                return NotFound("No Area");
            }

            if (!await _supervisorMobilityRepository.DistributionExistsAsync(jobObservation.DistributionId))
            {
                return NotFound("No Distribution");
            }

            if (!await _supervisorMobilityRepository.OperationExistsAsync(jobObservation.OperationId))
            {
                return NotFound("No Operation");
            }

            var finalJobObservation = _mapper.Map<JobObservation>(jobObservation);

            _supervisorMobilityRepository.AddJobObservation(finalJobObservation);
            await _supervisorMobilityRepository.SaveChangesAsync();
            return Ok(finalJobObservation);
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobObservationDto>>> GetAllJobObservationsAsync()
        {

            var allJobObservations = await _supervisorMobilityRepository.GetAllJobObservationsAsync();

            return Ok(_mapper.Map<IEnumerable<JobObservationDto>>(allJobObservations));
        }

        [HttpGet("{jobObservationId}", Name = "GetJobObservation")]
        public async Task<IActionResult> GetJobObservation(int jobObservationId)
        {
            //Find Job Observation type
            var jobObservation = await _supervisorMobilityRepository.GetJobObservationAsync(jobObservationId);
            if (jobObservation == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<JobObservationDto>(jobObservation));
        }



        [HttpPut("{jobObservationId}")]
        public async Task<ActionResult> UpdateJobObservation(int jobObservationId, JobObservationForUpdateDto jobObservationForUpdate)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(jobObservationForUpdate.PlantId))
            {
                return NotFound();
            }

            if (!await _supervisorMobilityRepository.AreaExistAsync(jobObservationForUpdate.AreaId))
            {
                return NotFound("No Area");
            }

            if (!await _supervisorMobilityRepository.DistributionExistsAsync(jobObservationForUpdate.DistributionId))
            {
                return NotFound("No Distribution");
            }

            if (!await _supervisorMobilityRepository.OperationExistsAsync(jobObservationForUpdate.OperationId))
            {
                return NotFound("No Operation");
            }


            var jobObservationEntity = await _supervisorMobilityRepository.GetJobObservationAsync(jobObservationId);

            if (jobObservationEntity == null)
            {
                return NotFound("Job Observation Not Found");
            }

            _mapper.Map(jobObservationForUpdate, jobObservationEntity);

            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();

        }

        [HttpDelete("{jobObservationId}")]
        public async Task<ActionResult> DeleteJobObservation(int jobObservationId)
        {
            var jobObservation = await _supervisorMobilityRepository.GetJobObservationAsync(jobObservationId);

            if (jobObservation == null)
            {
                return NotFound();
            }

            _supervisorMobilityRepository.DeleteJobObservation(jobObservation);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();
        }


    }
}
