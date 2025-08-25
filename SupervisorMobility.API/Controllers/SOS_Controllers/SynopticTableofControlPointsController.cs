using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Services;
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
        public SynopticTableofControlPointsController(IWebHostEnvironment env, IMapper mapper, ISOS_ProcessRepository repository)
        {
            _ProcessRepository = repository;
            _mapper = mapper ??
                  throw new ArgumentNullException(nameof(mapper));
            _env = env ?? throw new ArgumentNullException(nameof(env));
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
                    SOSSequence sequenceToAdd = await _ProcessRepository.GetSOSSequence(sequence.SOSSequenceId);
                    SynopticTableofControlPointsToCreate.Sequences.ToList().Add(sequenceToAdd);
                }

                foreach (var analysis in sOSSynopticTableofControlPointsToCreate.Analyses)
                {
                    SOSAnalysis analysisToAdd = await _ProcessRepository.GetSOSAnalysis(analysis.SOSAnalysisId);
                    SynopticTableofControlPointsToCreate.Analyses.ToList().Add(analysisToAdd);
                }


                var createdResult = await _ProcessRepository.CreateSOSSynopticTableofControlPoints(SynopticTableofControlPointsToCreate);
                if (createdResult != null)
                    return Ok(SynopticTableofControlPointsToCreate);
                else
                    return BadRequest();
            }
            else
            {
                //only add revision
                SOSSynopticTableofControlPoints _sosSynopticTableofControlPoints = await _ProcessRepository.GetSOSSynopticTableofControlPoints(sOSSynopticTableofControlPointsToCreate.SOSSynopticTableofControlPointsId, true, true, true);

                SOSSynopticPointsLogbook _logbookToCreate = _mapper.Map<SOSSynopticPointsLogbook>(sOSSynopticTableofControlPointsToCreate.SynopticPointsLogbooks?.Last());
                _logbookToCreate.SOSSynopticTableofControlPointsId = _sosSynopticTableofControlPoints.SOSSynopticTableofControlPointsId;

                var resultAddSections = await _ProcessRepository.CreateSOSSynopticPointsLogbook(_logbookToCreate);

                if (resultAddSections > 0)
                {
                    Debug.WriteLine("SOSSynopticPointsLogbook añadidas con exito");
                    await _ProcessRepository.AddSOSSynopticPointsLogbookToSOSSynopticTableofControlPoints(_sosSynopticTableofControlPoints, _logbookToCreate);
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

            var SOSSynopticTableofControlPoints = await _ProcessRepository.GetSOSSynopticTableofControlPoints(id, includeLogbooks, includeSOS, includeCollections);
            if (SOSSynopticTableofControlPoints == null)
            {
                return NotFound("SOSSynopticTableofControlPoints not found!");
            }

            return Ok(_mapper.Map<SOSSynopticControlPointsDto>(SOSSynopticTableofControlPoints));
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<SOSSynopticControlPointsDto>>> GetAllSOSSynopticTableofOperatingControlPoints(bool includeLogbooks = false, bool includeSOS = false, bool includeCollections = false)
        {

            var CheckpointEntities = await _ProcessRepository.GetAllSOSSynopticTableofControlPoints(includeLogbooks, includeSOS, includeCollections);
            if (CheckpointEntities == null)
            {
                return NotFound("Get All Sos Analisis not found!");
            }

            return Ok(_mapper.Map<IEnumerable<SOSSynopticControlPointsDto>>(CheckpointEntities));

        }
    }
}
