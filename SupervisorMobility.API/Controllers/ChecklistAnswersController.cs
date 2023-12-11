using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.ChecklistAnswerDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Services;
using System.Linq;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/checklistAnswers")]
    [ApiController]
    public class ChecklistAnswersController : ControllerBase
    {
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly IAssyChartService _assyChartService;
        public ChecklistAnswersController(ISupervisorMobilityRepository supervisorMobilityRepository, IMapper mapper, IWebHostEnvironment env, IAssyChartService assyChartService)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _assyChartService = assyChartService ??
                throw new ArgumentNullException(nameof(assyChartService)); ;
            _env = env;
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

        public class ChecklistContent 
        {
            public List<IFormFile>? Files { get; set; }
            public ChecklistAnswerDto? checklistAnswer { get; set; }
            public List<FileUpload>? Evidences { get; set; }
        }

        [EnableCors]
        [HttpPost]
        public async Task<ActionResult<ChecklistAnswerDto>> CreateChecklistAnswer([FromForm] ChecklistContent CkContent)
        {

            var checklistAnswer = CkContent.checklistAnswer;

            if (!await _supervisorMobilityRepository.JobObservationExistAsync((int)checklistAnswer.JobObservationId))
            {
                return NotFound();
            }

            var finalChecklistAnswer = _mapper.Map<ChecklistAnswer>(checklistAnswer);

            _supervisorMobilityRepository.AddChecklistAnswer(finalChecklistAnswer);


            //Upload Images in foreach

            foreach(var file in CkContent.Files)
            {
                var uploadResult = new FileUploadForCreationDto();
                string trustedFileNameForStorage = string.Empty;
                var unstrustedFileName = file.FileName;

                trustedFileNameForStorage = Path.GetRandomFileName();
                var path = Path.Combine(_env.ContentRootPath, "uploads\\evidence", trustedFileNameForStorage);

                await using FileStream fs = new(path, FileMode.Create);
                await file.CopyToAsync(fs);

                uploadResult.FileName = unstrustedFileName;
                uploadResult.StorageFileName = trustedFileNameForStorage;
                uploadResult.ContentType = file.ContentType;
                uploadResult.UploadDate = DateTime.Now;

                var fileToReturn = await _assyChartService.CreateFileAsync(uploadResult);
                await _supervisorMobilityRepository.AddEvidenceForCkAnswerAsync(finalChecklistAnswer.AnswerId, fileToReturn);
            }

            await _supervisorMobilityRepository.SaveChangesAsync();
            return Ok(finalChecklistAnswer);
        }

        [HttpPost("evidences")]
        public async Task<ActionResult<ChecklistAnswerDto>> CreateEvidencesChecklistAnswer([FromForm] ChecklistContent CkContent)
        {

            var checklistAnswer = CkContent.checklistAnswer;
           
            var finalChecklistAnswer = await _supervisorMobilityRepository.GetChecklistAnswerAsync(checklistAnswer.AnswerId);


            if(CkContent.Files?.Count > 0)
            {

            foreach (var file in CkContent.Files)
            {
                var uploadResult = new FileUploadForCreationDto();
                string trustedFileNameForStorage = string.Empty;
                var unstrustedFileName = file.FileName;

                trustedFileNameForStorage = Path.GetRandomFileName();
                var path = Path.Combine(_env.ContentRootPath, "uploads\\evidence", trustedFileNameForStorage);

                await using FileStream fs = new(path, FileMode.Create);
                await file.CopyToAsync(fs);

                uploadResult.FileName = unstrustedFileName;
                uploadResult.StorageFileName = trustedFileNameForStorage;
                uploadResult.ContentType = file.ContentType;
                uploadResult.UploadDate = DateTime.Now;

                var fileToReturn = await _assyChartService.CreateFileAsync(uploadResult);
                await _supervisorMobilityRepository.AddEvidenceForCkAnswerAsync(finalChecklistAnswer.AnswerId, fileToReturn);
            }
            await _supervisorMobilityRepository.SaveChangesAsync();
            }

            return Ok(finalChecklistAnswer);
        }

        [HttpPost("RemoveEvidences/{idAnswer}")]
        public async Task<ActionResult<ChecklistAnswerDto>> RemoveEvidencesChecklistAnswer(int idAnswer, List<int> EvidencesToRemove)
        {

            var finalChecklistAnswer = await _supervisorMobilityRepository.GetChecklistAnswerAsync(idAnswer);


            if (EvidencesToRemove.Count > 0)
            {

                foreach (var file in EvidencesToRemove)
                {
                    FileUpload ToRemove = finalChecklistAnswer.Evidences.ToList().Find(e => e.FileUploadId == file);

                    finalChecklistAnswer.Evidences.Remove(ToRemove);
                }
                await _supervisorMobilityRepository.SaveChangesAsync();
            }

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

            return Ok(checklistAnswerEntity);

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
