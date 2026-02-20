using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.DataAccess.Services.SOS_AnalysisRepository;
using SupervisorMobility.API.DataAccess.Services.SOS_SequenceRepository;
using SupervisorMobility.API.DataAccess.Services.SOS_SynopticTableRepository;
using SupervisorMobility.API.Interfaces.SOSDistribution.SOSDistributionExcel;
using SupervisorMobility.API.Models.SOS.SOSSynopticTableofControlPointsDtos;
using SupervisorMobility.API.Models.SOS.SOSSynopticTableofControlPointsDtos;
using SupervisorMobility.API.Models.SOS.SOSSynopticTableofControlPointsDtos;
using System.Diagnostics;

namespace SupervisorMobility.API.Controllers.SOS_Controllers
{
    [Route("api/SOS/SynopticTableofControlPoints")]
    [ApiController]
    public class SynopticTableofControlPointsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly ISOS_ProcessRepository _ProcessRepository;
        private readonly ISOS_SynopticTableRepository _SynopticTableRepository;
        private readonly ISOS_SequenceRepository _SequenceRepository;
        private readonly ISOS_AnalysisRepository _AnalisisRepository;
        public SynopticTableofControlPointsController(IWebHostEnvironment env, IMapper mapper, ISOS_ProcessRepository repository, ISOS_SynopticTableRepository synopticTableRepository, ISOS_SequenceRepository sequenceRepository, ISOS_AnalysisRepository analysisRepository)
        {
            _ProcessRepository = repository;
            _mapper = mapper ??
                  throw new ArgumentNullException(nameof(mapper));
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _SynopticTableRepository = synopticTableRepository;
            _SequenceRepository = sequenceRepository;
            _AnalisisRepository = analysisRepository;
        }

        [HttpPost]
        public async Task<ActionResult<SOSSynopticControlPointsDto>> GenerateSynopticTableofControlPoints(SOSSynopticTableofControlPointsForCreateDto sOSSynopticTableofControlPointsToCreate, int SOSHubCollection_Id)
        {

            if (sOSSynopticTableofControlPointsToCreate.SOSSynopticTableofControlPointsId == 0)
            {
                sOSSynopticTableofControlPointsToCreate.CreatedAt = DateTime.Now;
                sOSSynopticTableofControlPointsToCreate.IsActive = true;

                sOSSynopticTableofControlPointsToCreate.SOSHubId = SOSHubCollection_Id;

                SOSSynopticTableofControlPoints SynopticTableofControlPointsToCreate = _mapper.Map<SOSSynopticTableofControlPoints>(sOSSynopticTableofControlPointsToCreate);

                SynopticTableofControlPointsToCreate.Analyses = new List<SOSAnalysis>();
                SynopticTableofControlPointsToCreate.Sequences = new List<SOSSequence>();

                foreach (var sequence in sOSSynopticTableofControlPointsToCreate.Sequences)
                {
                    SOSSequence sequenceToAdd = await _SequenceRepository.GetSOSSequence(sequence.SOSSequenceId);
                    SynopticTableofControlPointsToCreate.Sequences.Add(sequenceToAdd);
                }

                foreach (var analysis in sOSSynopticTableofControlPointsToCreate.Analyses)
                {
                    SOSAnalysis analysisToAdd = await _AnalisisRepository.GetSOSAnalysis(analysis.SOSAnalysisId);
                    SynopticTableofControlPointsToCreate.Analyses.Add(analysisToAdd);
                }


                var createdResult = await _SynopticTableRepository.CreateSOSSynopticTableofControlPoints(SynopticTableofControlPointsToCreate);
                if (createdResult != null)
                    return Ok(SynopticTableofControlPointsToCreate);
                else
                    return BadRequest();
            }
            else
            {
                //only add revision
                SOSSynopticTableofControlPoints _sosSynopticTableofControlPoints = await _SynopticTableRepository.GetSOSSynopticTableofControlPoints(sOSSynopticTableofControlPointsToCreate.SOSSynopticTableofControlPointsId, true, true, true);

                SOSSynopticPointsLogbook _logbookToCreate = _mapper.Map<SOSSynopticPointsLogbook>(sOSSynopticTableofControlPointsToCreate.SynopticPointsLogbooks?.Last());
                _logbookToCreate.SOSSynopticTableofControlPointsId = _sosSynopticTableofControlPoints.SOSSynopticTableofControlPointsId;

                var resultAddSections = await _SynopticTableRepository.CreateSOSSynopticPointsLogbook(_logbookToCreate);

                if (resultAddSections > 0)
                {
                    Debug.WriteLine("SOSSynopticPointsLogbook añadidas con exito");
                    await _SynopticTableRepository.AddSOSSynopticPointsLogbookToSOSSynopticTableofControlPoints(_sosSynopticTableofControlPoints, _logbookToCreate);
                }
                else
                {
                    Debug.WriteLine("Error Sections añadidos");
                    return BadRequest();
                }



                return Ok("Revision");
            }

        }

        [HttpGet("{id}", Name = "GetSOSSynopticTableofControlPoints")]
        public async Task<ActionResult<SOSSynopticControlPointsDto>> GetSOSSynopticTableofControlPoints(int id, bool includeLogbooks = false, bool includeSOS = false, bool includeCollections = false)
        {

            var SOSSynopticTableofControlPoints = await _SynopticTableRepository.GetSOSSynopticTableofControlPoints(id, includeLogbooks, includeSOS, includeCollections);
            if (SOSSynopticTableofControlPoints == null)
            {
                return NotFound("SOSSynopticTableofControlPoints not found!");
            }

            return Ok(_mapper.Map<SOSSynopticControlPointsDto>(SOSSynopticTableofControlPoints));
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<SOSSynopticControlPointsDto>>> GetAllSOSSynopticTableofOperatingControlPoints(bool includeLogbooks = false, bool includeSOS = false, bool includeCollections = false)
        {

            var CheckpointEntities = await _SynopticTableRepository.GetAllSOSSynopticTableofControlPoints(includeLogbooks, includeSOS, includeCollections);
            if (CheckpointEntities == null)
            {
                return NotFound("Get All Sos Analisis not found!");
            }

            return Ok(_mapper.Map<IEnumerable<SOSSynopticControlPointsDto>>(CheckpointEntities));

        }

        [HttpDelete("{SOSSynopticTableofControlPointsId}")]
        public async Task<ActionResult<int>> RemoveSOSSynopticTableofControlPoints(int SOSSynopticTableofControlPointsId)
        {
            var result = await _SynopticTableRepository.RemoveSOSSynopticTableofControlPoints(SOSSynopticTableofControlPointsId);

            if (result > 0)
                return Ok();
            else
                return BadRequest("something wrong");
        }
    }
}
