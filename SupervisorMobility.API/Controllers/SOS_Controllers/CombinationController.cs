using AutoMapper;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Quartz.Core;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.DataAccess.Services.SOS_Combination;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.NotificationDtos;
using SupervisorMobility.API.Models.SOS.SOSAnalysisDtos;
using SupervisorMobility.API.Models.SOS.SOSCombinationDtos;
using SupervisorMobility.API.Models.SOS.SOSCombinationLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSCombinationOperationSequenceDtos;
using SupervisorMobility.API.Models.SOS.SOSFlowDtos;
using SupervisorMobility.API.Models.SOS.SOSFlowLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSTimeDtos;
using SupervisorMobility.API.Models.SOS.TurnDtos;
using System.Diagnostics;

namespace SupervisorMobility.API.Controllers.SOS_Controllers
{
    [Route("api/SOS/Combination")]
    [ApiController]
    public class CombinationController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly ISOS_ProcessRepository _ProcessRepository;
        private readonly ISOS_CombinationRepository _CombinationRepository;
        private readonly INotificationService _notificationService;

        public CombinationController(IWebHostEnvironment env, IMapper mapper, ISOS_ProcessRepository repository, ISOS_CombinationRepository combinationRepository, INotificationService notificationService)
        {
            _ProcessRepository = repository;
            _mapper = mapper ??
                  throw new ArgumentNullException(nameof(mapper));
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _CombinationRepository = combinationRepository;
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        [HttpPost]
        public async Task<ActionResult<SOSCombinationDto>> GenerateCombination(SOSCombinationForCreateDto sOSCombinationToCreate, int SOSHubCollection_Id)
        {
            SOSHub SOSEntity = await _ProcessRepository.GetSOSHub(SOSHubCollection_Id, includeInformation: true, includeSections: true);

            if (sOSCombinationToCreate.SOSCombinationId == 0)
            {
                sOSCombinationToCreate.IsActive = true;
                sOSCombinationToCreate.CreatedAt = DateTime.Now;

                sOSCombinationToCreate.SOSHubId = SOSHubCollection_Id;

                if(sOSCombinationToCreate.SOSCombinationOperationSequence == null || sOSCombinationToCreate.SOSCombinationOperationSequence.Count==0)
                {
                    sOSCombinationToCreate.SOSCombinationOperationSequence = new List<SOSCombinationOperationSequenceForCreateDto>();
                }

                    //sOSCombinationToCreate.SOSCombinationOperationSequence = new List<SOSCombinationOperationSequenceForCreateDto>();
                if (SOSEntity != null && (SOSEntity.Sections!=null && SOSEntity.Sections.Count > 0))
                {
                    foreach (Section sec in SOSEntity.Sections)
                    {
                        SOSCombinationOperationSequenceForCreateDto CombinationOperationToAdd = new();

                        CombinationOperationToAdd.SectionId = sec.SectionId;
                        CombinationOperationToAdd.ProcessName = sec.Step;
                        CombinationOperationToAdd.SequenceId = SOSEntity.Sections.ToList().IndexOf(sec) + 1;
                        CombinationOperationToAdd.IsActive = true;

                        sOSCombinationToCreate.SOSCombinationOperationSequence?.Add(CombinationOperationToAdd);
                    }
                }
               

                SOSCombination CombinationToCreate = _mapper.Map<SOSCombination>(sOSCombinationToCreate);

                if (CombinationToCreate.ReviewerHSId <= 0)
                {
                    CombinationToCreate.ReviewerHSId = null;
                }

                var createdResult = await _CombinationRepository.CreateSOSCombination(CombinationToCreate);
                if (createdResult > 0)
                {
                    int notifyUserId = CombinationToCreate.ReviewerHSId ?? SOSEntity?.CreatorId ?? 1;
                    await _notificationService.CreateNotificationAsync(new NotificationToCreateDto
                    {
                        MadeBy = "SM Mobility",
                        NotificationType = "SOS Combination Created",
                        NotificationText = $"SOS Combination (ID: {CombinationToCreate.SOSCombinationId}) has been generated for SOS Hub (ID: {SOSHubCollection_Id}).",
                        UserId = notifyUserId,
                        IsActive = true,
                        IsAccepted = true,
                        EntryDate = DateTime.Now
                    });

                    return Ok(CombinationToCreate);
                }
                else
                    return BadRequest();
            }
            else
            {
                //only add revision
                SOSCombination _sosCombination = await _CombinationRepository.GetSOSCombination(sOSCombinationToCreate.SOSCombinationId, true, true, true, true );

                if (sOSCombinationToCreate.ReviewerHSId <= 0)
                {
                    sOSCombinationToCreate.ReviewerHSId = null;
                }else if(sOSCombinationToCreate.ReviewerHSId != _sosCombination.ReviewerHSId) {
                    //update
                }


                SOSCombinationLogbook _logbookToCreate = _mapper.Map<SOSCombinationLogbook>(sOSCombinationToCreate.CombinationLogbooks?.Last());
                _logbookToCreate.SOSCombinationId = _sosCombination.SOSCombinationId;

                var resultAddSections = await _CombinationRepository.CreateSOSCombinationLogbook(_logbookToCreate);

                if (resultAddSections > 0)
                {
                    Debug.WriteLine("SOSCombinationLogbook añadidas con exito");
                    await _CombinationRepository.AddSOSCombinationLogbookToSOSCombination(_sosCombination, _logbookToCreate);
                }
                else
                {
                    Debug.WriteLine("Error Sections añadidos");
                    return BadRequest();
                }



                SOSHub relatedSOSHub = await _ProcessRepository.GetSOSHub(_sosCombination.SOSHubId, includeInformation: true);
                int notifyUserId = relatedSOSHub?.CreatorId ?? _sosCombination.ReviewerHSId ?? 1;
                await _notificationService.CreateNotificationAsync(new NotificationToCreateDto
                {
                    MadeBy = "SM Mobility",
                    NotificationType = "SOS Combination Review Completed",
                    NotificationText = $"SOS Combination (ID: {_sosCombination.SOSCombinationId}) review has been completed.",
                    UserId = notifyUserId,
                    IsActive = true,
                    IsAccepted = true,
                    EntryDate = DateTime.Now
                });

                return Ok(_sosCombination);
            }

        }//New revision

        [HttpGet("{id}", Name = "GetSOSCombination")]
        public async Task<ActionResult<SOSCombinationDto>> GetSOSCombination(int id, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false, bool includeImagesSOS = false, bool includeProcess = false)
        {

            var SOSCombination = await _CombinationRepository.GetSOSCombination(id, includeImages, includeNotes, includeLogbooks,  includeSOS, includeImagesSOS, includeProcess);
            if (SOSCombination == null)
            {
                return NotFound("SOSCombination not found!");
            }

            return Ok(_mapper.Map<SOSCombinationDto>(SOSCombination));
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<SOSCombinationDto>>> GetAllSOSCombination(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false)
        {

            var CheckpointEntities = await _CombinationRepository.GetAllSOSCombination(includeImages, includeNotes, includeLogbooks, includeSOS);
            if (CheckpointEntities == null)
            {
                return NotFound("Get All Sos Analisis not found!");
            }

            return Ok(_mapper.Map<IEnumerable<SOSCombinationDto>>(CheckpointEntities));
        }

        //Update
        [HttpPut("{sosCombination_Id}")]
        public async Task<ActionResult> UpdateSOSCombination(int sosCombination_Id, SOSCombinationForUpdateDto sosUpdateEntity)
        {
            try
            {
                List<Turn> Bkup_Turn = new List<Turn>();
                List<SOSCombinationLogbook> Bkup_CombinationLogbook = new List<SOSCombinationLogbook>();
                List<SOSCombinationOperationSequence> Bkup_OperationSequence = new List<SOSCombinationOperationSequence>();

                // Filtrar nuevos CombinationLogbooks
                List<SOSCombinationLogbookForUpdateDto> filteredCombinationLogbooksList = sosUpdateEntity.CombinationLogbooks.Where(t => t.SOSCombinationLogbookId <= 0).ToList();
                // Filtrar nuevos Turnos
                List<TurnForUpdateDto> filteredTurnList = sosUpdateEntity.Turns.Where(t => t.TurnId <= 0).ToList();
                //filtrar nuevos SOSCombinationOperationSequence
                List<SOSCombinationOperationSequenceForUpdateDto> filteredOperationSequence = sosUpdateEntity.SOSCombinationOperationSequence.Where(sq => sq.SOSCombinationOperationSequenceId <= 0).ToList();

                // Remover nuevos SOSCombinationOperationSequenceForUpdateDto de la lista principal para evitar duplicados
                if (filteredOperationSequence.Any())
                {
                    sosUpdateEntity.SOSCombinationOperationSequence.RemoveAll(t => t.SOSCombinationOperationSequenceId == null || t.SOSCombinationOperationSequenceId <= 0);

                    // Mapear nuevas norms/standars
                    List<SOSCombinationOperationSequence> newSOSOperationSequences = _mapper.Map<List<SOSCombinationOperationSequence>>(filteredOperationSequence);

                    foreach (var OperationSequence in newSOSOperationSequences)
                    {
                        OperationSequence.SOSCombinationOperationSequenceId = 0;
                        OperationSequence.IsActive = true;
                    }

                    var resultAddOperationSequences = await _CombinationRepository.AddRangeSOSCombinationOperationSequences(newSOSOperationSequences);

                    if (resultAddOperationSequences != null)
                    {
                        Debug.WriteLine("Operation Sequences añadidos con exitop");
                        Bkup_OperationSequence.AddRange(resultAddOperationSequences);
                    }
                    else
                    {
                        Debug.WriteLine("Error OperationSequences añadidos");
                    }
                }


                // Remover nuevos CombinationLogbooks de la lista principal para evitar duplicados
                if (filteredCombinationLogbooksList.Any())
                {
                    sosUpdateEntity.CombinationLogbooks.RemoveAll(t => t.SOSCombinationLogbookId == null || t.SOSCombinationLogbookId <= 0);

                    // Mapear nuevas norms/standars
                    List<SOSCombinationLogbook> newSOSCombinationLogbook = _mapper.Map<List<SOSCombinationLogbook>>(filteredCombinationLogbooksList);

                    foreach (var CombinationLogbook in newSOSCombinationLogbook)
                    {
                        CombinationLogbook.SOSCombinationLogbookId = 0;
                        CombinationLogbook.IsActive = true;
                    }

                    var resultAddSOSCombinationLogbook = await _CombinationRepository.AddRangeSOSCombinationLogbook(newSOSCombinationLogbook);

                    if (resultAddSOSCombinationLogbook != null)
                    {
                        Debug.WriteLine("CombinationLogbooks añadidos con exitop");
                        Bkup_CombinationLogbook.AddRange(resultAddSOSCombinationLogbook);
                    }
                    else
                    {
                        Debug.WriteLine("Error CombinationLogbooks añadidos");
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


                SOSCombination _sosCombination = await _CombinationRepository.GetSOSCombination(sosCombination_Id, true, true, true);

                ////Aqui va el historico de ser necesario en  un futuro 

                ////Ejemplo de uso 
                ////Compare genera un string que menciona las diferencias
                ////string jsonResult = CompareAndGenerateJson(_mapper.Map<SOSHubForUpdateDto>(entitySOSHub), _SOSHubForUpdate);
                ////se crea un entity 
                ////SOSHubHistory newHistory = new SOSHubHistory();
                ////_mapper.Map(entitySOSHub, newHistory);
                ////newHistory.VersionChanges = jsonResult;
                ////se almacena la entity anterior y se le añade el resumen de cambios
                ////await _ProcessRepository.CreateHistorySOScollection(newHistory);



                //Crear bkup de datos relacionados

                foreach (var logbook in sosUpdateEntity.CombinationLogbooks)
                {
                    var CombinationUpdate = await _CombinationRepository.UpdateCombinationLogbook(logbook);
                    SOSCombinationLogbook CombinationBkaux = await _CombinationRepository.GetSOSCombinationLogbookById(logbook.SOSCombinationLogbookId);
                    Bkup_CombinationLogbook.Add(CombinationBkaux);
                }

                foreach (var turn in sosUpdateEntity.Turns)
                {
                    var turnUpdate = await _ProcessRepository.UpdateTurn(turn);
                    Turn turnBkaux = await _ProcessRepository.GetTurnById(turn.TurnId);
                    Bkup_Turn.Add(turnBkaux);
                }

                foreach (var operationSequence in sosUpdateEntity.SOSCombinationOperationSequence)
                {
                    var operationSequenceUpdate = await _CombinationRepository.UpdateSOSCombinationOperationSequences(operationSequence);
                    SOSCombinationOperationSequence operationSequenceBkaux = await _CombinationRepository.GetSOSCombinationOperationSequencesById(operationSequence.SOSCombinationOperationSequenceId);
                    Bkup_OperationSequence.Add(operationSequenceBkaux);
                }


                //Nulleamos el update para evitar errores
                sosUpdateEntity.Turns = null;
                sosUpdateEntity.SOSHub = null;
                sosUpdateEntity.CombinationLogbooks = null;
                sosUpdateEntity.SOSCombinationOperationSequence = null;

                await _ProcessRepository.RemoveAllTurnsFromSOSCombination(_sosCombination);
                await _CombinationRepository.RemoveAllOperationsSequenceFromSOSCombination(_sosCombination);
                await _CombinationRepository.SOSDataRemoveAllSOSCombinationLogbookFromSOSCombination(_sosCombination);

                var result = await _CombinationRepository.UpdateSOSCombination(sosUpdateEntity, _sosCombination);

                // Volver a añádir bkup

                //Combination Logbook
                if (Bkup_CombinationLogbook.Any())
                {
                    foreach (SOSCombinationLogbook logbook in Bkup_CombinationLogbook)
                    {
                        await _CombinationRepository.AddSOSCombinationLogbookToSOSCombination(_sosCombination, logbook);
                    }
                }

                //turns
                if (Bkup_Turn.Any())
                {
                    foreach (Turn turn in Bkup_Turn)
                    {
                        await _ProcessRepository.AddTurnToSOSCombination(_sosCombination, turn);
                    }
                }

                //Operations Sequence
                if (Bkup_OperationSequence.Any())
                {
                    foreach (SOSCombinationOperationSequence operationSequence in Bkup_OperationSequence)
                    {
                        await _CombinationRepository.AddOperationSequenceToSOSCombination(_sosCombination, operationSequence);
                    }
                }

                if (result != null)
                {
                    return Ok(_sosCombination);
                }
                else
                    return BadRequest();
            }
            catch (Exception ex)
            {
                return NotFound($"Error: {ex.Message}, Inner: {ex.InnerException}");
            }

        }//end Update 



        [HttpDelete("{SOSCombinationId}")]
        public async Task<ActionResult<int>> RemoveSOSHub(int SOSCombinationId)
        {
            var result = await _CombinationRepository.RemoveSOSCombination(SOSCombinationId);

            if (result > 0)
                return Ok();
            else
                return BadRequest("something wrong");
        }


        //ilustrations

        [HttpPost("Ilustrations/{Combination_id}")]
        public async Task<ActionResult<FileUpload>> UploadIlustrations(int Combination_id, IFormFile file)
        {

            var uploadResult = new FileUploadForCreationDto();
            string trustedFileNameForStorage = string.Empty;
            var unstrustedFileName = file.FileName;

            trustedFileNameForStorage = Path.GetRandomFileName();

            var path = Path.Combine(_env.ContentRootPath, "uploads\\SOSCombination\\Ilustrations", trustedFileNameForStorage);
            //var path = Path.Combine("C:\\", "Users\\zkril\\source\\repos\\SupervisorMobility.API\\upload\\SOSCombination\\Ilustrations", trustedFileNameForStorage);
          
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

            await _CombinationRepository.AddIlustrationToSOSCombination(Combination_id, fileToReturn);
            await _ProcessRepository.SaveChangesAsync();

            return Ok(fileToReturn);
        }

        [HttpGet("Ilustrations/{fileid}")]
        public async Task<IActionResult> DownloadIlustrations(int fileid)
        {
            var FileInfo = await _ProcessRepository.FetchFileAsync(fileid);

            if (FileInfo is not null)
            {
                var path = Path.Combine(_env.ContentRootPath, "uploads\\SOSCombination\\Ilustrations", FileInfo.StorageFileName);
                //var path = Path.Combine("C:\\", "Users\\zkril\\source\\repos\\SupervisorMobility.API\\upload\\SOSCombination\\Ilustrations", FileInfo.StorageFileName);
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

        [HttpDelete("Ilustrations/{SOS_SOSCombination_id}/remove/{ImageFile_id}")]
        public async Task<ActionResult<int>> RemoveImage(int SOS_SOSCombination_id, int ImageFile_id)
        {
            var result = await _CombinationRepository.RemoveIlustrationFromSOSCombination(SOS_SOSCombination_id, ImageFile_id);

            if (result > 0)
                return Ok();
            else
                return BadRequest("something went wrong");
        }
    }
}
