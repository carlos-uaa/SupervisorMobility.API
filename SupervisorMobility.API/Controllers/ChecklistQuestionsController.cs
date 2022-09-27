using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.ChecklistQuestionDtos;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/checklistcategories/{categoryId}/checklistQuestions")]
    [ApiController]
    public class ChecklistQuestionsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;

        public ChecklistQuestionsController(ISupervisorMobilityRepository supervisorMobilityRepository,
            IMapper mapper)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ?? 
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChecklistQuestionWithoutNavigationPropertiesDto>>> GetChecklistQuestions(
            int categoryId)
        {
            if (!await _supervisorMobilityRepository.ChecklistCategoryExistAsync(categoryId))
            {
                return NotFound();
            }

            var checklistQuestionsForCategory = await _supervisorMobilityRepository
                .GetChecklistQuestionsForCategoryAsync(categoryId);

            return Ok(_mapper.Map<IEnumerable<ChecklistQuestionWithoutNavigationPropertiesDto>>(checklistQuestionsForCategory));
        }

        [HttpGet("{checklistQuestionId}", Name = "GetChecklistQuestion")]
        public async Task<ActionResult<ChecklistQuestionWithoutNavigationPropertiesDto>> GetChecklistQuestion(
            int categoryId, int checklistQuestionId)
        {
            if (!await _supervisorMobilityRepository.ChecklistCategoryExistAsync(categoryId))
            {
                return NotFound();
            }

            var checklistQuestion = await _supervisorMobilityRepository
                .GetChecklistQuestionForCategoryAsync(categoryId, checklistQuestionId);

            if (checklistQuestion == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ChecklistQuestionWithoutNavigationPropertiesDto>(checklistQuestion));
        }

        [HttpPost]
        public async Task<ActionResult<ChecklistQuestionWithoutNavigationPropertiesDto>> CreateChecklistQuestion(
            int categoryId,
            ChecklistQuestionForCreationDto checklistQuestion)
        {
            if (!await _supervisorMobilityRepository.ChecklistCategoryExistAsync(categoryId))
            {
                return NotFound();
            }

            var finalChecklistQuestion = _mapper.Map<Entities.ChecklistQuestion>(checklistQuestion);
            finalChecklistQuestion.CategorySequence = await 
                _supervisorMobilityRepository.GetChecklistQuestionMaxCategorySequenceAsync(categoryId);

            await _supervisorMobilityRepository.AddChecklistQuestionForCategoryAsync(
                categoryId, finalChecklistQuestion);

            await _supervisorMobilityRepository.SaveChangesAsync();

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
            if (!await _supervisorMobilityRepository.ChecklistCategoryExistAsync(categoryId))
            {
                return NotFound();
            }

            var checklistQuestionEntity = await _supervisorMobilityRepository
                .GetChecklistQuestionForCategoryAsync(categoryId, checklistquestionid);
            if (checklistQuestionEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(checklistQuestion, checklistQuestionEntity);

            await _supervisorMobilityRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{checklistquestionid}")]
        public async Task<ActionResult> PartiallyUpdateChecklistQuestion(
            int categoryId, int checklistquestionid,
            JsonPatchDocument<ChecklistQuestionForUpdateDto> patchDocumentChecklistQuestion)
        {
            if (!await _supervisorMobilityRepository.ChecklistCategoryExistAsync(categoryId))
            {
                return NotFound();
            }

            var checklistQuestionEntity = await _supervisorMobilityRepository
                .GetChecklistQuestionForCategoryAsync(categoryId, checklistquestionid);
            if (checklistQuestionEntity == null)
            {
                return NotFound();
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

            _mapper.Map(checklistQuestionToPatch, checklistQuestionEntity);

            await _supervisorMobilityRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{checklistquestionid}")]
        public async Task<ActionResult> DeleteChecklistQuestion(int categoryId, int checklistquestionid)
        {
            if (!await _supervisorMobilityRepository.ChecklistCategoryExistAsync(categoryId))
            {
                return NotFound();
            }

            var checklistQuestionEntity = await _supervisorMobilityRepository
                .GetChecklistQuestionForCategoryAsync(categoryId, checklistquestionid);
            if (checklistQuestionEntity == null)
            {
                return NotFound();
            }

            _supervisorMobilityRepository.DeleteChecklistQuestions(checklistQuestionEntity);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("sequence/{checklistquestionid}")]
        public async Task<ActionResult> UpdateChecklistQuestionSequence(
            int categoryId, int checklistQuestionId, ChecklistQuestionSequenceForUpdateDto checklistQuestion)
        {
            if (!await _supervisorMobilityRepository.ChecklistCategoryExistAsync(categoryId))
            {
                return NotFound();
            }

            var checklistQuestionEntity = await _supervisorMobilityRepository
                .GetChecklistQuestionForCategoryAsync(categoryId, checklistQuestionId);
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
                await _supervisorMobilityRepository.GetChecklistQuestionMaxCategorySequenceAsync(categoryId))
            {
                return BadRequest("Sequence must be greater than 1 and lower that the current max sequence.");
            }

            //Update just the checklist questions sequences between the desired and the old one.
            var currentSequence =
                checklistQuestion.CategorySequence < checklistQuestionEntity.CategorySequence
                ? checklistQuestion.CategorySequence
                : checklistQuestionEntity.CategorySequence - 1;

            var checklistQuestionsEntities = await 
                _supervisorMobilityRepository.GetChecklistQuestionsForUpdateSequenceAsync(
                checklistQuestion.CategorySequence, 
                checklistQuestionEntity.CategorySequence, 
                categoryId, checklistQuestionId);

            foreach (var checklistQuestionsEntityForUpdate in checklistQuestionsEntities)
            {
                currentSequence += 1;
                checklistQuestionsEntityForUpdate.CategorySequence = currentSequence;
            }

            _mapper.Map(checklistQuestion, checklistQuestionEntity);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}
