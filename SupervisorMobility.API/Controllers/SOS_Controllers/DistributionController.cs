// - Core .NET imports
using System.Diagnostics;
using System.Collections.Generic;

// - External imports
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

// - Context imports
using SupervisorMobility.API.DataAccess.Services;

// - Entity imports
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities;

// - Interface imports
using SupervisorMobility.API.Interfaces.SOS;

// - Service imports
using SupervisorMobility.API.Services.SOS;

// - Model imports
using SupervisorMobility.API.Models.SOS.SOSDistributionOperationSequenceDtos;
using SupervisorMobility.API.Models.SOS.SOSDistributionAdditionalTimeDtos;
using SupervisorMobility.API.Models.SOS.SOSDistributionLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos.SectionDtos;
using SupervisorMobility.API.Models.SOS.SOSDistributionDtos;
using SupervisorMobility.API.Models.SOS.SOSTimeDtos;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.SOS.TurnDtos;
using SupervisorMobility.API.DataAccess.Services.SOS_DistributionRepository;
using SupervisorMobility.API.DataAccess.Services.SOS_SequenceRepository;
using SupervisorMobility.API.DataAccess.Services.SOS_AnalysisRepository;


namespace SupervisorMobility.API.Controllers.SOS_Controllers
{
    [Route("api/SOS/Distribution")]
    [ApiController]
    public class DistributionController : ControllerBase
    {
        // +=============== DEPENDENCIES ===============+\\
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly ISOS_ProcessRepository _ProcessRepository;
        private readonly ISOS_DistributionRepository _DistributionRepository;
        private readonly ISOS_SequenceRepository _SequenceRepository;
        private readonly ISOS_AnalysisRepository _AnalysisRepository;
        private readonly ISTROSyncDistributionService _STROSyncDistributionService;

        /// <summary>
        /// Initializes dependencies required by the DistributionController.
        /// </summary>
        /// <param name="env">Provides information about the web hosting environment.</param>
        /// <param name="mapper">AutoMapper instance for DTO-to-entity mapping.</param>
        /// <param name="repository">Repository for handling SOS process operations.</param>
        /// <param name="STROSyncDistributionService">Service for synchronizing SOS distribution sequences.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="mapper"/> or <paramref name="env"/> is null.
        /// </exception>
        public DistributionController(IWebHostEnvironment env, IMapper mapper, ISOS_ProcessRepository repository, ISTROSyncDistributionService STROSyncDistributionService, ISOS_DistributionRepository distributionRepository, ISOS_AnalysisRepository analysisRepository, ISOS_SequenceRepository sequenceRepository)
        {
            _ProcessRepository = repository;
            _mapper = mapper ??throw new ArgumentNullException(nameof(mapper));
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _STROSyncDistributionService = STROSyncDistributionService;
            _DistributionRepository = distributionRepository;
            _SequenceRepository = sequenceRepository;
            _AnalysisRepository = analysisRepository;
        }

        // +============ ROUTES / ENDPOINTS ============+\\

        /// <summary>
        /// Creates a new SOS Distribution or adds a revision to an existing one.
        /// </summary>
        /// <param name="sOSDistributionToCreate">Data for the distribution to create or update.</param>
        /// <param name="SOSHubCollection_Id">ID of the SOS Hub Collection.</param>
        /// <returns>
        /// Created distribution data or revision confirmation message.
        /// </returns>
        /// <response code="200">Operation succeeded.</response>
        /// <response code="400">Invalid input or failed operation.</response>
        [HttpPost]
        public async Task<ActionResult<SOSDistributionDto>> GenerateDistribution(SOSDistributionForCreateDto sOSDistributionToCreate, int SOSHubCollection_Id)
        {

            // ============ CREATE NEW DISTRIBUTION =============\\
            if (sOSDistributionToCreate.SOSDistributionId == 0)
            {
                //Nombre del documento GOS o processShet
                //sOSDistributionToCreate.InternalControlNumber = SOSEntity.Folio;
                //sOSDistributionToCreate.ProcessName = SOSEntity.ProcessSheet;

                // NOTE: Setting metadata for creation
                sOSDistributionToCreate.CreatedAt = DateTime.Now;
                sOSDistributionToCreate.IsActive = true;

                sOSDistributionToCreate.SOSHubId = SOSHubCollection_Id;
                // NOTE: Replace with logic to add SOS hubs from selected analyses and sequences

                // NOTE: Ensure additional time details are initialized if missing
                if (sOSDistributionToCreate.SOSDistributionAdditionalTime == null)
                {
                    sOSDistributionToCreate.SOSDistributionAdditionalTime = new SOSDistributionAdditionalTime
                    {
                        TakeQuantity = "§§§§",
                        TakeTime = "§§§§§",
                        LeaveQuantity = "§§§§",
                        LeaveTime = "§§§§§",
                        StepsQuantity = "§§§§",
                        StepsTime = "§§§§§",
                        IsActive = true
                    };
                }

                // NOTE: Map DTO to entity before creation
                SOSDistribution DistributionToCreate = _mapper.Map<SOSDistribution>(sOSDistributionToCreate);

                // NOTE: Create new distribution in repository
                var createdResult = await _DistributionRepository.CreateSOSDistribution(DistributionToCreate);
                if (createdResult != null)
                    return Ok(DistributionToCreate);
                else
                    return BadRequest();
            }
            else
            {
                // NOTE: Fetch existing distribution with all related data
                SOSDistribution _sosDistribution = await _DistributionRepository.GetSOSDistribution(sOSDistributionToCreate.SOSDistributionId, true, true, true, true);

                // NOTE: Map the last logbook entry from DTO
                SOSDistributionLogbook _logbookToCreate = _mapper.Map<SOSDistributionLogbook>(sOSDistributionToCreate.DistributionLogbooks?.Last());
                _logbookToCreate.SOSDistributionId = _sosDistribution.SOSDistributionId;

                // NOTE: Add logbook entry to repository
                var resultAddSections = await _DistributionRepository.CreateSOSDistributionLogbook(_logbookToCreate);

                if (resultAddSections > 0)
                {
                    // NOTE: Link logbook entry to distribution
                    await _DistributionRepository.AddSOSDistributionLogbookToSOSDistribution(_sosDistribution, _logbookToCreate);
                }
                else
                {
                    return BadRequest();
                }



                return Ok("Revision");
            }

        }


        /// <summary>
        /// Retrieves a specific SOS Distribution by its ID, with optional related data.
        /// </summary>
        /// <param name="id">Identifier of the SOS Distribution to retrieve.</param>
        /// <param name="includeImages">Include associated images if true.</param>
        /// <param name="includeNotes">Include associated notes if true.</param>
        /// <param name="includeLogbooks">Include related logbooks if true.</param>
        /// <param name="includeSOS">Include related SOS data if true.</param>
        /// <param name="includeImagesSOS">Include SOS-related images if true.</param>
        /// <param name="includeTurns">Include associated turns if true.</param>
        /// <param name="includeTimes">Include associated time details if true.</param>
        /// <param name="includeCollections">Include related collections if true.</param>
        /// <returns>Returns the SOS Distribution with optional related data.</returns>
        /// <response code="200">Returns the SOS Distribution data.</response>
        /// <response code="404">If the SOS Distribution is not found.</response>
        [HttpGet("{id}", Name = "GetSOSDistribution")]
        public async Task<ActionResult<SOSDistributionDto>> GetSOSDistribution(int id, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSOS = false, bool includeImagesSOS = false, bool includeTurns = false, bool includeTimes = false, bool includeCollections = false)
        {
            // NOTE: Retrieve the SOS distribution with requested related data
            var SOSDistribution = await _DistributionRepository.GetSOSDistribution(id, includeImages, includeNotes, includeLogbooks, includeSOS, includeImagesSOS, includeTurns, includeTimes, includeCollections: includeCollections);

            // NOTE: Return 404 if distribution is not found
            if (SOSDistribution == null) { return NotFound("SOSDistribution not found!"); }

            // NOTE: Map entity to DTO for return
            var mappedDto = _mapper.Map<SOSDistributionDto>(SOSDistribution);
            return Ok(mappedDto);
        }

        /// <summary>
        /// Retrieves the SOS Distribution associated with a specific SOS Hub ID,
        /// optionally including related data.
        /// </summary>
        /// <param name="idSOSHub">Identifier of the SOS Hub.</param>
        /// <param name="includeImages">Include associated images if true.</param>
        /// <param name="includeNotes">Include associated notes if true.</param>
        /// <param name="includeLogbooks">Include related logbooks if true.</param>
        /// <param name="includeSOS">Include related SOS data if true.</param>
        /// <param name="includeImagesSOS">Include SOS-related images if true.</param>
        /// <param name="includeTurns">Include associated turns if true.</param>
        /// <param name="includeTimes">Include associated time details if true.</param>
        /// <param name="includeCollections">Include related collections if true.</param>
        /// <returns>Returns the SOS Distribution linked to the given SOS Hub.</returns>
        /// <response code="200">Returns the SOS Distribution data.</response>
        /// <response code="404">If no distribution is found for the given SOS Hub ID.</response>
        [HttpGet("bySosHub/{idSOSHub}", Name = "GetDistributionBySOSHub")]
        public async Task<ActionResult<SOSDistributionDto>> GetDistributionBySOSHub(int idSOSHub, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSOS = false, bool includeImagesSOS = false, bool includeTurns = false, bool includeTimes = false, bool includeCollections = false)
        {
            // NOTE: Retrieve the distribution ID associated with the SOS Hub
            var idDistribution = await _DistributionRepository.GetIdDistributionBySosHub(idSOSHub);

            // NOTE: Retrieve the distribution with requested related data
            var SOSDistribution = await _DistributionRepository.GetSOSDistribution(idDistribution, includeImages, includeNotes, includeLogbooks, includeSOS, includeImagesSOS, includeTurns, includeTimes, includeCollections: includeCollections);

            // NOTE: Return 404 if distribution is not found
            if (SOSDistribution == null) return NotFound("SOSDistribution not found!");

            // NOTE: Map entity to DTO for return
            var mappedDto = _mapper.Map<SOSDistributionDto>(SOSDistribution);
            return Ok(mappedDto);
        }


        /// <summary>
        /// Retrieves all SOS Distributions, optionally including related data.
        /// </summary>
        /// <param name="includeImages">Include associated images if true.</param>
        /// <param name="includeNotes">Include associated notes if true.</param>
        /// <param name="includeLogbooks">Include related logbooks if true.</param>
        /// <param name="includeSOS">Include related SOS data if true.</param>
        /// <returns>Returns a list of SOS Distributions with optional related data.</returns>
        /// <response code="200">Returns the list of SOS Distributions.</response>
        /// <response code="404">If no SOS Distributions are found.</response>
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<SOSDistributionDto>>> GetAllSOSDistribution(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSOS = false)
        {
            // NOTE: Retrieve all distributions with requested related data
            var CheckpointEntities = await _DistributionRepository.GetAllSOSDistribution(includeImages, includeNotes, includeLogbooks, includeSOS);

            // NOTE: Return 404 if no distributions found
            if (CheckpointEntities == null) return NotFound("Get All Sos Analisis not found!");

            // NOTE: Map entity collection to DTO collection
            return Ok(_mapper.Map<IEnumerable<SOSDistributionDto>>(CheckpointEntities));
        }

        //Update

        [HttpPut("{sosDistribution_Id}")]
        public async Task<ActionResult> UpdateSOSDistribution(int sosDistribution_Id, SOSDistributionForUpdateDto sosUpdateEntity)
        {
            List<SOSAnalysis> Bkup_Analysis = new List<SOSAnalysis>();
            List<SOSSequence> Bkup_Sequence = new List<SOSSequence>();

            List<Commentary> Bkup_Notes = new List<Commentary>();
            List<Turn> Bkup_Turn = new List<Turn>();
            List<SOSDistributionLogbook> Bkup_DistributionLogbook = new List<SOSDistributionLogbook>();
            List<SOSDistributionOperationSequence> Backup_DistributionOperationSequence = new List<SOSDistributionOperationSequence>();
            SOSDistributionAdditionalTime additionalTime = new SOSDistributionAdditionalTime();

            // Filtrar nuevos Comentarios
            List<UpdateCommentaryDto> filteredCommentaryList = sosUpdateEntity.Notes.Where(t => t.CommentaryId <= 0).ToList();
            // Filtrar nuevos DistributionLogbooks
            List<SOSDistributionLogbookForUpdateDto> filteredDistributionLogbooksList = sosUpdateEntity.DistributionLogbooks.Where(t => t.SOSDistributionLogbookId <= 0).ToList();
            // Filtrar nuevos Tiempos
            List<SOSDistributionOperationSequenceForUpdateDto> filteredSOSOperationSequenceList = sosUpdateEntity.SOSDistributionOperationSequence.Where(t => t.SOSDistributionOperationSequenceId <= 0).ToList();
            // Filtrar nuevos Turnos
            List<TurnForUpdateDto> filteredTurnList = sosUpdateEntity.Turns.Where(t => t.TurnId <= 0).ToList();


            // Remover nuevos Comentarios de la lista principal para evitar duplicados
            if (filteredCommentaryList.Any())
            {
                sosUpdateEntity.Notes.RemoveAll(t => t.CommentaryId == null || t.CommentaryId <= 0);

                // Mapear nuevas norms/standars
                List<Commentary> newCommentarys = _mapper.Map<List<Commentary>>(filteredCommentaryList);

                foreach (var newComentary in newCommentarys)
                {
                    newComentary.CommentaryId = 0;
                    newComentary.IsActive = true;
                }

                var resultAddCommentary = await _ProcessRepository.AddRangeCommentary(newCommentarys);

                if (resultAddCommentary != null)
                {
                    Debug.WriteLine("Commentarios añadidos con exitop");
                    Bkup_Notes.AddRange(resultAddCommentary);
                }
                else
                {
                    Debug.WriteLine("Error Commentarios añadidos");
                }
            }



            // Remover nuevos DistributionLogbooks de la lista principal para evitar duplicados
            if (filteredDistributionLogbooksList.Any())
            {
                sosUpdateEntity.DistributionLogbooks.RemoveAll(t => t.SOSDistributionLogbookId == null || t.SOSDistributionLogbookId <= 0);

                // Mapear nuevas norms/standars
                List<SOSDistributionLogbook> newSOSDistributionLogbook = _mapper.Map<List<SOSDistributionLogbook>>(filteredDistributionLogbooksList);

                foreach (var DistributionLogbook in newSOSDistributionLogbook)
                {
                    DistributionLogbook.SOSDistributionLogbookId = 0;
                    DistributionLogbook.IsActive = true;
                }

                var resultAddSOSDistributionLogbook = await _DistributionRepository.AddRangeSOSDistributionLogbook(newSOSDistributionLogbook);

                if (resultAddSOSDistributionLogbook != null)
                {
                    Debug.WriteLine("DistributionLogbooks añadidos con exitop");
                    Bkup_DistributionLogbook.AddRange(resultAddSOSDistributionLogbook);
                }
                else
                {
                    Debug.WriteLine("Error DistributionLogbooks añadidos");
                }
            }

            //aqui añadiremos los nuevos sequencias
            if (filteredSOSOperationSequenceList.Any())
            {
                sosUpdateEntity.SOSDistributionOperationSequence.RemoveAll(t => t.SOSDistributionOperationSequenceId == null || t.SOSDistributionOperationSequenceId <= 0);

                // Mapear nuevas tiempos
                List<SOSDistributionOperationSequence> newSOSTime = _mapper.Map<List<SOSDistributionOperationSequence>>(filteredSOSOperationSequenceList);

                foreach (var time in newSOSTime)
                {
                    time.SOSDistributionOperationSequenceId = 0;
                    time.IsActive = true;
                }

                var resultAddSOSTime = await _DistributionRepository.AddRangeSOSDistributionOperationSequences(newSOSTime);


                if (resultAddSOSTime != null)
                {
                    Debug.WriteLine("Add SOSTime añadidos con exitop");
                    Backup_DistributionOperationSequence.AddRange(resultAddSOSTime);
                }
                else
                {
                    Debug.WriteLine("Error Add SOSTime añadidos");
                }
            }

            //Turnos
            if (filteredTurnList.Any())
            {
                sosUpdateEntity.Turns.RemoveAll(t => t.TurnId == null || t.TurnId <= 0);

                // Mapear nuevas tiempos
                List<Turn> newTurn = _mapper.Map<List<Turn>>(filteredTurnList);

                foreach (var time in newTurn)
                {
                    time.TurnId = 0;
                    //time.IsActive = true;
                }

                var resultAddTurn = await _ProcessRepository.AddRangeTurns(newTurn);

                if (resultAddTurn != null)
                {
                    Debug.WriteLine("Add Turn añadidos con exito");
                    Bkup_Turn.AddRange(resultAddTurn);
                }
                else
                {
                    Debug.WriteLine("Error Add Turn añadidos");
                }
            }


            SOSDistribution _sosDistribution = await _DistributionRepository.GetSOSDistribution(sosDistribution_Id, true, true, true, true, includeTurns: true, includeTimes: true, includeCollections: true);

            //Aqui va el historico de ser necesario en  un futuro 

            //Ejemplo de uso 
            //Compare genera un string que menciona las diferencias
            //string jsonResult = CompareAndGenerateJson(_mapper.Map<SOSHubForUpdateDto>(entitySOSHub), _SOSHubForUpdate);
            //se crea un entity 
            //SOSHubHistory newHistory = new SOSHubHistory();
            //_mapper.Map(entitySOSHub, newHistory);
            //newHistory.VersionChanges = jsonResult;
            //se almacena la entity anterior y se le añade el resumen de cambios
            //await _ProcessRepository.CreateHistorySOScollection(newHistory);



            //Crear bkup de datos relacionados
            //hacer update entity sin relaciones

            foreach (var sequence in sosUpdateEntity.Sequences)
            {
                SOSSequence sequenceToAdd = await _SequenceRepository.GetSOSSequence(sequence.SOSSequenceId);
                Bkup_Sequence.Add(sequenceToAdd);
            }

            foreach (var analysis in sosUpdateEntity.Analyses)
            {
                SOSAnalysis analysisToAdd = await _AnalysisRepository.GetSOSAnalysis(analysis.SOSAnalysisId);
                Bkup_Analysis.Add(analysisToAdd);
            }


            foreach (var note in sosUpdateEntity.Notes)
            {
                var CommentaryUpdate = await _ProcessRepository.UpdateCommentary(note);

                Commentary CommentaryToAdd = await _ProcessRepository.GetCommentaryById(note.CommentaryId);
                Bkup_Notes.Add(CommentaryToAdd);
            }

            var AdditionalTimeUpdate = await _DistributionRepository.UpdateSOSDistributionAdditionalTime(sosUpdateEntity.SOSDistributionAdditionalTime);
            additionalTime = await _DistributionRepository.GetSOSDistributionAdditionalTimeId(sosUpdateEntity.SOSDistributionAdditionalTime.SOSDistributionAdditionalTimeId);

            foreach (var logbook in sosUpdateEntity.DistributionLogbooks)
            {
                var distributionUpdate = await _DistributionRepository.UpdateDistributionLogbook(logbook);
                SOSDistributionLogbook distributionBkaux = await _DistributionRepository.GetSOSDistributionLogbookById(logbook.SOSDistributionLogbookId);
                Bkup_DistributionLogbook.Add(distributionBkaux);
            }

            //Update - Process ALL sequences from the request, not just new ones
            List<SOSDistributionOperationSequence> OperationSequencesDelete = new List<SOSDistributionOperationSequence>();
            List<SOSDistributionOperationSequenceForUpdateDto> OperationSequencesUpdate = new List<SOSDistributionOperationSequenceForUpdateDto>();

            // Get all existing sequences
            var existingSequences = _sosDistribution.SOSDistributionOperationSequence?.ToList() ?? new List<SOSDistributionOperationSequence>();

            // Get all sequences from the request (both new and existing)
            var requestSequences = sosUpdateEntity.SOSDistributionOperationSequence?.ToList() ?? new List<SOSDistributionOperationSequenceForUpdateDto>();

            foreach (var existingSequence in existingSequences)
            {
                var findOperation = requestSequences.FirstOrDefault(a => a.SOSDistributionOperationSequenceId == existingSequence.SOSDistributionOperationSequenceId);
                if (findOperation == null)
                {
                    // Sequence not in request - mark for deletion
                    OperationSequencesDelete.Add(existingSequence);
                }
                else
                {
                    // Sequence found in request - mark for update
                    OperationSequencesUpdate.Add(findOperation);
                }
            }

            foreach (var operationsequence in OperationSequencesUpdate)
            {
                var operationsequenceUpdate = await _DistributionRepository.UpdateSOSDistributionOperationSequences(operationsequence);
                SOSDistributionOperationSequence timeBkaux = await _DistributionRepository.GetSOSDistributionOperationSequencesById(operationsequence.SOSDistributionOperationSequenceId);
                Backup_DistributionOperationSequence.Add(timeBkaux);
            }

            foreach (var operationsequence in OperationSequencesDelete)
            {
                await _DistributionRepository.DeleteSOSDistributionOperationSequencesById(operationsequence.SOSDistributionOperationSequenceId);
            }

            foreach (var turn in sosUpdateEntity.Turns)
            {
                var turnUpdate = await _ProcessRepository.UpdateTurn(turn);
                Turn turnBkaux = await _ProcessRepository.GetTurnById(turn.TurnId);
                Bkup_Turn.Add(turnBkaux);
            }

            var hubIdsFromAnalyses = Bkup_Analysis.Select(a => a.SOSHubId);

            var hubIdsFromSequences = Bkup_Sequence.Select(s => s.SOSHubId);

            var allHubIds = hubIdsFromAnalyses.Concat(hubIdsFromSequences).Distinct().ToList();
            var hubsToAssociate = new List<SOSHub>();
            foreach (var hubId in allHubIds)
            {
                var hub = await _ProcessRepository.GetSOSHub(hubId);
                if (hub != null)
                    hubsToAssociate.Add(hub);
            }

            //if (_sosDistribution.SOSHubs == null)
            //    _sosDistribution.SOSHubs = new List<SOSHub>();

            // Store the sequence ordering information from the request before nullifying
            Dictionary<int, int> sequenceOrderMap = new Dictionary<int, int>();
            if (sosUpdateEntity.SOSDistributionOperationSequence != null)
            {
                foreach (var opSeq in sosUpdateEntity.SOSDistributionOperationSequence)
                {
                    if (opSeq.SOSDistributionOperationSequenceId > 0 && opSeq.SequenceId.HasValue)
                    {
                        // Use the sequenceId from the request as-is (it represents the desired order)
                        sequenceOrderMap[opSeq.SOSDistributionOperationSequenceId] = opSeq.SequenceId.Value;
                    }
                }

                // Debug: Log what we captured
                System.Diagnostics.Debug.WriteLine($"Captured sequence mappings: {string.Join(", ", sequenceOrderMap.Select(kvp => $"ID{kvp.Key}=>{kvp.Value}"))}");
            }

            //Nulleamos el update para evitar errores
            sosUpdateEntity.SOSHubs = null;
            sosUpdateEntity.Sequences = null;
            sosUpdateEntity.Analyses = null;

            sosUpdateEntity.Notes = null;
            sosUpdateEntity.Turns = null;
            sosUpdateEntity.DistributionLogbooks = null;
            sosUpdateEntity.SOSDistributionOperationSequence = null;
            sosUpdateEntity.SOSDistributionAdditionalTime = null;

            await _DistributionRepository.SOSDataRemoveAllSequencesFromSOSDistribution(_sosDistribution);
            await _DistributionRepository.SOSDataRemoveAllAnalysisFromSOSDistribution(_sosDistribution);

            await _ProcessRepository.RemoveAllTurnsFromSOSDistribution(_sosDistribution);
            await _DistributionRepository.SOSDataRemoveAllNotesFromSOSDistribution(_sosDistribution);
            await _DistributionRepository.SOSDataRemoveAllSOSDistributionLogbookFromSOSDistribution(_sosDistribution);
            await _DistributionRepository.SOSDataRemoveAllSOSDistributionAdditionalTimeFromSOSDistribution(_sosDistribution);

            await _DistributionRepository.SOSDataRemoveAllSOSHubsFromSOSDistribution(_sosDistribution);

            var result = await _DistributionRepository.UpdateSOSDistribution(sosUpdateEntity, _sosDistribution);




            //Notes - Volver a añádir las notas
            if (Bkup_Analysis.Any())
            {
                foreach (SOSAnalysis Analysis in Bkup_Analysis)
                {
                    await _DistributionRepository.AddAnalysisToSOSDistribution(_sosDistribution, Analysis);
                }
            }
            if (Bkup_Sequence.Any())
            {
                foreach (SOSSequence Sequence in Bkup_Sequence)
                {
                    await _DistributionRepository.AddSequenceToSOSDistribution(_sosDistribution, Sequence);
                }
            }



            if (Bkup_Notes.Any())
            {
                foreach (Commentary Comment in Bkup_Notes)
                {
                    await _DistributionRepository.AddNoteToSOSDistribution(_sosDistribution, Comment);
                }
            }

            //Distribution Logbook
            if (Bkup_DistributionLogbook.Any())
            {
                foreach (SOSDistributionLogbook logbook in Bkup_DistributionLogbook)
                {
                    await _DistributionRepository.AddSOSDistributionLogbookToSOSDistribution(_sosDistribution, logbook);
                }
            }

            //Times - Apply updated sequence ordering
            if (Backup_DistributionOperationSequence.Any())
            {
                // Debug: Log the sequence order map
                System.Diagnostics.Debug.WriteLine($"Sequence Order Map: {string.Join(", ", sequenceOrderMap.Select(kvp => $"{kvp.Key}={kvp.Value}"))}");

                // Sort backup sequences by their new order to ensure proper insertion sequence
                var orderedBackupSequences = Backup_DistributionOperationSequence
                    .OrderBy(ops => sequenceOrderMap.ContainsKey(ops.SOSDistributionOperationSequenceId)
                        ? sequenceOrderMap[ops.SOSDistributionOperationSequenceId]
                        : int.MaxValue)
                    .ToList();

                foreach (SOSDistributionOperationSequence operationSequence in orderedBackupSequences)
                {
                    var oldSequenceId = operationSequence.SequenceId;

                    // Apply the updated sequence order if it was provided in the request
                    if (sequenceOrderMap.ContainsKey(operationSequence.SOSDistributionOperationSequenceId))
                    {
                        operationSequence.SequenceId = sequenceOrderMap[operationSequence.SOSDistributionOperationSequenceId];
                        System.Diagnostics.Debug.WriteLine($"Updated sequence {operationSequence.SOSDistributionOperationSequenceId}: {oldSequenceId} -> {operationSequence.SequenceId}");
                    }

                    await _DistributionRepository.AddOperationSequenceToSOSDistribution(_sosDistribution, operationSequence);
                }
            }

            //turns
            if (Bkup_Turn.Any())
            {
                foreach (Turn turn in Bkup_Turn)
                {
                    await _ProcessRepository.AddTurnToSOSDistribution(_sosDistribution, turn);
                }
            }

            await _DistributionRepository.AddSOSDistributionAdditionalTimeToSOSDistribution(_sosDistribution, additionalTime);


            if (hubsToAssociate.Any())
            {
                foreach (SOSHub hub in hubsToAssociate)
                {
                    if (_sosDistribution.SOSHubs.Any(h => h.SOSHubId == hub.SOSHubId))
                        await _DistributionRepository.AddSOSHubToSOSDistribution(_sosDistribution, hub);
                }
            }

            // aqui me falta una verificacion para que se añada la relacioncon los soshubs
            // si hay soscombinationoperations o analisis o secuencia debe haber 1 sos hub



            if (_sosDistribution.SOSHubs == null || !_sosDistribution.SOSHubs.Any())
            {
                int? hubId =
                    Bkup_Analysis.FirstOrDefault()?.SOSHubId ??
                    Bkup_Sequence.FirstOrDefault()?.SOSHubId ??
                    Backup_DistributionOperationSequence
                        .Select(op =>
                        {

                            return (int?)null;
                        })
                        .FirstOrDefault(id => id.HasValue);

                if (hubId.HasValue)
                {
                    var hub = await _ProcessRepository.GetSOSHub(hubId.Value);
                    if (hub != null)
                    {
                        await _DistributionRepository.AddSOSHubToSOSDistribution(_sosDistribution, hub);
                    }
                }
            }

            var isUpdateSTROSequencesRelated = await _STROSyncDistributionService.SyncDistributionsWithSTROs(_sosDistribution.SOSDistributionId);
            if (!isUpdateSTROSequencesRelated) throw new Exception("STRO Sequences not sync");

            if (result != null)
            {
                return Ok(_sosDistribution);
            }
            else
                return BadRequest();

        }//end Update 

        [HttpDelete("{SOSDistributionId}")]
        public async Task<ActionResult<int>> RemoveSOSHub(int SOSDistributionId)
        {
            var result = await _DistributionRepository.RemoveSOSDistribution(SOSDistributionId);

            if (result > 0)
                return Ok();
            else
                return BadRequest("something wrong");
        }


        //ilustrations

        [HttpPost("Ilustrations/{Distribution_id}")]
        public async Task<ActionResult<FileUpload>> UploadIlustrations(int Distribution_id, IFormFile file)
        {

            var uploadResult = new FileUploadForCreationDto();
            string trustedFileNameForStorage = string.Empty;
            var unstrustedFileName = file.FileName;

            trustedFileNameForStorage = Path.GetRandomFileName();

            var path = Path.Combine(_env.ContentRootPath, "uploads\\SOSDistribution\\Ilustrations", trustedFileNameForStorage);
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

            var fileToReturn = await _ProcessRepository.CreateFileAsync(uploadResult);

            await _DistributionRepository.AddIlustrationToSOSDistribution(Distribution_id, fileToReturn);
            await _ProcessRepository.SaveChangesAsync();

            return Ok(fileToReturn);
        }

        [HttpGet("Ilustrations/{fileid}")]
        public async Task<IActionResult> DownloadIlustrations(int fileid)
        {
            var FileInfo = await _ProcessRepository.FetchFileAsync(fileid);

            if (FileInfo is not null)
            {
                var path = Path.Combine(_env.ContentRootPath, "uploads\\SOSDistribution\\Ilustrations", FileInfo.StorageFileName);

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

        [HttpDelete("Ilustrations/{SOS_SOSDistribution_id}/remove/{ImageFile_id}")]
        public async Task<ActionResult<int>> RemoveImage(int SOS_SOSDistribution_id, int ImageFile_id)
        {
            var result = await _DistributionRepository.RemoveIlustrationFromSOSDistribution(SOS_SOSDistribution_id, ImageFile_id);

            if (result > 0)
                return Ok();
            else
                return BadRequest("something went wrong");
        }

    }
}
