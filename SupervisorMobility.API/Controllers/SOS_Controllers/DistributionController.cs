using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.SOS.SOSDistributionAdditionalTimeDtos;
using SupervisorMobility.API.Models.SOS.SOSDistributionDtos;
using SupervisorMobility.API.Models.SOS.SOSDistributionLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSDistributionOperationSequenceDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos.SectionDtos;
using SupervisorMobility.API.Models.SOS.SOSTimeDtos;
using SupervisorMobility.API.Models.SOS.TurnDtos;
using System.Collections.Generic;
using System.Diagnostics;

namespace SupervisorMobility.API.Controllers.SOS_Controllers
{
    [Route("api/SOS/Distribution")]
    [ApiController]
    public class DistributionController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly ISOS_ProcessRepository _ProcessRepository;
        public DistributionController(IWebHostEnvironment env, IMapper mapper, ISOS_ProcessRepository repository)
        {
            _ProcessRepository = repository;
            _mapper = mapper ??
                  throw new ArgumentNullException(nameof(mapper));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        [HttpPost]
        public async Task<ActionResult<SOSDistributionDto>> GenerateDistribution(SOSDistributionForCreateDto sOSDistributionToCreate, int SOSHubCollection_Id)
        {

            if (sOSDistributionToCreate.SOSDistributionId == 0)
            {
                //Nombre del documento GOS o processShet
                //sOSDistributionToCreate.InternalControlNumber = SOSEntity.Folio;
                //sOSDistributionToCreate.ProcessName = SOSEntity.ProcessSheet;

                sOSDistributionToCreate.CreatedAt = DateTime.Now;
                sOSDistributionToCreate.IsActive = true;

                sOSDistributionToCreate.SOSHubId = SOSHubCollection_Id;
                //cambiar por la adicion de los soshub de las analisis y secuencias elegidos


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

                SOSDistribution DistributionToCreate = _mapper.Map<SOSDistribution>(sOSDistributionToCreate);

                var createdResult = await _ProcessRepository.CreateSOSDistribution(DistributionToCreate);
                if (createdResult != null)
                    return Ok(DistributionToCreate);
                else
                    return BadRequest();
            }
            else
            {
                //only add revision
                SOSDistribution _sosDistribution = await _ProcessRepository.GetSOSDistribution(sOSDistributionToCreate.SOSDistributionId, true, true, true, true);

                SOSDistributionLogbook _logbookToCreate = _mapper.Map<SOSDistributionLogbook>(sOSDistributionToCreate.DistributionLogbooks?.Last());
                _logbookToCreate.SOSDistributionId = _sosDistribution.SOSDistributionId;

                var resultAddSections = await _ProcessRepository.CreateSOSDistributionLogbook(_logbookToCreate);

                if (resultAddSections > 0)
                {
                    Debug.WriteLine("SOSDistributionLogbook añadidas con exito");
                    await _ProcessRepository.AddSOSDistributionLogbookToSOSDistribution(_sosDistribution, _logbookToCreate);
                }
                else
                {
                    Debug.WriteLine("Error Sections añadidos");
                    return BadRequest();
                }



                return Ok("Revision");
            }

        }

        [HttpGet("{id}", Name = "GetSOSDistribution")]
        public async Task<ActionResult<SOSDistributionDto>> GetSOSDistribution(int id, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSOS = false, bool includeImagesSOS = false, bool includeTurns = false, bool includeTimes = false, bool includeCollections = false)
        {

            var SOSDistribution = await _ProcessRepository.GetSOSDistribution(id, includeImages, includeNotes, includeLogbooks, includeSOS, includeImagesSOS, includeTurns, includeTimes, includeCollections: includeCollections);
            if (SOSDistribution == null)
            {
                return NotFound("SOSDistribution not found!");
            }

            var mappedDto = _mapper.Map<SOSDistributionDto>(SOSDistribution);

            return Ok(mappedDto);
        }

        [HttpGet("bySosHub/{idSOSHub}", Name = "GetDistributionBySOSHub")]
        public async Task<ActionResult<SOSDistributionDto>> GetDistributionBySOSHub(int idSOSHub, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSOS = false, bool includeImagesSOS = false, bool includeTurns = false, bool includeTimes = false, bool includeCollections = false)
        {
            var idDistribution = await _ProcessRepository.GetIdDistributionBySosHub(idSOSHub);

            var SOSDistribution = await _ProcessRepository.GetSOSDistribution(idDistribution, includeImages, includeNotes, includeLogbooks, includeSOS, includeImagesSOS, includeTurns, includeTimes, includeCollections: includeCollections);
            if (SOSDistribution == null)
            {
                return NotFound("SOSDistribution not found!");
            }

            var mappedDto = _mapper.Map<SOSDistributionDto>(SOSDistribution);

            return Ok(mappedDto);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<SOSDistributionDto>>> GetAllSOSDistribution(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSOS = false)
        {

            var CheckpointEntities = await _ProcessRepository.GetAllSOSDistribution(includeImages, includeNotes, includeLogbooks, includeSOS);
            if (CheckpointEntities == null)
            {
                return NotFound("Get All Sos Analisis not found!");
            }

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

                var resultAddSOSDistributionLogbook = await _ProcessRepository.AddRangeSOSDistributionLogbook(newSOSDistributionLogbook);

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

                var resultAddSOSTime = await _ProcessRepository.AddRangeSOSDistributionOperationSequences(newSOSTime);


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


            SOSDistribution _sosDistribution = await _ProcessRepository.GetSOSDistribution(sosDistribution_Id, true, true, true, true, includeTurns: true, includeTimes: true, includeCollections: true);

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
                SOSSequence sequenceToAdd = await _ProcessRepository.GetSOSSequence(sequence.SOSSequenceId);
                Bkup_Sequence.Add(sequenceToAdd);
            }

            foreach (var analysis in sosUpdateEntity.Analyses)
            {
                SOSAnalysis analysisToAdd = await _ProcessRepository.GetSOSAnalysis(analysis.SOSAnalysisId);
                Bkup_Analysis.Add(analysisToAdd);
            }


            foreach (var note in sosUpdateEntity.Notes)
            {
                var CommentaryUpdate = await _ProcessRepository.UpdateCommentary(note);

                Commentary CommentaryToAdd = await _ProcessRepository.GetCommentaryById(note.CommentaryId);
                Bkup_Notes.Add(CommentaryToAdd);
            }

            var AdditionalTimeUpdate = await _ProcessRepository.UpdateSOSDistributionAdditionalTime(sosUpdateEntity.SOSDistributionAdditionalTime);
            additionalTime = await _ProcessRepository.GetSOSDistributionAdditionalTimeId(sosUpdateEntity.SOSDistributionAdditionalTime.SOSDistributionAdditionalTimeId);

            foreach (var logbook in sosUpdateEntity.DistributionLogbooks)
            {
                var distributionUpdate = await _ProcessRepository.UpdateDistributionLogbook(logbook);
                SOSDistributionLogbook distributionBkaux = await _ProcessRepository.GetSOSDistributionLogbookById(logbook.SOSDistributionLogbookId);
                Bkup_DistributionLogbook.Add(distributionBkaux);
            }

            //Update 
            //var validIds = operationSequences.Select(x => x.SOSDistributionOperationSequenceId).ToList();
            //Console.WriteLine(validIds);
            // Buscar en la base de datos todas las OperationSequences que NO estén en la lista de IDs válidos
            //var toRemove = await _context.SOSDistributionOperationSequence
            //    .Where(x => !validIds.Contains(x.SOSDistributionOperationSequenceId))
            //    .ToListAsync();

            List<int> idsNewPerSection = filteredSOSOperationSequenceList.Where(os => os.SectionId.HasValue).Select(os => os.SectionId.Value).ToList();

            List<SOSDistributionOperationSequence> AllOperationSequences = _sosDistribution.SOSDistributionOperationSequence.Where(so => !idsNewPerSection.Contains(so.SOSDistributionOperationSequenceId)).ToList();
            List<SOSDistributionOperationSequence> OperationSequencesDelete = new List<SOSDistributionOperationSequence>();
            List<SOSDistributionOperationSequenceForUpdateDto> OperationSequencesUpdate = new List<SOSDistributionOperationSequenceForUpdateDto>();


            foreach (var operationSequence in AllOperationSequences)
            {
                var findOperation = sosUpdateEntity.SOSDistributionOperationSequence.FirstOrDefault(a => a.SectionId == operationSequence.SectionId);
                if (findOperation == null)
                {
                    OperationSequencesDelete.Add(operationSequence);
                }
                else
                {
                    OperationSequencesUpdate.Add(findOperation);
                }
            }

            foreach (var operationsequence in OperationSequencesUpdate)
            {
                var operationsequenceUpdate = await _ProcessRepository.UpdateSOSDistributionOperationSequences(operationsequence);
                SOSDistributionOperationSequence timeBkaux = await _ProcessRepository.GetSOSDistributionOperationSequencesById(operationsequence.SOSDistributionOperationSequenceId);
                Backup_DistributionOperationSequence.Add(timeBkaux);
            }

            foreach (var operationsequence in OperationSequencesDelete) {
                await _ProcessRepository.DeleteSOSDistributionOperationSequencesById(operationsequence.SOSDistributionOperationSequenceId);
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

            //Nulleamos el update para evitar errores
            sosUpdateEntity.SOSHubs = null;
            sosUpdateEntity.Sequences = null;
            sosUpdateEntity.Analyses = null;

            sosUpdateEntity.Notes = null;
            sosUpdateEntity.Turns = null;
            sosUpdateEntity.DistributionLogbooks = null;
            sosUpdateEntity.SOSDistributionOperationSequence = null;
            sosUpdateEntity.SOSDistributionAdditionalTime = null;

            await _ProcessRepository.SOSDataRemoveAllSequencesFromSOSDistribution(_sosDistribution);
            await _ProcessRepository.SOSDataRemoveAllAnalysisFromSOSDistribution(_sosDistribution);

            // await _ProcessRepository.RemoveAllOperationsSequenceFromSOSDistribution(_sosDistribution, Backup_DistributionOperationSequence);
 
            await _ProcessRepository.RemoveAllTurnsFromSOSDistribution(_sosDistribution);
            await _ProcessRepository.SOSDataRemoveAllNotesFromSOSDistribution(_sosDistribution);
            await _ProcessRepository.SOSDataRemoveAllSOSDistributionLogbookFromSOSDistribution(_sosDistribution);
            await _ProcessRepository.SOSDataRemoveAllSOSDistributionAdditionalTimeFromSOSDistribution(_sosDistribution);

            await _ProcessRepository.SOSDataRemoveAllSOSHubsFromSOSDistribution(_sosDistribution);

            var result = await _ProcessRepository.UpdateSOSDistribution(sosUpdateEntity, _sosDistribution);

          


            //Notes - Volver a añádir las notas
            if (Bkup_Analysis.Any())
            {
                foreach (SOSAnalysis Analysis in Bkup_Analysis)
                {
                    await _ProcessRepository.AddAnalysisToSOSDistribution(_sosDistribution, Analysis);
                }
            }
            if (Bkup_Sequence.Any())
            {
                foreach (SOSSequence Sequence in Bkup_Sequence)
                {
                    await _ProcessRepository.AddSequenceToSOSDistribution(_sosDistribution, Sequence);
                }
            }



            if (Bkup_Notes.Any())
            {
                foreach (Commentary Comment in Bkup_Notes)
                {
                    await _ProcessRepository.AddNoteToSOSDistribution(_sosDistribution, Comment);
                }
            }

            //Distribution Logbook
            if (Bkup_DistributionLogbook.Any())
            {
                foreach (SOSDistributionLogbook logbook in Bkup_DistributionLogbook)
                {
                    await _ProcessRepository.AddSOSDistributionLogbookToSOSDistribution(_sosDistribution, logbook);
                }
            }

            //Times
            if (Backup_DistributionOperationSequence.Any())
            {
                foreach (SOSDistributionOperationSequence operationSequence in Backup_DistributionOperationSequence)
                {
                    await _ProcessRepository.AddOperationSequenceToSOSDistribution(_sosDistribution, operationSequence);
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

            await _ProcessRepository.AddSOSDistributionAdditionalTimeToSOSDistribution(_sosDistribution, additionalTime);


             if (hubsToAssociate.Any())
            {
                foreach (SOSHub hub in hubsToAssociate)
                {
                    if (_sosDistribution.SOSHubs.Any(h => h.SOSHubId == hub.SOSHubId))
                        await _ProcessRepository.AddSOSHubToSOSDistribution(_sosDistribution, hub);
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
                        await _ProcessRepository.AddSOSHubToSOSDistribution(_sosDistribution, hub);
                    }
                }
            }


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
            var result = await _ProcessRepository.RemoveSOSDistribution(SOSDistributionId);

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

            await _ProcessRepository.AddIlustrationToSOSDistribution(Distribution_id, fileToReturn);
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
            var result = await _ProcessRepository.RemoveIlustrationFromSOSDistribution(SOS_SOSDistribution_id, ImageFile_id);

            if (result > 0)
                return Ok();
            else
                return BadRequest("something went wrong");
        }

    }
}
