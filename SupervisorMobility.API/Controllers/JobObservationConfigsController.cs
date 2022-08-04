using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.JobObservationConfigsDtos;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/jobobservationtypes/{jobobservationtypeid}/jobobservationconfigs")]
    [ApiController]
    public class JobObservationConfigsController : ControllerBase
    {
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;
        private readonly IMapper _mapper;

        public JobObservationConfigsController(ISupervisorMobilityRepository supervisorMobilityRepository, IMapper mapper)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ?? 
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ?? 
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobObservationConfigsWithoutNavigationPropertiesDto>>> GetJobObservationConfigs(
            int jobObservationTypeId)
        {
            if (!await _supervisorMobilityRepository.JobObservationTypeExistAsync(jobObservationTypeId))
            {
                return NotFound();
            }
          
            var jobObservationConfigForJobObservationType = await _supervisorMobilityRepository
                .GetJobOperationConfigsForJobOperationTypeAsync(jobObservationTypeId);

            return Ok(_mapper.Map<IEnumerable<JobObservationConfigsWithoutNavigationPropertiesDto>>(jobObservationConfigForJobObservationType));
        }

        [HttpGet("{jobobservationconfigid}", Name = "GetJobObservationConfig")]
        public async Task<ActionResult<JobObservationConfigsWithoutNavigationPropertiesDto>> GetJobObservationConfig(int jobObservationTypeId, int jobObservationConfigId)
        {
            if (!await _supervisorMobilityRepository.JobObservationTypeExistAsync(jobObservationTypeId))
            {
                return NotFound();
            }

            var jobObservationConfig = await _supervisorMobilityRepository
                .GetJobOperationConfigForJobOperationTypeAsync(jobObservationTypeId, jobObservationConfigId);

            if (jobObservationConfig == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<JobObservationConfigsWithoutNavigationPropertiesDto>(jobObservationConfig));
        }


        [HttpPost]
        public async Task<ActionResult<JobObservationConfigsWithoutNavigationPropertiesDto>> CreateJobObservationConfig(
            int jobObservationTypeId,
            JobObservationConfigForCreationDto jobObservationConfig)
        {
            if (!await _supervisorMobilityRepository.JobObservationTypeExistAsync(jobObservationTypeId))
            {
                return NotFound();
            }

            var finalJobObservationConfig = _mapper.Map<Entities.JobObservationConfig>(jobObservationConfig);

            await _supervisorMobilityRepository.AddJobOperationConfigForJobOperationTypeAsync(
                jobObservationTypeId, finalJobObservationConfig);

            await _supervisorMobilityRepository.SaveChangesAsync();

            var createdJobObservationConfigToReturn =
                _mapper.Map<JobObservationConfigsWithoutNavigationPropertiesDto>(finalJobObservationConfig);

            return CreatedAtAction("GetJobObservationConfig", 
                new 
                { 
                    jobObservationTypeId = jobObservationTypeId,
                    jobObservationConfigId = createdJobObservationConfigToReturn.JobObservationConfigId
                }, 
                createdJobObservationConfigToReturn);
        }

        [HttpPut("{jobobservationconfigid}")]
        public async Task<IActionResult> PutJobObservationConfig(int jobObservationTypeId, int jobObservationConfigId,
            JobObservationConfigForUpdateDto jobObservationConfig)
        {
            if (!await _supervisorMobilityRepository.JobObservationTypeExistAsync(jobObservationTypeId))
            {
                return NotFound();
            }

            var jobObservationConfigEntity = await _supervisorMobilityRepository
                .GetJobOperationConfigForJobOperationTypeAsync(jobObservationTypeId, jobObservationConfigId);

            if (jobObservationConfigEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(jobObservationConfig, jobObservationConfigEntity);

            await _supervisorMobilityRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{jobobservationconfigid}")]
        public async Task<ActionResult> PartiallyUpdateChecklistQuestion(
            int jobObservationTypeId, int jobObservationConfigId,
            JsonPatchDocument<JobObservationConfigForUpdateDto> patchDocumentJobObservationConfig)
        {
            if (!await _supervisorMobilityRepository.JobObservationTypeExistAsync(jobObservationTypeId))
            {
                return NotFound();
            }

            var jobObservationConfigEntity = await _supervisorMobilityRepository
                .GetJobOperationConfigForJobOperationTypeAsync(jobObservationTypeId, jobObservationConfigId);

            if (jobObservationConfigEntity == null)
            {
                return NotFound();
            }

            var jobObservationConfigToPatch = _mapper.Map<JobObservationConfigForUpdateDto>(jobObservationConfigEntity);

            patchDocumentJobObservationConfig.ApplyTo(jobObservationConfigToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(jobObservationConfigToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(jobObservationConfigToPatch, jobObservationConfigEntity);

            await _supervisorMobilityRepository.SaveChangesAsync();

            return NoContent();
        }

        
        [HttpDelete("{jobobservationconfigid}")]
        public async Task<IActionResult> DeleteJobObservationConfig(
            int jobObservationTypeId, int jobObservationConfigId)
        {
            if (!await _supervisorMobilityRepository.JobObservationTypeExistAsync(jobObservationTypeId))
            {
                return NotFound();
            }

            var jobObservationConfigEntity = await _supervisorMobilityRepository
                .GetJobOperationConfigForJobOperationTypeAsync(jobObservationTypeId, jobObservationConfigId);

            if (jobObservationConfigEntity == null)
            {
                return NotFound();
            }

            _supervisorMobilityRepository.DeleteJobOperationConfig(jobObservationConfigEntity);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}
