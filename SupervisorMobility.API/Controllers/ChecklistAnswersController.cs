using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.ChecklistAnswerDtos;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/checklistAnswers")]
    [ApiController]
    public class ChecklistAnswersController : ControllerBase
    {
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;
        private readonly IMapper _mapper;

        public ChecklistAnswersController(ISupervisorMobilityRepository supervisorMobilityRepository, IMapper mapper)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChecklistAnswerDto>>> GetAllChecklistAnswerAsync()
        {

            var allChecklistAnswer = await _supervisorMobilityRepository.GetAllChecklistAnswerAsync();

            return Ok(_mapper.Map<IEnumerable<ChecklistAnswerDto>>(allChecklistAnswer));
        }

        [HttpGet("{checklistAnswerId}", Name = "GetChecklistAnswer")]
        public async Task<IActionResult> GetChecklistAnswer(int checklistAnswerId)
        {

            var checklistAnswer = await _supervisorMobilityRepository.GetChecklistAnswerAsync(checklistAnswerId);
            if (checklistAnswer == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<ChecklistAnswerDto>(checklistAnswer));


        }


        [HttpGet("JobObservationId/{jobObservationId}", Name = "GetAnswersByJobObservationId")]
        public async Task<ActionResult<IEnumerable<ChecklistAnswerDto>>> GetAllChecklistAnswerAsync(int jobObservationId)
        {

            var allChecklistAnswer = await _supervisorMobilityRepository.GetAllChecklistAnswersByJobObservationIdAsync(jobObservationId);

            return Ok(_mapper.Map<IEnumerable<ChecklistAnswerDto>>(allChecklistAnswer));
        }

        [HttpPost]
        public async Task<ActionResult<ChecklistAnswerDto>> CreateChecklistAnswer(
            ChecklistAnswerForCreationDto checklistAnswer)
        {

            if (!await _supervisorMobilityRepository.JobObservationExistAsync(checklistAnswer.JobObservationId))
            {
                return NotFound();
            }

            var finalChecklistAnswer = _mapper.Map<ChecklistAnswer>(checklistAnswer);

            _supervisorMobilityRepository.AddChecklistAnswer(finalChecklistAnswer);
            await _supervisorMobilityRepository.SaveChangesAsync();
            return Ok(finalChecklistAnswer);
        }

        [HttpPut("{checklistAnswerId}")]
        public async Task<ActionResult> UpdateChecklistAnswer(int checklistAnswerId, ChecklistAnswerForUpdateDto checklistAnswerForUpdate)
        {

            if (!await _supervisorMobilityRepository.JobObservationExistAsync(checklistAnswerForUpdate.JobObservationId))
            {
                return NotFound();
            }


            var checklistAnswerEntity = await _supervisorMobilityRepository.GetChecklistAnswerAsync(checklistAnswerId);

            if (checklistAnswerEntity == null)
            {
                return NotFound("ChecklistAnswer Not Found");
            }

            _mapper.Map(checklistAnswerForUpdate, checklistAnswerEntity);

            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();

        }

        [HttpDelete("{checklistAnswerId}")]
        public async Task<ActionResult> DeleteChecklistAnswer(int checklistAnswerId)
        {
            var checklistAnswer = await _supervisorMobilityRepository.GetChecklistAnswerAsync(checklistAnswerId);

            if (checklistAnswer == null)
            {
                return NotFound();
            }

            _supervisorMobilityRepository.DeleteChecklistAnswer(checklistAnswer);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();
        }


    }
}
