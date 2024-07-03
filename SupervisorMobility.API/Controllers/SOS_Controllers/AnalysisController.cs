using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.SOS.EquipmentDtos;
using SupervisorMobility.API.Models.SOS.MaterialDtos;
using SupervisorMobility.API.Models.SOS.SOSAnalysisDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.Models.SOS.ToolDtos;
using SupervisorMobility.API.Models.SOSReviewDtos;
using System.Diagnostics;

namespace SupervisorMobility.API.Controllers.SOS_Controllers
{
    [Route("api/SOS/Analysis")]
    [ApiController]
    public class AnalysisController : Controller
    {
        private readonly ISOSAnalysis_ProcessRepository _AnalysisProcessRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly SupervisorMobilityContext _context;
        public AnalysisController(IWebHostEnvironment env, SupervisorMobilityContext context, IMapper mapper, ISOSAnalysis_ProcessRepository repository)
        {
            _AnalysisProcessRepository = repository;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ??
                  throw new ArgumentNullException(nameof(mapper));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        [HttpPost]
        public async Task<ActionResult<SOSAnalysisDto>> GenerateAnalysis(int SOSHubCollection_Id, string InternalControlNumber, string ProcessNumber)
        {
            SOSHub SOSEntity = await _AnalysisProcessRepository.GetSOSHub(SOSHubCollection_Id);

            SOSAnalysis sOSAnalysisToCreate = new SOSAnalysis();

            //generarlo desde aqui

            sOSAnalysisToCreate.InternalControlNumber = InternalControlNumber;

            sOSAnalysisToCreate.CreatedDate = DateTime.Now;
            sOSAnalysisToCreate.IsActive = true;

            sOSAnalysisToCreate.SOSHubId = SOSHubCollection_Id;

            var createdResult = await _AnalysisProcessRepository.CreateSOSAnalysis(sOSAnalysisToCreate);
            if (createdResult != null)
                return Ok(sOSAnalysisToCreate);
            else
                return BadRequest(); ;

        }

        [HttpGet("{id}", Name = "GetSOSAnalysis")]
        public async Task<ActionResult<SOSAnalysisDto>> GetSOSAnalysis(int id, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false)
        {

            var SOSAnalysis = await _AnalysisProcessRepository.GetSOSAnalysis(id, includeImages, includeNotes, includeLogbooks, includeSpecialCases, includeSOS);
            if (SOSAnalysis == null)
            {
                return NotFound("SOSAnalysis not found!");
            }

            return Ok(_mapper.Map<SOSAnalysisDto>(SOSAnalysis));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SOSAnalysisDto>>> GetAllSOSAnalysis(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false)
        {

            var CheckpointEntities = await _AnalysisProcessRepository.GetAllSOSAnalysis(includeImages, includeNotes, includeLogbooks, includeSpecialCases, includeSOS);
            if (CheckpointEntities == null)
            {
                return NotFound("Get All Sos Analisis not found!");
            }

            return Ok(_mapper.Map<IEnumerable<SOSAnalysisDto>>(CheckpointEntities));
        }

        //Update

        //[HttpPut("{SOSid}")]
        //public async Task<ActionResult> UpdateSOSAnalysis(int SOSid, SOSReviewForUpdateDto sosUpdateEntity)
        //{

            

        //    var result = await _AnalysisProcessRepository.UpdateSOSAnalysis(_CheckpointForUpdate, entityCheckpoint);

        //    if (result > 0)
        //        return Ok(entityCheckpoint);
        //    else
        //        return BadRequest();

        //}//end Update 

        [HttpDelete("{SOSAnaysisId}")]
        public async Task<ActionResult<int>> RemoveSOSHub(int SOSAnaysisId)
        {
            var result = await _AnalysisProcessRepository.RemoveSOSAnalysis(SOSAnaysisId);

            var SOSHub = await _AnalysisProcessRepository.GetSOSHub(SOSAnaysisId);

            if (result > 0)
                return Ok(SOSHub);
            else
                return BadRequest("something wrong");
        }


        //ilustrations

        [HttpPost("Ilustrations/{analysis_id}")]
        public async Task<ActionResult<FileUpload>> UploadIlustrations(int analysis_id, IFormFile file)
        {

            var uploadResult = new FileUploadForCreationDto();
            string trustedFileNameForStorage = string.Empty;
            var unstrustedFileName = file.FileName;

            trustedFileNameForStorage = Path.GetRandomFileName();

            var path = Path.Combine(_env.ContentRootPath, "uploads\\SOSAnalysis\\Ilustrations", trustedFileNameForStorage);
            // Asegurarse de que el directorio de destino exista
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }


            await using FileStream fs = new(path, FileMode.Create);
            await file.CopyToAsync(fs);

            uploadResult.FileName = unstrustedFileName;
            uploadResult.StorageFileName = trustedFileNameForStorage;
            uploadResult.ContentType = file.ContentType;
            uploadResult.UploadDate = DateTime.Now;
            uploadResult.IsActive = true;

            var fileToReturn = await _AnalysisProcessRepository.CreateFileAsync(uploadResult);

            await _AnalysisProcessRepository.AddIlustrationToSOSAnalysis(analysis_id, fileToReturn);
            await _AnalysisProcessRepository.SaveChangesAsync();

            return Ok(fileToReturn);
        }

        [HttpGet("Ilustrations/{fileid}")]
        public async Task<IActionResult> DownloadIlustrations(int fileid)
        {
            var FileInfo = await _AnalysisProcessRepository.FetchFileAsync(fileid);

            if (FileInfo is not null)
            {
                var path = Path.Combine(_env.ContentRootPath, "uploads\\SOSAnalysis\\Ilustrations", FileInfo.StorageFileName);

                var memory = new MemoryStream();
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                var result = File(memory, FileInfo.ContentType, Path.GetFileName(path));
                result.EnableRangeProcessing = true;

                return result;
            }
            return NotFound("Error File download");
        }



    }
}
