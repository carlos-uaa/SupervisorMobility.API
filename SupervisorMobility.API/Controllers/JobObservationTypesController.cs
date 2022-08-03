using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Models.JobObservationTypeDtos;
using SupervisorMobility.API.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SupervisorMobility.API.Controllers
{
    [Route("api/jobobservationtypes")]
    [ApiController]
    public class JobObservationTypesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;

        public JobObservationTypesController(IMapper mapper, ISupervisorMobilityRepository supervisorMobilityRepository)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ?? 
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobObservationTypeWithoutConfigsDto>>> GetJobObservationTypes()
        {
            var jobObservationTypeEntities = await _supervisorMobilityRepository.GetJobObservationTypesAsync();
            return Ok(_mapper.Map<IEnumerable<JobObservationTypeWithoutConfigsDto>>(jobObservationTypeEntities));  
        }

        [HttpGet("{id}", Name = "GetJobObservationType")]
        public async Task<ActionResult> GetJobObservationType(int id, bool includeConfigs = false)
        {
            //Find Job Observation type
            var jobObservationType = await _supervisorMobilityRepository
                .GetJobObservationTypeAsync(id, includeConfigs);
            if (jobObservationType == null)
            {
                return NotFound();
            }

            if (includeConfigs)
            {
                return Ok(_mapper.Map<JobObservationTypeDto>(jobObservationType));
            }

            return Ok(_mapper.Map<JobObservationTypeWithoutConfigsDto>(jobObservationType));
        }

        [HttpPost]
        public async Task<ActionResult<JobObservationTypeDto>> CreateJobObservationType(
            JobObservationTypeForCreationDto jobObservationType)
        {
            //Mpa the pbject
            var finalJobObservationType = _mapper.Map<Entities.JobObservationType>(jobObservationType);
            _supervisorMobilityRepository.AddJobObservationType(finalJobObservationType);
            await _supervisorMobilityRepository.SaveChangesAsync();

            var createJobObservationTypeToReturn =
                _mapper.Map<JobObservationTypeDto>(finalJobObservationType);

            return CreatedAtRoute("GetJobObservationType",
                new
                {
                    id = createJobObservationTypeToReturn.JobObservationTypeId
                },
                createJobObservationTypeToReturn);
        }


        [HttpPut("{jobObservationTypeId}")]
        public async Task<ActionResult> UpdateJobObservationType(int jobObservationTypeId,
            JobObservationTypeForUpdateDto jobObservationType)
        {
            var jobObservationTypeEntity = await _supervisorMobilityRepository.GetJobObservationTypeAsync(jobObservationTypeId);
            if (jobObservationTypeEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(jobObservationType, jobObservationTypeEntity);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return NoContent();

        }

        [HttpPatch("{jobObservationTypeId}")]
        public async Task<ActionResult> PartiallyUpdateJobObservationType(
            int jobObservationTypeId,
            JsonPatchDocument<JobObservationTypeForUpdateDto> patchDocumentJobObservationType)
        {
            var jobObservationTypeEntity = await _supervisorMobilityRepository.GetJobObservationTypeAsync(jobObservationTypeId);
            if (jobObservationTypeEntity == null)
            {
                return NotFound();
            }

            var jobObservationTypeToPatch = _mapper.Map<JobObservationTypeForUpdateDto>(jobObservationTypeEntity);

            patchDocumentJobObservationType.ApplyTo(jobObservationTypeToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(patchDocumentJobObservationType))
            {
                return BadRequest();
            }

            _mapper.Map(jobObservationTypeToPatch, jobObservationTypeEntity);

            await _supervisorMobilityRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{jobObservationTypeId}")]
        public async Task<ActionResult> DeleteJobObservationType(int jobObservationTypeId)
        {
            var jobObservationTypeEntity = await _supervisorMobilityRepository.GetJobObservationTypeAsync(jobObservationTypeId);
            if (jobObservationTypeEntity == null)
            {
                return NotFound();
            }

            _supervisorMobilityRepository.DeleteJobObservationType(jobObservationTypeEntity);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}
