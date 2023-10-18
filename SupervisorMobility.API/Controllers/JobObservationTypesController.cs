using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.Models.JobObservationTypeDtos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SupervisorMobility.API.Controllers
{
    [Route("api/jobobservationtypes")]
    [ApiController]
    public class JobObservationTypesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IJobObservationService _jobObservationService;

        public JobObservationTypesController(IMapper mapper, IJobObservationService jobObservationService)
        {
            _jobObservationService = jobObservationService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobObservationTypeWithoutConfigsDto>>> GetJobObservationTypes()
        {
            var jobObservationTypeEntities = await _jobObservationService.FetchJobObservationTypesAsync();
            return Ok(_mapper.Map<IEnumerable<JobObservationTypeWithoutConfigsDto>>(jobObservationTypeEntities));
        }

        [HttpGet("{id}", Name = "GetJobObservationType")]
        public async Task<ActionResult> GetJobObservationType(int id, bool includeConfigs = false)
        {
            //Find Job Observation type
            var jobObservationType = await _jobObservationService
                .FetchJobObservationTypeAsync(id, includeConfigs);
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
            //Map the object
            var finalJobObservationType = await _jobObservationService.CreateJobObservationTypeAsync(jobObservationType);

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
            var jobObservationTypeEntity = await _jobObservationService.FetchJobObservationTypeAsync(jobObservationTypeId);
            if (jobObservationTypeEntity == null)
            {
                return NotFound();
            }

            await _jobObservationService.UpdateJobObservationTypeAsync(jobObservationType, jobObservationTypeEntity);

            return NoContent();

        }

        [HttpPatch("{jobObservationTypeId}")]
        public async Task<ActionResult> PartiallyUpdateJobObservationType(
            int jobObservationTypeId,
            JsonPatchDocument<JobObservationTypeForUpdateDto> patchDocumentJobObservationType)
        {
            var jobObservationTypeEntity = await _jobObservationService.FetchJobObservationTypeAsync(jobObservationTypeId);
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

            await _jobObservationService.UpdateJobObservationTypeAsync(jobObservationTypeToPatch, jobObservationTypeEntity);

            return NoContent();
        }

        [HttpDelete("{jobObservationTypeId}")]
        public async Task<ActionResult> DeleteJobObservationType(int jobObservationTypeId)
        {
            var jobObservationTypeEntity = await _jobObservationService.FetchJobObservationTypeAsync(jobObservationTypeId);
            if (jobObservationTypeEntity == null)
            {
                return NotFound();
            }

            await _jobObservationService.DeleteJobObservationTypeAsync(jobObservationTypeEntity);

            return NoContent();
        }
    }
}
