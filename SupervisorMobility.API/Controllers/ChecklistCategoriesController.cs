using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.Models.ChecklistCategoryDtos;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Controllers
{
    [ApiController]
    [Route("api/checklistcategories")]
    public class ChecklistCategoriesController : ControllerBase
    {
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository; //TODO: Remove from business logic
        private readonly IChecklistCategoryService _checklistCategoryService;
        private readonly IMapper _mapper;

        public ChecklistCategoriesController(
            ISupervisorMobilityRepository supervisorMobilityRepository, 
            IMapper mapper, 
            IChecklistCategoryService checklistCategoryService)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository;
            _mapper = mapper;
            _checklistCategoryService = checklistCategoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChecklistCategoryWithoutChecklistQuestionsDto>>> GetChecklistCategories()
        {
            var checklistCategoryEntities = await _supervisorMobilityRepository.GetChecklistCategoriesAsync();
            return Ok(_mapper.Map<IEnumerable<ChecklistCategoryWithoutChecklistQuestionsDto>>(checklistCategoryEntities));
        }

        [HttpGet("{id}", Name = "GetChecklistCategory")]
        public async Task<IActionResult> GetChecklistCategory(int id, bool includeChecklistQuestions = false)
        {
            //Find Checklist category
            var checklistCategory = await _supervisorMobilityRepository
                .GetChecklistCategoryAsync(id, includeChecklistQuestions);
            if (checklistCategory == null)
            {
                return NotFound();
            }

            if (includeChecklistQuestions)
            {
                return Ok(_mapper.Map<ChecklistCategoryWithJustchecklistQuestionsDto>(checklistCategory));
            }

            return Ok(_mapper.Map<ChecklistCategoryWithoutChecklistQuestionsDto>(checklistCategory));
        }

        [HttpPost]
        public async Task<ActionResult<ChecklistCategoryDto>> CreateChecklistCategory(
            ChecklistCategoryForCreationDto checklistCategory)
        {
            //Map object ans save it to the DB
            var finalChecklistCategory = _mapper.Map<Entities.ChecklistCategory>(checklistCategory);
            finalChecklistCategory.Sequence = await _supervisorMobilityRepository.GetChecklistCategoriesMaxSequenceAsync();
            _supervisorMobilityRepository.AddChecklistCategory(finalChecklistCategory);
            await _supervisorMobilityRepository.SaveChangesAsync();

            var createChecklistCategoryToReturn = 
                _mapper.Map<ChecklistCategoryDto>(finalChecklistCategory);

            return CreatedAtRoute("GetChecklistCategory",
                new
                {
                    id = createChecklistCategoryToReturn.ChecklistCategoryId
                },
                createChecklistCategoryToReturn);
        }

        [HttpPut("{checklistCategoryId}")]
        public async Task<ActionResult> UpdateChecklistCategory(int checklistCategoryId,
            ChecklistCategoryForUpdateDto checklistCategory)
        {
            var checklistCategoryEntity = await _supervisorMobilityRepository.GetChecklistCategoryAsync(checklistCategoryId);
            if (checklistCategoryEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(checklistCategory, checklistCategoryEntity);

            await _supervisorMobilityRepository.SaveChangesAsync();

            return NoContent();

        }

        [HttpPatch("{checklistCategoryId}")]
        public async Task<ActionResult> PartiallyUpdateChecklistQuestion(
            int checklistCategoryId,
            JsonPatchDocument<ChecklistCategoryForUpdateDto> patchDocumentChecklistCategory)
        {
            var checklistCategoryEntity = await _supervisorMobilityRepository.GetChecklistCategoryAsync(checklistCategoryId);
            if (checklistCategoryEntity == null)
            {
                return NotFound();
            }

            var checklistCategoryToPatch = _mapper.Map<ChecklistCategoryForUpdateDto>(checklistCategoryEntity);

            patchDocumentChecklistCategory.ApplyTo(checklistCategoryToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(checklistCategoryToPatch))
            {
                return BadRequest();
            }

            _mapper.Map(checklistCategoryToPatch, checklistCategoryEntity);

            await _supervisorMobilityRepository.SaveChangesAsync();

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
            var checklistCategorySequence = new ChecklistCategorySequenceForUpdateDto();
            checklistCategorySequence.Sequence = await _checklistCategoryService.FetchChecklistCategoriesMaxSequenceAsync() - 1;
            await _checklistCategoryService
                .UpdateChecklistCategoriesSequenceAsync(checklistCategorySequence, checklistCategoryEntity);

            await _checklistCategoryService.DeleteChecklistCategoryAsync(checklistCategoryEntity);

            return NoContent();
        }

        [HttpPut("sequence/{checklistCategoryId}")]
        public async Task<ActionResult> UpdateChecklistCategorySequence(int checklistCategoryId,
            ChecklistCategorySequenceForUpdateDto checklistCategory)
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
