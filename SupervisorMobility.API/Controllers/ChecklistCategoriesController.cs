using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Models.ChecklistCategoryDtos;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Controllers
{
    [ApiController]
    [Route("api/checklistcategories")]
    public class ChecklistCategoriesController : ControllerBase
    {
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;
        private readonly IMapper _mapper;

        public ChecklistCategoriesController(ISupervisorMobilityRepository supervisorMobilityRepository, IMapper mapper)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
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
                return Ok(_mapper.Map<ChecklistCategoryWithJustchecklistQuestions>(checklistCategory));
            }

            return Ok(_mapper.Map<ChecklistCategoryWithoutChecklistQuestionsDto>(checklistCategory));
        }

        [HttpPost]
        public async Task<ActionResult<ChecklistCategoryDto>> CreateChecklistCategory(
            ChecklistCategoryForCreationDto checklistCategory)
        {
            //Map object ans save it to the DB
            var finalChecklistCategory = _mapper.Map<Entities.ChecklistCategory>(checklistCategory);
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
            var checklistCategoryEntity = await _supervisorMobilityRepository.GetChecklistCategoryAsync(checklistCategoryId);
            if (checklistCategoryEntity == null)
            {
                return NotFound();
            }

            _supervisorMobilityRepository.DeleteChecklistCategory(checklistCategoryEntity);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return NoContent();
        }

    }
}
