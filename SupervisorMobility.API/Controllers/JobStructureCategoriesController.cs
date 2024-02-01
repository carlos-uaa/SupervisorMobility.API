using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.ChecklistCategoryDtos;

namespace SupervisorMobility.API.Controllers
{
    [ApiController]
    [Route("api/checklistcategories")]
    public class JobStructureCategoriesController : ControllerBase
    {
        private readonly IJobObservationService _checklistCategoryService;
        private readonly IMapper _mapper;

        public JobStructureCategoriesController(
            IMapper mapper,
            IJobObservationService checklistCategoryService)
        {
            _mapper = mapper;
            _checklistCategoryService = checklistCategoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobCategoryStructureWithoutChecklistQuestionsDto>>> GetChecklistCategories(bool includeChecklistQuestions = false)
        {

            var checklistCategoryEntities = await _checklistCategoryService.FetchChecklistCategoriesAsync(includeChecklistQuestions);
            if (checklistCategoryEntities == null)
            {
                return NotFound("Checklist categories not found!");
            }

            if (includeChecklistQuestions)
            {
                return Ok(_mapper.Map<IEnumerable<JobCategoryStructureWithJustchecklistQuestionsDto>>(checklistCategoryEntities));
            }
            return Ok(_mapper.Map<IEnumerable<JobCategoryStructureWithoutChecklistQuestionsDto>>(checklistCategoryEntities));
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<JobCategoryStructureWithoutChecklistQuestionsDto>>> GetAllChecklistCategories(bool includeChecklistQuestions = false)
        {

            var checklistCategoryEntities = await _checklistCategoryService.FetchChecklistCategoriesAsync(includeChecklistQuestions);
            if (checklistCategoryEntities == null)
            {
                return NotFound("Checklist categories not found!");
            }

            if (includeChecklistQuestions)
            {
                return Ok(_mapper.Map<IEnumerable<JobCategoryStructureWithJustchecklistQuestionsDto>>(checklistCategoryEntities));
            }
            return Ok(_mapper.Map<IEnumerable<JobCategoryStructureWithoutChecklistQuestionsDto>>(checklistCategoryEntities));
        }

        [HttpGet("{id}", Name = "GetChecklistCategory")]
        public async Task<IActionResult> GetChecklistCategory(int id, bool includeChecklistQuestions = false)
        {
            //Find Checklist category
            var checklistCategory = await _checklistCategoryService.FetchChecklistCategoryAsync(id, includeChecklistQuestions);
            if (checklistCategory == null)
            {
                return NotFound("Checklist category not found!");
            }

            if (includeChecklistQuestions)
            {
                return Ok(_mapper.Map<JobCategoryStructureWithJustchecklistQuestionsDto>(checklistCategory));
            }

            return Ok(_mapper.Map<JobCategoryStructureWithoutChecklistQuestionsDto>(checklistCategory));
        }

        [HttpPost]
        public async Task<ActionResult<JobCategoryStructureDto>> CreateChecklistCategory(
            JobCategoryStructureForCreationDto checklistCategory)
        {
            var finalChecklistCategory = await _checklistCategoryService.CreateChecklistCategoryAsync(checklistCategory);

            var createChecklistCategoryToReturn =
                _mapper.Map<JobCategoryStructureDto>(finalChecklistCategory);

            return CreatedAtRoute("GetChecklistCategory",
                new
                {
                    id = createChecklistCategoryToReturn.JobCategoryStructureId
                },
                createChecklistCategoryToReturn);
        }

        [HttpPut("{checklistCategoryId}")]
        public async Task<ActionResult> UpdateChecklistCategory(int checklistCategoryId,
            JobCategoryStructureForUpdateDto checklistCategory)
        {
            var checklistCategoryEntity = await _checklistCategoryService.FetchChecklistCategoryAsync(checklistCategoryId);
            if (checklistCategoryEntity == null)
            {
                return NotFound();
            }

            await _checklistCategoryService.UpdateChecklistCategoryAsync(checklistCategory, checklistCategoryEntity);

            return NoContent();

        }

        [HttpPatch("{checklistCategoryId}")]
        public async Task<ActionResult> PartiallyUpdateChecklistQuestion(
            int checklistCategoryId,
            JsonPatchDocument<JobCategoryStructureForUpdateDto> patchDocumentChecklistCategory)
        {
            var checklistCategoryEntity = await _checklistCategoryService.FetchChecklistCategoryAsync(checklistCategoryId);
            if (checklistCategoryEntity == null)
            {
                return NotFound();
            }

            var checklistCategoryToPatch = _mapper.Map<JobCategoryStructureForUpdateDto>(checklistCategoryEntity);

            patchDocumentChecklistCategory.ApplyTo(checklistCategoryToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(checklistCategoryToPatch))
            {
                return BadRequest();
            }

            await _checklistCategoryService.UpdateChecklistCategoryAsync(checklistCategoryToPatch, checklistCategoryEntity);

            return NoContent();

        }

        [HttpDelete("{ChecklistCategoryId}")]
        public async Task<ActionResult> DeleteChecklistCategory(int checklistCategoryId)
        {
            var checklistCategoryEntity = await _checklistCategoryService.FetchChecklistCategoryAsync(checklistCategoryId);
            if (checklistCategoryEntity == null)
            {
                return NotFound("Checklist category not found.");
            }

            //Send this category to the end
            var checklistCategorySequence = new JobCategoryStructureSequenceForUpdateDto();
            checklistCategorySequence.Sequence = await _checklistCategoryService.FetchChecklistCategoriesMaxSequenceAsync() - 1;
            await _checklistCategoryService
                .UpdateChecklistCategoriesSequenceAsync(checklistCategorySequence, checklistCategoryEntity);

            await _checklistCategoryService.DeleteChecklistCategoryAsync(checklistCategoryEntity);

            return NoContent();
        }

        [HttpPut("sequence/{checklistCategoryId}")]
        public async Task<ActionResult> UpdateChecklistCategorySequence(int checklistCategoryId,
            JobCategoryStructureSequenceForUpdateDto checklistCategory)
        {
            var checklistCategoryEntity = await _checklistCategoryService.FetchChecklistCategoryAsync(checklistCategoryId);
            if (checklistCategoryEntity == null)
            {
                return NotFound("Checklist category not found.");
            }

            if (checklistCategory.Sequence == checklistCategoryEntity.Sequence)
            {
                return NoContent();
            }

            if (checklistCategory.Sequence < 1
                || checklistCategory.Sequence > await _checklistCategoryService.FetchChecklistCategoriesMaxSequenceAsync())
            {
                return BadRequest("Sequence must be greater than 1 and lower that the current max sequence.");
            }

            await _checklistCategoryService
                .UpdateChecklistCategoriesSequenceAsync(checklistCategory, checklistCategoryEntity);

            return NoContent();

        }
    }
}
