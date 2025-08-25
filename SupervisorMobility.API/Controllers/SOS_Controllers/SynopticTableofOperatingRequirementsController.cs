// ====================== CORE / SYSTEM IMPORTS ====================== //
using System.Diagnostics;

// ====================== MICROSOFT / FRAMEWORK ====================== //
using Microsoft.AspNetCore.Mvc;

// ====================== THIRD-PARTY LIBRARIES ====================== //
using AutoMapper;

// ======================= DATA ACCESS IMPORTS ======================= //
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Services;

// ============================ INTERFACES =========================== //
using SupervisorMobility.API.Interfaces.SOS;

// ========================== MODELS / DTOs ========================== //
using SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsOperationSequenceDtos;
using SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos.SectionDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.FileUploadDto;


namespace SupervisorMobility.API.Controllers.SOS_Controllers
{
    [Route("api/SOS/SynopticTableofOperatingRequirements")]
    [ApiController]
    public class SynopticTableofOperatingRequirementsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly ISOS_ProcessRepository _ProcessRepository;
        private readonly ISTOperatingRequirementsService _STOperatingRequirementsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SynopticTableofOperatingRequirementsController"/> class.
        /// </summary>
        /// <param name="env">Provides information about the web hosting environment.</param>
        /// <param name="mapper">Automapper instance for mapping entities to DTOs and vice versa.</param>
        /// <param name="repository">Repository used to access SOS process data.</param>
        /// <param name="STOperatingRequirementsService">Service used to manage Synoptic Table of Operating Requirements (STRO) operations.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="mapper"/> or <paramref name="env"/> is null.</exception>
        public SynopticTableofOperatingRequirementsController(IWebHostEnvironment env, IMapper mapper, ISOS_ProcessRepository repository, ISTOperatingRequirementsService STOperatingRequirementsService)
        {
            _ProcessRepository = repository;
            _STOperatingRequirementsService = STOperatingRequirementsService;
            _mapper = mapper ??
                  throw new ArgumentNullException(nameof(mapper));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        [HttpPost]
        public async Task<ActionResult<SOSSynopticRequirementsDto>> GenerateSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirementsForCreateDto sOSSynopticTableofOperatingRequirementsToCreate, int SOSHubCollection_Id)
        {

            if (sOSSynopticTableofOperatingRequirementsToCreate.SOSSynopticTableofOperatingRequirementsId == 0)
            {
                sOSSynopticTableofOperatingRequirementsToCreate.CreatedAt = DateTime.Now;
                sOSSynopticTableofOperatingRequirementsToCreate.IsActive = true;

                sOSSynopticTableofOperatingRequirementsToCreate.SOSHubId = SOSHubCollection_Id;

                SOSSynopticTableofOperatingRequirements SynopticTableofOperatingRequirementsToCreate = _mapper.Map<SOSSynopticTableofOperatingRequirements>(sOSSynopticTableofOperatingRequirementsToCreate);

                SynopticTableofOperatingRequirementsToCreate.Analyses = new List<SOSAnalysis>();
                SynopticTableofOperatingRequirementsToCreate.Sequences = new List<SOSSequence>();

                foreach (var sequence in sOSSynopticTableofOperatingRequirementsToCreate.Sequences)
                {
                    SOSSequence sequenceToAdd = await _ProcessRepository.GetSOSSequence(sequence.SOSSequenceId);
                    SynopticTableofOperatingRequirementsToCreate.Sequences.ToList().Add(sequenceToAdd);
                }

                foreach (var analysis in sOSSynopticTableofOperatingRequirementsToCreate.Analyses)
                {
                    SOSAnalysis analysisToAdd = await _ProcessRepository.GetSOSAnalysis(analysis.SOSAnalysisId);
                    SynopticTableofOperatingRequirementsToCreate.Analyses.ToList().Add(analysisToAdd);
                }


                var createdResult = await _ProcessRepository.CreateSOSSynopticTableofOperatingRequirements(SynopticTableofOperatingRequirementsToCreate);
                if (createdResult != null)
                    return Ok(SynopticTableofOperatingRequirementsToCreate);
                else
                    return BadRequest();
            }
            else
            {
                //only add revision
                SOSSynopticTableofOperatingRequirements _sosSynopticTableofOperatingRequirements = await _ProcessRepository.GetSOSSynopticTableofOperatingRequirements(sOSSynopticTableofOperatingRequirementsToCreate.SOSSynopticTableofOperatingRequirementsId, true, true, true);

                SOSSynopticRequirementsLogbook _logbookToCreate = _mapper.Map<SOSSynopticRequirementsLogbook>(sOSSynopticTableofOperatingRequirementsToCreate.SynopticRequirementsLogbooks?.Last());
                _logbookToCreate.SOSSynopticRequirementsLogbookId = _sosSynopticTableofOperatingRequirements.SOSSynopticTableofOperatingRequirementsId;

                var resultAddSections = await _ProcessRepository.CreateSOSSynopticRequirementsLogbook(_logbookToCreate);

                if (resultAddSections > 0)
                {
                    Debug.WriteLine("SOSSynopticRequirementsLogbook añadidas con exito");
                    await _ProcessRepository.AddSOSSynopticRequirementsLogbookToSOSSynopticTableofOperatingRequirements(_sosSynopticTableofOperatingRequirements, _logbookToCreate);
                }
                else
                {
                    Debug.WriteLine("Error Sections añadidos");
                    return BadRequest();
                }



                return Ok("Revision");
            }

        }

        [HttpGet("{id}", Name = "GetSOSSynopticTableofOperatingRequirements")]
        public async Task<ActionResult<SOSSynopticRequirementsDto>> GetSOSSynopticTableofOperatingRequirements(int id, bool includeLogbooks = false, bool includeSOS = false, bool includeCollections = false)
        {

            var SOSSynopticTableofOperatingRequirements = await _ProcessRepository.GetSOSSynopticTableofOperatingRequirements(id, includeLogbooks, includeSOS, includeCollections);
            if (SOSSynopticTableofOperatingRequirements == null)
            {
                return NotFound("SOSSynopticTableofOperatingRequirements not found!");
            }

            return Ok(_mapper.Map<SOSSynopticRequirementsDto>(SOSSynopticTableofOperatingRequirements));
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<SOSSynopticRequirementsDto>>> GetAllSOSSynopticTableofOperatingRequirements(bool includeLogbooks = false, bool includeSOS = false, bool includeCollections = false)
        {

            var CheckpointEntities = await _ProcessRepository.GetAllSOSSynopticTableofOperatingRequirements(includeLogbooks, includeSOS, includeCollections);
            if (CheckpointEntities == null)
            {
                return NotFound("Get All Sos Analisis not found!");
            }

            return Ok(_mapper.Map<IEnumerable<SOSSynopticRequirementsDto>>(CheckpointEntities));

        }

        /// <summary>
        /// Generates an Excel file for the Synoptic Table of Operating Requirements (STOR)
        /// for the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The STOR record identifier.</param>
        /// <returns>A file result containing the Excel file named "STOR.xlsx".</returns>
        /// <response code="200">The Excel file was generated successfully.</response>
        /// <response code="400">If the provided <paramref name="id"/> is invalid or the generation fails.</response>
        [HttpGet("GenerateExcelSTOperatingRequirements/{id}")]
        public async Task<ActionResult<int>> GenerateExcelSTOperatingRequirements(int id)
        {
            byte[] resGenerate = await _STOperatingRequirementsService.GenerateExcelSTOperatingRequirements(id);

            return File(resGenerate, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "STOR.xlsx");
        }

        //Update

        //[HttpPut("{sosSynopticTableofOperatingRequirements_Id}")]
        //public async Task<ActionResult> UpdateSOSSynopticTableofOperatingRequirements(int sosSynopticTableofOperatingRequirements_Id, SOSSynopticTableofOperatingRequirementsForUpdateDto sosUpdateEntity)
        //{
        //    List<SOSAnalysis> Bkup_Analysis = new List<SOSAnalysis>();
        //    List<SOSSequence> Bkup_Sequence = new List<SOSSequence>();

        //    List<Commentary> Bkup_Notes = new List<Commentary>();
        //    List<Turn> Bkup_Turn = new List<Turn>();
        //    List<SOSSynopticRequirementsLogbook> Bkup_SynopticRequirementsLogbook = new List<SOSSynopticRequirementsLogbook>();
        //    List<SOSSynopticTableofOperatingRequirementsOperationSequence> Backup_SynopticTableofOperatingRequirementsOperationSequence = new List<SOSSynopticTableofOperatingRequirementsOperationSequence>();

        //    // Filtrar nuevos Comentarios
        //    List<UpdateCommentaryDto> filteredCommentaryList = sosUpdateEntity.Notes.Where(t => t.CommentaryId <= 0).ToList();
        //    // Filtrar nuevos SynopticRequirementsLogbooks
        //    List<SOSSynopticRequirementsLogbookForUpdateDto> filteredSynopticRequirementsLogbooksList = sosUpdateEntity.SynopticRequirementsLogbooks.Where(t => t.SOSSynopticRequirementsLogbookId <= 0).ToList();
        //    // Filtrar nuevos Tiempos
        //    List<SOSSynopticTableofOperatingRequirementsOperationSequenceForUpdateDto> filteredSOSOperationSequenceList = sosUpdateEntity.SOSSynopticTableofOperatingRequirementsOperationSequence.Where(t => t.SOSSynopticTableofOperatingRequirementsOperationSequenceId <= 0).ToList();
        //    // Filtrar nuevos Turnos
        //    List<TurnForUpdateDto> filteredTurnList = sosUpdateEntity.Turns.Where(t => t.TurnId <= 0).ToList();


        //    // Remover nuevos Comentarios de la lista principal para evitar duplicados
        //    if (filteredCommentaryList.Any())
        //    {
        //        sosUpdateEntity.Notes.RemoveAll(t => t.CommentaryId == null || t.CommentaryId <= 0);

        //        // Mapear nuevas norms/standars
        //        List<Commentary> newCommentarys = _mapper.Map<List<Commentary>>(filteredCommentaryList);

        //        foreach (var newComentary in newCommentarys)
        //        {
        //            newComentary.CommentaryId = 0;
        //            newComentary.IsActive = true;
        //        }

        //        var resultAddCommentary = await _ProcessRepository.AddRangeCommentary(newCommentarys);

        //        if (resultAddCommentary != null)
        //        {
        //            Debug.WriteLine("Commentarios añadidos con exitop");
        //            Bkup_Notes.AddRange(resultAddCommentary);
        //        }
        //        else
        //        {
        //            Debug.WriteLine("Error Commentarios añadidos");
        //        }
        //    }



        //    // Remover nuevos SynopticRequirementsLogbooks de la lista principal para evitar duplicados
        //    if (filteredSynopticRequirementsLogbooksList.Any())
        //    {
        //        sosUpdateEntity.SynopticRequirementsLogbooks.RemoveAll(t => t.SOSSynopticRequirementsLogbookId == null || t.SOSSynopticRequirementsLogbookId <= 0);

        //        // Mapear nuevas norms/standars
        //        List<SOSSynopticRequirementsLogbook> newSOSSynopticRequirementsLogbook = _mapper.Map<List<SOSSynopticRequirementsLogbook>>(filteredSynopticRequirementsLogbooksList);

        //        foreach (var SynopticRequirementsLogbook in newSOSSynopticRequirementsLogbook)
        //        {
        //            SynopticRequirementsLogbook.SOSSynopticRequirementsLogbookId = 0;
        //            SynopticRequirementsLogbook.IsActive = true;
        //        }

        //        var resultAddSOSSynopticRequirementsLogbook = await _ProcessRepository.AddRangeSOSSynopticRequirementsLogbook(newSOSSynopticRequirementsLogbook);

        //        if (resultAddSOSSynopticRequirementsLogbook != null)
        //        {
        //            Debug.WriteLine("SynopticRequirementsLogbooks añadidos con exitop");
        //            Bkup_SynopticRequirementsLogbook.AddRange(resultAddSOSSynopticRequirementsLogbook);
        //        }
        //        else
        //        {
        //            Debug.WriteLine("Error SynopticRequirementsLogbooks añadidos");
        //        }
        //    }

        //    //aqui añadiremos las nuevas OperationSequence
        //    if (filteredSOSOperationSequenceList.Any())
        //    {
        //        sosUpdateEntity.SOSSynopticTableofOperatingRequirementsOperationSequence.RemoveAll(t => t.SOSSynopticTableofOperatingRequirementsOperationSequenceId == null || t.SOSSynopticTableofOperatingRequirementsOperationSequenceId <= 0);

        //        // Mapear nuevas tiempos
        //        List<SOSSynopticTableofOperatingRequirementsOperationSequence> newSOSTime = _mapper.Map<List<SOSSynopticTableofOperatingRequirementsOperationSequence>>(filteredSOSOperationSequenceList);

        //        foreach (var time in newSOSTime)
        //        {
        //            time.SOSSynopticTableofOperatingRequirementsOperationSequenceId = 0;
        //            time.IsActive = true;
        //        }

        //        var resultAddSOSTime = await _ProcessRepository.AddRangeSOSSynopticTableofOperatingRequirementsOperationSequences(newSOSTime);


        //        if (resultAddSOSTime != null)
        //        {
        //            Debug.WriteLine("Add SOSTime añadidos con exitop");
        //            Backup_SynopticTableofOperatingRequirementsOperationSequence.AddRange(resultAddSOSTime);
        //        }
        //        else
        //        {
        //            Debug.WriteLine("Error Add SOSTime añadidos");
        //        }
        //    }

        //    //Turnos
        //    if (filteredTurnList.Any())
        //    {
        //        sosUpdateEntity.Turns.RemoveAll(t => t.TurnId == null || t.TurnId <= 0);

        //        // Mapear nuevas tiempos
        //        List<Turn> newTurn = _mapper.Map<List<Turn>>(filteredTurnList);

        //        foreach (var time in newTurn)
        //        {
        //            time.TurnId = 0;
        //            //time.IsActive = true;
        //        }

        //        var resultAddTurn = await _ProcessRepository.AddRangeTurns(newTurn);

        //        if (resultAddTurn != null)
        //        {
        //            Debug.WriteLine("Add Turn añadidos con exito");
        //            Bkup_Turn.AddRange(resultAddTurn);
        //        }
        //        else
        //        {
        //            Debug.WriteLine("Error Add Turn añadidos");
        //        }
        //    }


        //    SOSSynopticTableofOperatingRequirements _sosSynopticTableofOperatingRequirements = await _ProcessRepository.GetSOSSynopticTableofOperatingRequirements(sosSynopticTableofOperatingRequirements_Id, true, true, true, true, includeTurns: true, includeTimes: true, includeCollections: true);

        //    ////Aqui va el historico de ser necesario en  un futuro 

        //    ////Ejemplo de uso 
        //    ////Compare genera un string que menciona las diferencias
        //    ////string jsonResult = CompareAndGenerateJson(_mapper.Map<SOSHubForUpdateDto>(entitySOSHub), _SOSHubForUpdate);
        //    ////se crea un entity 
        //    ////SOSHubHistory newHistory = new SOSHubHistory();
        //    ////_mapper.Map(entitySOSHub, newHistory);
        //    ////newHistory.VersionChanges = jsonResult;
        //    ////se almacena la entity anterior y se le añade el resumen de cambios
        //    ////await _ProcessRepository.CreateHistorySOScollection(newHistory);



        //    //Crear bkup de datos relacionados
        //    //hacer update entity sin relaciones

        //    foreach (var sequence in sosUpdateEntity.Sequences)
        //    {
        //        SOSSequence sequenceToAdd = await _ProcessRepository.GetSOSSequence(sequence.SOSSequenceId);
        //        Bkup_Sequence.Add(sequenceToAdd);
        //    }

        //    foreach (var analysis in sosUpdateEntity.Analyses)
        //    {
        //        SOSAnalysis analysisToAdd = await _ProcessRepository.GetSOSAnalysis(analysis.SOSAnalysisId);
        //        Bkup_Analysis.Add(analysisToAdd);
        //    }


        //    foreach (var note in sosUpdateEntity.Notes)
        //    {
        //        var CommentaryUpdate = await _ProcessRepository.UpdateCommentary(note);

        //        Commentary CommentaryToAdd = await _ProcessRepository.GetCommentaryById(note.CommentaryId);
        //        Bkup_Notes.Add(CommentaryToAdd);
        //    }

        //    var AdditionalTimeUpdate = await _ProcessRepository.UpdateSOSSynopticTableofOperatingRequirementsAdditionalTime(sosUpdateEntity.SOSSynopticTableofOperatingRequirementsAdditionalTime);
        //    additionalTime = await _ProcessRepository.GetSOSSynopticTableofOperatingRequirementsAdditionalTimeId(sosUpdateEntity.SOSSynopticTableofOperatingRequirementsAdditionalTime.SOSSynopticTableofOperatingRequirementsAdditionalTimeId);

        //    foreach (var logbook in sosUpdateEntity.SynopticRequirementsLogbooks)
        //    {
        //        var SynopticTableofOperatingRequirementsUpdate = await _ProcessRepository.UpdateSynopticRequirementsLogbook(logbook);
        //        SOSSynopticRequirementsLogbook SynopticTableofOperatingRequirementsBkaux = await _ProcessRepository.GetSOSSynopticRequirementsLogbookById(logbook.SOSSynopticRequirementsLogbookId);
        //        Bkup_SynopticRequirementsLogbook.Add(SynopticTableofOperatingRequirementsBkaux);
        //    }

        //    //Update 
        //    foreach (var operationsequence in sosUpdateEntity.SOSSynopticTableofOperatingRequirementsOperationSequence)
        //    {
        //        var operationsequenceUpdate = await _ProcessRepository.UpdateSOSSynopticTableofOperatingRequirementsOperationSequences(operationsequence);
        //        SOSSynopticTableofOperatingRequirementsOperationSequence timeBkaux = await _ProcessRepository.GetSOSSynopticTableofOperatingRequirementsOperationSequencesById(operationsequence.SOSSynopticTableofOperatingRequirementsOperationSequenceId);
        //        Backup_SynopticTableofOperatingRequirementsOperationSequence.Add(timeBkaux);
        //    }

        //    foreach (var turn in sosUpdateEntity.Turns)
        //    {
        //        var turnUpdate = await _ProcessRepository.UpdateTurn(turn);
        //        Turn turnBkaux = await _ProcessRepository.GetTurnById(turn.TurnId);
        //        Bkup_Turn.Add(turnBkaux);
        //    }

        //    var hubIdsFromAnalyses = Bkup_Analysis.Select(a => a.SOSHubId);

        //    var hubIdsFromSequences = Bkup_Sequence.Select(s => s.SOSHubId);

        //    var allHubIds = hubIdsFromAnalyses.Concat(hubIdsFromSequences).Distinct().ToList();

        //    var hubsToAssociate = new List<SOSHub>();
        //    foreach (var hubId in allHubIds)
        //    {
        //        var hub = await _ProcessRepository.GetSOSHub(hubId);
        //        if (hub != null)
        //            hubsToAssociate.Add(hub);
        //    }


        //    //if (_sosSynopticTableofOperatingRequirements.SOSHubs == null)
        //    //    _sosSynopticTableofOperatingRequirements.SOSHubs = new List<SOSHub>();


        //    //Nulleamos el update para evitar errores
        //    sosUpdateEntity.SOSHubs = null;
        //    sosUpdateEntity.Sequences = null;
        //    sosUpdateEntity.Analyses = null;

        //    sosUpdateEntity.Notes = null;
        //    sosUpdateEntity.Turns = null;
        //    sosUpdateEntity.SynopticRequirementsLogbooks = null;
        //    sosUpdateEntity.SOSSynopticTableofOperatingRequirementsOperationSequence = null;
        //    sosUpdateEntity.SOSSynopticTableofOperatingRequirementsAdditionalTime = null;

        //    await _ProcessRepository.SOSDataRemoveAllSequencesFromSOSSynopticTableofOperatingRequirements(_sosSynopticTableofOperatingRequirements);
        //    await _ProcessRepository.SOSDataRemoveAllAnalysisFromSOSSynopticTableofOperatingRequirements(_sosSynopticTableofOperatingRequirements);

        //    await _ProcessRepository.RemoveAllOperationsSequenceFromSOSSynopticTableofOperatingRequirements(_sosSynopticTableofOperatingRequirements, Backup_SynopticTableofOperatingRequirementsOperationSequence);

        //    await _ProcessRepository.RemoveAllTurnsFromSOSSynopticTableofOperatingRequirements(_sosSynopticTableofOperatingRequirements);
        //    await _ProcessRepository.SOSDataRemoveAllNotesFromSOSSynopticTableofOperatingRequirements(_sosSynopticTableofOperatingRequirements);
        //    await _ProcessRepository.SOSDataRemoveAllSOSSynopticRequirementsLogbookFromSOSSynopticTableofOperatingRequirements(_sosSynopticTableofOperatingRequirements);
        //    await _ProcessRepository.SOSDataRemoveAllSOSSynopticTableofOperatingRequirementsAdditionalTimeFromSOSSynopticTableofOperatingRequirements(_sosSynopticTableofOperatingRequirements);

        //    await _ProcessRepository.SOSDataRemoveAllSOSHubsFromSOSSynopticTableofOperatingRequirements(_sosSynopticTableofOperatingRequirements);

        //    var result = await _ProcessRepository.UpdateSOSSynopticTableofOperatingRequirements(sosUpdateEntity, _sosSynopticTableofOperatingRequirements);




        //    //Notes - Volver a añádir las notas
        //    if (Bkup_Analysis.Any())
        //    {
        //        foreach (SOSAnalysis Analysis in Bkup_Analysis)
        //        {
        //            await _ProcessRepository.AddAnalysisToSOSSynopticTableofOperatingRequirements(_sosSynopticTableofOperatingRequirements, Analysis);
        //        }
        //    }
        //    if (Bkup_Sequence.Any())
        //    {
        //        foreach (SOSSequence Sequence in Bkup_Sequence)
        //        {
        //            await _ProcessRepository.AddSequenceToSOSSynopticTableofOperatingRequirements(_sosSynopticTableofOperatingRequirements, Sequence);
        //        }
        //    }




        //    if (Bkup_Notes.Any())
        //    {
        //        foreach (Commentary Comment in Bkup_Notes)
        //        {
        //            await _ProcessRepository.AddNoteToSOSSynopticTableofOperatingRequirements(_sosSynopticTableofOperatingRequirements, Comment);
        //        }
        //    }

        //    //SynopticTableofOperatingRequirements Logbook
        //    if (Bkup_SynopticRequirementsLogbook.Any())
        //    {
        //        foreach (SOSSynopticRequirementsLogbook logbook in Bkup_SynopticRequirementsLogbook)
        //        {
        //            await _ProcessRepository.AddSOSSynopticRequirementsLogbookToSOSSynopticTableofOperatingRequirements(_sosSynopticTableofOperatingRequirements, logbook);
        //        }
        //    }

        //    //Times
        //    if (Backup_SynopticTableofOperatingRequirementsOperationSequence.Any())
        //    {
        //        foreach (SOSSynopticTableofOperatingRequirementsOperationSequence operationSequence in Backup_SynopticTableofOperatingRequirementsOperationSequence)
        //        {
        //            await _ProcessRepository.AddOperationSequenceToSOSSynopticTableofOperatingRequirements(_sosSynopticTableofOperatingRequirements, operationSequence);
        //        }
        //    }

        //    //turns
        //    if (Bkup_Turn.Any())
        //    {
        //        foreach (Turn turn in Bkup_Turn)
        //        {
        //            await _ProcessRepository.AddTurnToSOSSynopticTableofOperatingRequirements(_sosSynopticTableofOperatingRequirements, turn);
        //        }
        //    }

        //    await _ProcessRepository.AddSOSSynopticTableofOperatingRequirementsAdditionalTimeToSOSSynopticTableofOperatingRequirements(_sosSynopticTableofOperatingRequirements, additionalTime);

        //     if (hubsToAssociate.Any())
        //    {
        //        foreach (SOSHub hub in hubsToAssociate)
        //        {
        //            if (!_sosSynopticTableofOperatingRequirements.SOSHubs.Any(h => h.SOSHubId == hub.SOSHubId))
        //                await _ProcessRepository.AddSOSHubToSOSSynopticTableofOperatingRequirements(_sosSynopticTableofOperatingRequirements, hub);
        //        }
        //    }

        //    // aqui me falta una verificacion para que se añada la relacioncon los soshubs
        //    // si hay soscombinationoperations o analisis o secuencia debe haber 1 sos hub


        //    if (_sosSynopticTableofOperatingRequirements.SOSHubs == null || !_sosSynopticTableofOperatingRequirements.SOSHubs.Any())
        //    {
        //        int? hubId =
        //            Bkup_Analysis.FirstOrDefault()?.SOSHubId ??
        //            Bkup_Sequence.FirstOrDefault()?.SOSHubId ??
        //            Backup_SynopticTableofOperatingRequirementsOperationSequence
        //                .Select(op =>
        //                {

        //                    return (int?)null;
        //                })
        //                .FirstOrDefault(id => id.HasValue);

        //        if (hubId.HasValue)
        //        {
        //            var hub = await _ProcessRepository.GetSOSHub(hubId.Value);
        //            if (hub != null)
        //            {
        //                await _ProcessRepository.AddSOSHubToSOSSynopticTableofOperatingRequirements(_sosSynopticTableofOperatingRequirements, hub);
        //            }
        //        }
        //    }


        //    if (result != null)
        //    {
        //        return Ok(_sosSynopticTableofOperatingRequirements);
        //    }
        //    else
        //        return BadRequest();

        //}//end Update 

        //[HttpDelete("{SOSSynopticTableofOperatingRequirementsId}")]
        //public async Task<ActionResult<int>> RemoveSOSHub(int SOSSynopticTableofOperatingRequirementsId)
        //{
        //    var result = await _ProcessRepository.RemoveSOSSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirementsId);

        //    if (result > 0)
        //        return Ok();
        //    else
        //        return BadRequest("something wrong");
        //}


        ////ilustrations

        //[HttpPost("Ilustrations/{SynopticTableofOperatingRequirements_id}")]
        //public async Task<ActionResult<FileUpload>> UploadIlustrations(int SynopticTableofOperatingRequirements_id, IFormFile file)
        //{

        //    var uploadResult = new FileUploadForCreationDto();
        //    string trustedFileNameForStorage = string.Empty;
        //    var unstrustedFileName = file.FileName;

        //    trustedFileNameForStorage = Path.GetRandomFileName();

        //    var path = Path.Combine(_env.ContentRootPath, "uploads\\SOSSynopticTableofOperatingRequirements\\Ilustrations", trustedFileNameForStorage);
        //    // Asegurarse de que el directorio de destino exista
        //    var directory = Path.GetDirectoryName(path);
        //    if (!Directory.Exists(directory))
        //    {
        //        Directory.CreateDirectory(directory);
        //    }


        //    await using FileStream fs = new(path, FileMode.Create);
        //    await file.CopyToAsync(fs);

        //    uploadResult.FileName = unstrustedFileName;
        //    uploadResult.StorageFileName = trustedFileNameForStorage;
        //    uploadResult.ContentType = file.ContentType;
        //    uploadResult.UploadDate = DateTime.Now;
        //    uploadResult.IsActive = true;

        //    var fileToReturn = await _ProcessRepository.CreateFileAsync(uploadResult);

        //    await _ProcessRepository.AddIlustrationToSOSSynopticTableofOperatingRequirements(SynopticTableofOperatingRequirements_id, fileToReturn);
        //    await _ProcessRepository.SaveChangesAsync();

        //    return Ok(fileToReturn);
        //}

        //[HttpGet("Ilustrations/{fileid}")]
        //public async Task<IActionResult> DownloadIlustrations(int fileid)
        //{
        //    var FileInfo = await _ProcessRepository.FetchFileAsync(fileid);

        //    if (FileInfo is not null)
        //    {
        //        var path = Path.Combine(_env.ContentRootPath, "uploads\\SOSSynopticTableofOperatingRequirements\\Ilustrations", FileInfo.StorageFileName);

        //        var memory = new MemoryStream();
        //        using (var stream = new FileStream(path, FileMode.Open))
        //        {
        //            await stream.CopyToAsync(memory);
        //        }
        //        memory.Position = 0;

        //        var result = File(memory, FileInfo.ContentType, Path.GetFileName(path));
        //        result.EnableRangeProcessing = true;

        //        return result;
        //    }
        //    return NotFound("Error File download");
        //}

        //[HttpDelete("Ilustrations/{SOS_SOSSynopticTableofOperatingRequirements_id}/remove/{ImageFile_id}")]
        //public async Task<ActionResult<int>> RemoveImage(int SOS_SOSSynopticTableofOperatingRequirements_id, int ImageFile_id)
        //{
        //    var result = await _ProcessRepository.RemoveIlustrationFromSOSSynopticTableofOperatingRequirements(SOS_SOSSynopticTableofOperatingRequirements_id, ImageFile_id);

        //    if (result > 0)
        //        return Ok();
        //    else
        //        return BadRequest("something went wrong");
        //}

    }
}
