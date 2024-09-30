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
using SupervisorMobility.API.Models.SOS.SOSTimeDtos;
using SupervisorMobility.API.Models.SOS.TurnDtos;
using System.Diagnostics;

namespace SupervisorMobility.API.Controllers.SOS_Controllers
{
    [Route("api/SOS/Distribution")]
    [ApiController]
    public class DistributionController : Controller
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
            SOSHub SOSEntity = await _ProcessRepository.GetSOSHub(SOSHubCollection_Id, includeInformation: true);

            if (sOSDistributionToCreate.SOSDistributionId == 0)
            {
                //Nombre del documento GOS o processShet
                //sOSDistributionToCreate.InternalControlNumber = SOSEntity.Folio;
                //sOSDistributionToCreate.ProcessName = SOSEntity.ProcessSheet;

                sOSDistributionToCreate.CreatedAt = DateTime.Now;
                sOSDistributionToCreate.IsActive = true;

                sOSDistributionToCreate.SOSHubId = SOSHubCollection_Id;

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
        public async Task<ActionResult<SOSDistributionDto>> GetSOSDistribution(int id, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSOS = false, bool includeImagesSOS = false, bool includeTurns = false, bool includeTimes = false)
        {

            var SOSDistribution = await _ProcessRepository.GetSOSDistribution(id, includeImages, includeNotes, includeLogbooks, includeSOS, includeImagesSOS, includeTurns, includeTimes);
            if (SOSDistribution == null)
            {
                return NotFound("SOSDistribution not found!");
            }

            return Ok(_mapper.Map<SOSDistributionDto>(SOSDistribution));
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
            List<Commentary> Bkup_Notes = new List<Commentary>();
            List<Turn> Bkup_Turn = new List<Turn>();
            List<SOSDistributionLogbook> Bkup_DistributionLogbook = new List<SOSDistributionLogbook>();
            List<SOSTime> Bkup_Times = new List<SOSTime>();
            SOSDistributionAdditionalTime additionalTime = new SOSDistributionAdditionalTime();

            // Filtrar nuevos Comentarios
            List<UpdateCommentaryDto> filteredCommentaryList = sosUpdateEntity.Notes.Where(t => t.CommentaryId <= 0).ToList();
            // Filtrar nuevos DistributionLogbooks
            List<SOSDistributionLogbookForUpdateDto> filteredDistributionLogbooksList = sosUpdateEntity.DistributionLogbooks.Where(t => t.SOSDistributionLogbookId <= 0).ToList();
            // Filtrar nuevos Tiempos
            List<SOSTimeForUpdateDto> filteredTimesList = sosUpdateEntity.Times.Where(t => t.SOSTimeId <= 0).ToList();
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

            //aqui añadir Tiempos
            if (filteredTimesList.Any())
            {
                sosUpdateEntity.Times.RemoveAll(t => t.SOSTimeId == null || t.SOSTimeId <= 0);

                // Mapear nuevas tiempos
                List<SOSTime> newSOSTime = _mapper.Map<List<SOSTime>>(filteredTimesList);

                foreach (var time in newSOSTime)
                {
                    time.SOSTimeId = 0;
                    time.IsActive = true;
                }

                var resultAddSOSTime = await _ProcessRepository.AddRangeSOSTimes(newSOSTime);


                if (resultAddSOSTime != null)
                {
                    Debug.WriteLine("Add SOSTime añadidos con exitop");
                    Bkup_Times.AddRange(resultAddSOSTime);
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


            SOSDistribution _sosDistribution = await _ProcessRepository.GetSOSDistribution(sosDistribution_Id, true, true, true, true, includeTurns:true, includeTimes:true);

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
            //hacer update entity sin relaciones
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

            foreach (var time in sosUpdateEntity.Times)
            {
                var timeUpdate = await _ProcessRepository.UpdateTime(time);
                SOSTime timeBkaux = await _ProcessRepository.GetSOSTimeById(time.SOSTimeId);
                Bkup_Times.Add(timeBkaux);
            }

            foreach (var turn in sosUpdateEntity.Turns)
            {
                var turnUpdate = await _ProcessRepository.UpdateTurn(turn);
                Turn turnBkaux = await _ProcessRepository.GetTurnById(turn.TurnId);
                Bkup_Turn.Add(turnBkaux);
            }

            //Nulleamos el update para evitar errores
            sosUpdateEntity.Notes = null;
            sosUpdateEntity.Turns = null;
            sosUpdateEntity.DistributionLogbooks = null;
            sosUpdateEntity.Times= null;
            sosUpdateEntity.SOSDistributionAdditionalTime = null;


            await _ProcessRepository.RemoveAllTimesFromSOSDistribution(_sosDistribution);
            await _ProcessRepository.RemoveAllTurnsFromSOSDistribution(_sosDistribution);
            await _ProcessRepository.SOSDataRemoveAllNotesFromSOSDistribution(_sosDistribution);
            await _ProcessRepository.SOSDataRemoveAllSOSDistributionLogbookFromSOSDistribution(_sosDistribution);
            await _ProcessRepository.SOSDataRemoveAllSOSDistributionAdditionalTimeFromSOSDistribution(_sosDistribution);


            var result = await _ProcessRepository.UpdateSOSDistribution(sosUpdateEntity, _sosDistribution);

            //Notes - Volver a añádir las notas
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
            if (Bkup_Times.Any())
            {
                foreach (SOSTime time in Bkup_Times)
                {
                    await _ProcessRepository.AddSOSTimeToSOSDistribution(_sosDistribution, time);
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
