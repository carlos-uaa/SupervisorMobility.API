using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.LupDtos;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/lup")]
    [ApiController]
    public class LupController : ControllerBase
    {
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;
        private readonly IMapper _mapper;

        public LupController(ISupervisorMobilityRepository supervisorMobilityRepository, IMapper mapper)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LupDto>>> GetAllLupAsync()
        {

            var allLup = await _supervisorMobilityRepository.GetAllLupAsync();

            return Ok(_mapper.Map<IEnumerable<LupDto>>(allLup));
        }

        [HttpGet("{lupId}", Name = "GetLup")]
        public async Task<IActionResult> GetLup(int lupId, bool includeFile = false)
        {
            if (includeFile)
            {
                var lup = await _supervisorMobilityRepository.GetLupAsync(lupId, includeFile);
                if (lup == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<LupWithFilesDto>(lup));
            }
            else
            {
                var lup = await _supervisorMobilityRepository.GetLupAsync(lupId);
                if (lup == null)
                {
                    return NotFound();
                }

                return Ok(_mapper.Map<LupDto>(lup));
            }

        }

        [HttpGet("ByFilters")]
        public async Task<ActionResult<IEnumerable<LupDto>>> GetLupsByFilters(
            DateTime startDate,
            DateTime endDate,
            int plantId,
            int areaId,
            int distributionId,
            int operationId,
            int supervisorId,
            int status)
        {

            var allJobObservations = await _supervisorMobilityRepository.GetLupsByFiltersAsync(startDate, endDate, plantId, areaId, distributionId, operationId, supervisorId, status);
            return Ok(_mapper.Map<IEnumerable<LupDto>>(allJobObservations));
        }


        [HttpGet("Insidences/{checklistQuestionId}")]
        public async Task<ActionResult<List<LupDto>>> GetChecklistQuestionInsidences(int checklistQuestionId, int supervisor_id, int distributionId)
        {
            var checklistQuestions = await _supervisorMobilityRepository.GetAllLupInsidences(checklistQuestionId, supervisor_id, distributionId);

            if (checklistQuestions == null)
            {
                return NotFound("No checklist Question Insidences found!");
            }

            return Ok(_mapper.Map<IEnumerable<LupDto>>(checklistQuestions));
        }

        [HttpPost]
        public async Task<ActionResult<LupWithoutNavigationPropertiesDto>> CreateLup(
            LupForCreationDto lup)
        {

            if (!await _supervisorMobilityRepository.JobObservationExistAsync(lup.JobObservationId))
            {
                return NotFound();
            }

            var finalLup = _mapper.Map<Lup>(lup);

            _supervisorMobilityRepository.AddLup(finalLup);
            await _supervisorMobilityRepository.SaveChangesAsync();
            return Ok(finalLup);
        }

        [HttpPut("{lupId}")]
        public async Task<ActionResult> UpdateLup(int lupId, LupForUpdateDto lupForUpdate)
        {

            if (!await _supervisorMobilityRepository.JobObservationExistAsync(lupForUpdate.JobObservationId))
            {
                return NotFound();
            }


            var lupEntity = await _supervisorMobilityRepository.GetLupAsync(lupId);

            if (lupEntity == null)
            {
                return NotFound("Lup Not Found");
            }

            _mapper.Map(lupForUpdate, lupEntity);

            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();

        }

        [HttpDelete("{lupId}")]
        public async Task<ActionResult> DeleteLup(int lupId)
        {
            var lup = await _supervisorMobilityRepository.GetLupAsync(lupId);

            if (lup == null)
            {
                return NotFound();
            }

            _supervisorMobilityRepository.DeleteLup(lup);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("{lupId}/evidence/remove")]
        public async Task<ActionResult<int>> RemoveEvidence(int lupId, [FromBody] int fileUploadId)
        {

            if (!await _supervisorMobilityRepository.LupExistAsync(lupId))
            {
                return NotFound("No lup Exists");
            }


            await _supervisorMobilityRepository.RemoveEvidenceForLupAsync(lupId, fileUploadId);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();
        }


    }
}
