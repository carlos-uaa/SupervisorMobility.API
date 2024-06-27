using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Models.SOS.EquipmentDtos;
using SupervisorMobility.API.Models.SOS.MaterialDtos;
using SupervisorMobility.API.Models.SOS.SOSAnalysisDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.Models.SOS.ToolDtos;
using System.Diagnostics;

namespace SupervisorMobility.API.Controllers.SOS_Controllers
{
    [Route("api/Analysis_Process/Analysis")]
    [ApiController]
    public class AnalysisController : Controller
    {
        private readonly ISOSAnalysis_ProcessRepository _AnalysisProcessRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        public AnalysisController(IWebHostEnvironment env, IMapper mapper, ISOSAnalysis_ProcessRepository repository)
        {
            _AnalysisProcessRepository = repository;
            _mapper = mapper ??
                  throw new ArgumentNullException(nameof(mapper));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        [HttpPost]
        public async Task<ActionResult<SOSAnalysisDto>> GenerateAnalysis(int SOSHubCollection_Id, string InternalControlNumber, string ProcessNumber)
        {
            SOSHub SOSEntity = await _AnalysisProcessRepository.GetSOSHub(SOSHubCollection_Id);

            SOSAnalysis sOSAnalysisToCreate = new SOSAnalysis();


            sOSAnalysisToCreate.InternalControlNumber = InternalControlNumber;
            sOSAnalysisToCreate.ProcessNumber = ProcessNumber;

            sOSAnalysisToCreate.CreatedDate = DateTime.Now;
            sOSAnalysisToCreate.IsActive = true;

            sOSAnalysisToCreate.SOSHubId = SOSHubCollection_Id;

            var createdResult = await _AnalysisProcessRepository.CreateSOSAnalysis(sOSAnalysisToCreate);
            if (createdResult != null)
                return Ok(SOSEntity);
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

    }
}
