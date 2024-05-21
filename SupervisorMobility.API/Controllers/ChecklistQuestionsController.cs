using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.Models.ChecklistQuestionDtos;
using SupervisorMobility.API.Models.LupDtos;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/checklistcategories/{categoryId}/checklistQuestions")]
    [ApiController]
    public class ChecklistQuestionsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IJobObservationService _jobObservationService;

        public ChecklistQuestionsController(
            IMapper mapper,
            IJobObservationService checklistCategoryService)
        {
            _mapper = mapper;
            _jobObservationService = checklistCategoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChecklistQuestionWithoutNavigationPropertiesDto>>> GetChecklistQuestions(
            int categoryId)
        {
            if (!await _jobObservationService.CheckChecklistCategoryExistAsync(categoryId))
            {
                return NotFound("No checklist category found!");
            }

            var checklistQuestionsForCategory = await _jobObservationService.FetchChecklistQuestionsForCategoryAsync(categoryId);

            return Ok(_mapper.Map<IEnumerable<ChecklistQuestionWithoutNavigationPropertiesDto>>(checklistQuestionsForCategory));
        }

        [HttpGet("{checklistQuestionId}", Name = "GetChecklistQuestion")]
        public async Task<ActionResult<ChecklistQuestionWithoutNavigationPropertiesDto>> GetChecklistQuestion(
            int categoryId, int checklistQuestionId)
        {
            if (!await _jobObservationService.CheckChecklistCategoryExistAsync(categoryId))
            {
                return NotFound("No category found!");
            }

            var checklistQuestion = await _jobObservationService.FetchChecklistQuestionForCategoryAsync(categoryId, checklistQuestionId);

            if (checklistQuestion == null)
            {
                return NotFound("No checklist question found!");
            }

            return Ok(_mapper.Map<ChecklistQuestionWithoutNavigationPropertiesDto>(checklistQuestion));
        }

    

        [HttpPost]
        public async Task<ActionResult<ChecklistQuestionWithoutNavigationPropertiesDto>> CreateChecklistQuestion(
            int categoryId,
            ChecklistQuestionForCreationDto checklistQuestion)
        {
            if (!await _jobObservationService.CheckChecklistCategoryExistAsync(categoryId))
            {
                return NotFound("No checklist category found!");
            }

            var finalChecklistQuestion = await _jobObservationService
                .CreateChecklistQuestionForCategoryAsync(categoryId, checklistQuestion);

            var createdChecklistQuestionToReturn =
                _mapper.Map<ChecklistQuestionWithoutNavigationPropertiesDto>(finalChecklistQuestion);

            return CreatedAtRoute("GetChecklistQuestion",
                new
                {
                    categoryId,
                    checKlistQuestionId = createdChecklistQuestionToReturn.QuestionID
                },
                createdChecklistQuestionToReturn);
        }

        [HttpPut("{checklistquestionid}")]
        public async Task<ActionResult> UpdateChecklistQuestion(int categoryId, int checklistquestionid,
            ChecklistQuestionForUpdateDto checklistQuestion)
        {
            if (!await _jobObservationService.CheckChecklistCategoryExistAsync(categoryId))
            {
                return NotFound("No checklist category Found!");
            }

            var checklistQuestionEntity = await _jobObservationService
                .FetchChecklistQuestionForCategoryAsync(categoryId, checklistquestionid);

            if (checklistQuestionEntity == null)
            {
                return NotFound("No checklist question found!");
            }

            await _jobObservationService
                .UpdateChecklistQuestionForCategoryAsync(checklistQuestion, checklistQuestionEntity);

            return Ok(checklistQuestionEntity);
        }

        [HttpPatch("{checklistquestionid}")]
        public async Task<ActionResult> PartiallyUpdateChecklistQuestion(
            int categoryId, int checklistquestionid,
            JsonPatchDocument<ChecklistQuestionForUpdateDto> patchDocumentChecklistQuestion)
        {
            if (!await _jobObservationService.CheckChecklistCategoryExistAsync(categoryId))
            {
                return NotFound("No checklist category found!");
            }

            var checklistQuestionEntity = await _jobObservationService
                .FetchChecklistQuestionForCategoryAsync(categoryId, checklistquestionid);
            if (checklistQuestionEntity == null)
            {
                return NotFound("No checklist question Found!");
            }

            var checklistQuestionToPatch = _mapper.Map<ChecklistQuestionForUpdateDto>(checklistQuestionEntity);

            patchDocumentChecklistQuestion.ApplyTo(checklistQuestionToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(checklistQuestionToPatch))
            {
                return BadRequest(ModelState);
            }

            await _jobObservationService
                .UpdateChecklistQuestionForCategoryAsync(checklistQuestionToPatch, checklistQuestionEntity);

            return NoContent();
        }

        [HttpDelete("{checklistquestionid}")]
        public async Task<ActionResult> DeleteChecklistQuestion(int categoryId, int checklistquestionid)
        {
            if (!await _jobObservationService.CheckChecklistCategoryExistAsync(categoryId))
            {
                return NotFound();
            }

            var checklistQuestionEntity = await _jobObservationService
                .FetchChecklistQuestionForCategoryAsync(categoryId, checklistquestionid);
            if (checklistQuestionEntity == null)
            {
                return NotFound();
            }

            //Send this question to the end
            var checklistQuestionSequence = new ChecklistQuestionSequenceForUpdateDto();
            checklistQuestionSequence.CategorySequence = await _jobObservationService
                .FetchChecklistQuestionMaxSequenceAsync(categoryId) - 1;
            await _jobObservationService
                .UpdateChecklistQuestionsSequenceAsync(checklistQuestionSequence, checklistQuestionEntity, categoryId);

            await _jobObservationService.DeleteChecklistQuestionAsync(checklistQuestionEntity);

            return NoContent();
        }

        [HttpPut("sequence/{checklistquestionid}")]
        public async Task<ActionResult> UpdateChecklistQuestionSequence(
            int categoryId, int checklistQuestionId, ChecklistQuestionSequenceForUpdateDto checklistQuestion)
        {
            if (!await _jobObservationService.CheckChecklistCategoryExistAsync(categoryId))
            {
                return NotFound();
            }

            var checklistQuestionEntity = await _jobObservationService
                .FetchChecklistQuestionForCategoryAsync(categoryId, checklistQuestionId);
            if (checklistQuestionEntity == null)
            {
                return NotFound();
            }

            if (checklistQuestion.CategorySequence == checklistQuestionEntity.CategorySequence)
            {
                return NoContent();
            }
            if (checklistQuestion.CategorySequence < 1
                || checklistQuestion.CategorySequence >
                await _jobObservationService.FetchChecklistQuestionMaxSequenceAsync(categoryId))
            {
                return BadRequest("Sequence must be greater than 1 and lower that the current max sequence.");
            }

            await _jobObservationService
                .UpdateChecklistQuestionsSequenceAsync(checklistQuestion, checklistQuestionEntity, categoryId);

            return NoContent();
        }
    }
}
