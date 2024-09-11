using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.SOS.SOSSequenceDtos;
using SupervisorMobility.API.Models.SOS.SOSSequenceLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSTimeDtos;
using System.Diagnostics;

namespace SupervisorMobility.API.Controllers.SOS_Controllers
{
    [Route("api/SOS/Sequence")]
    [ApiController]
    public class SequenceController : Controller
    {
        private readonly ISOS_ProcessRepository _ProcessRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        public SequenceController(IWebHostEnvironment env, IMapper mapper, ISOS_ProcessRepository repository)
        {
            _ProcessRepository = repository;
            _mapper = mapper ??
                  throw new ArgumentNullException(nameof(mapper));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        [HttpPost]
        public async Task<ActionResult<SOSSequenceDto>> GenerateSequence(SOSSequenceForCreateDto sOSSequenceToCreate, int SOSHubCollection_Id)
        {
            SOSHub SOSEntity = await _ProcessRepository.GetSOSHub(SOSHubCollection_Id, includeInformation: true);

            if (sOSSequenceToCreate.SOSSequenceId == 0)
            {
               

                sOSSequenceToCreate.CreatedDate = DateTime.Now;
                sOSSequenceToCreate.IsActive = true;

                sOSSequenceToCreate.SOSHubId = SOSHubCollection_Id;


                SOSSequence SequenceToCreate = _mapper.Map<SOSSequence>(sOSSequenceToCreate);

                var createdResult = await _ProcessRepository.CreateSOSSequence(SequenceToCreate);
                if (createdResult != null)
                    return Ok(SequenceToCreate);
                else
                    return BadRequest();
            }
            else
            {
                //only add revision
                SOSSequence _sosSequence = await _ProcessRepository.GetSOSSequence(sOSSequenceToCreate.SOSSequenceId, true, true, true, true, true, true);

                SOSSequenceLogbook _logbookToCreate = _mapper.Map<SOSSequenceLogbook>(sOSSequenceToCreate.SequenceLogbooks?.Last());
                _logbookToCreate.SOSSequenceId = _sosSequence.SOSSequenceId;

                var resultAddSections = await _ProcessRepository.CreateSOSSequenceLogbook(_logbookToCreate);

                if (resultAddSections > 0)
                {
                    Debug.WriteLine("SOSSequenceLogbook añadidas con exito");
                    await _ProcessRepository.AddSOSSequenceLogbookToSOSSequence(_sosSequence, _logbookToCreate);
                }
                else
                {
                    Debug.WriteLine("Error Sections añadidos");
                    return BadRequest();
                }



                return Ok(_sosSequence);
            }

        }

        [HttpGet("{id}", Name = "GetSOSSequence")]
        public async Task<ActionResult<SOSSequenceDto>> GetSOSSequence(int id, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false, bool includeImagesSOS = false)
        {

            var SOSSequence = await _ProcessRepository.GetSOSSequence(id, includeImages, includeNotes, includeLogbooks, includeSpecialCases, includeSOS, includeImagesSOS);
            if (SOSSequence == null)
            {
                return NotFound("SOSSequence not found!");
            }

            return Ok(_mapper.Map<SOSSequenceDto>(SOSSequence));
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<SOSSequenceDto>>> GetAllSOSSequence(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false)
        {

            var CheckpointEntities = await _ProcessRepository.GetAllSOSSequence(includeImages, includeNotes, includeLogbooks, includeSpecialCases, includeSOS);
            if (CheckpointEntities == null)
            {
                return NotFound("Get All Sos Analisis not found!");
            }

            return Ok(_mapper.Map<IEnumerable<SOSSequenceDto>>(CheckpointEntities));
        }

        //Update

        [HttpPut("{sosSequence_Id}")]
        public async Task<ActionResult> UpdateSOSSequence(int sosSequence_Id, SOSSequenceForUpdateDto sosUpdateEntity)
        {


            List<Commentary> Bkup_Notes = new List<Commentary>();
            List<SOSSequenceLogbook> Bkup_SequenceLogbook = new List<SOSSequenceLogbook>();
            List<SOSTime> Bkup_Times = new List<SOSTime>();

            // Filtrar nuevos Comentarios
            List<UpdateCommentaryDto> filteredCommentaryList = sosUpdateEntity.Notes.Where(t => t.CommentaryId <= 0).ToList();
            // Filtrar nuevos SequenceLogbooks
            List<SOSSequenceLogbookForUpdateDto> filteredSequenceLogbooksList = sosUpdateEntity.SequenceLogbooks.Where(t => t.SOSSequenceLogbookId <= 0).ToList();
            // Filtrar nuevos Tiempos
            List<SOSTimeForUpdateDto> filteredTimesList = sosUpdateEntity.Times.Where(t => t.SOSTimeId <= 0).ToList();


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



            // Remover nuevos SequenceLogbooks de la lista principal para evitar duplicados
            if (filteredSequenceLogbooksList.Any())
            {
                sosUpdateEntity.SequenceLogbooks.RemoveAll(t => t.SOSSequenceLogbookId == null || t.SOSSequenceLogbookId <= 0);

                // Mapear nuevas norms/standars
                List<SOSSequenceLogbook> newSOSSequenceLogbook = _mapper.Map<List<SOSSequenceLogbook>>(filteredSequenceLogbooksList);

                foreach (var SequenceLogbook in newSOSSequenceLogbook)
                {
                    SequenceLogbook.SOSSequenceLogbookId = 0;
                    SequenceLogbook.IsActive = true;
                }

                var resultAddSOSSequenceLogbook = await _ProcessRepository.AddRangeSOSSequenceLogbook(newSOSSequenceLogbook);

                if (resultAddSOSSequenceLogbook != null)
                {
                    Debug.WriteLine("AnalysisLogbook añadidos con exitop");
                    Bkup_SequenceLogbook.AddRange(resultAddSOSSequenceLogbook);
                }
                else
                {
                    Debug.WriteLine("Error AnalysisLogbook añadidos");
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


            SOSSequence _sosSequence = await _ProcessRepository.GetSOSSequence(sosSequence_Id, true, true, true, true);

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

            foreach (var logbook in sosUpdateEntity.SequenceLogbooks)
            {
                SOSSequenceLogbook SequenceBkaux = await _ProcessRepository.GetSOSSequenceLogbookById(logbook.SOSSequenceLogbookId);
                _mapper.Map(logbook, SequenceBkaux);
                Bkup_SequenceLogbook.Add(SequenceBkaux);
            }

            foreach (var time in sosUpdateEntity.Times)
            {
                var timeUpdate = await _ProcessRepository.UpdateTime(time);
                SOSTime timeBkaux = await _ProcessRepository.GetSOSTimeById(time.SOSTimeId);
                Bkup_Times.Add(timeBkaux);
            }

            //Nulleamos el update para evitar errores
            sosUpdateEntity.Notes = null;
            sosUpdateEntity.Times = null;
            sosUpdateEntity.SequenceLogbooks = null;

            await _ProcessRepository.SOSDataRemoveAllNotesFromSOSSequence(_sosSequence);
            await _ProcessRepository.SOSDataRemoveAllSOSSequenceLogbookFromSOSSequence(_sosSequence);
            await _ProcessRepository.RemoveAllTimesFromSOSSequence(_sosSequence);


            var result = await _ProcessRepository.UpdateSOSSequence(sosUpdateEntity, _sosSequence);

            //Notes - Volver a añádir las notas
            if (Bkup_Notes.Any())
            {
                foreach (Commentary Comment in Bkup_Notes)
                {
                    await _ProcessRepository.AddNoteToSOSSequence(_sosSequence, Comment);
                }
            }

            //Sequence Logbook
            if (Bkup_SequenceLogbook.Any())
            {
                foreach (SOSSequenceLogbook logbook in Bkup_SequenceLogbook)
                {
                    await _ProcessRepository.AddSOSSequenceLogbookToSOSSequence(_sosSequence, logbook);
                }
            }

            //Times
            if (Bkup_Times.Any())
            {
                foreach (SOSTime time in Bkup_Times)
                {
                    await _ProcessRepository.AddSOSTimeToSOSSequence(_sosSequence, time);
                }
            }


            if (result != null)
            {
                return Ok(_sosSequence);
            }
            else
                return BadRequest();

        }//end Update 

        [HttpDelete("{SOSSequenceId}")]
        public async Task<ActionResult<int>> RemoveSOSHub(int SOSSequenceId)
        {
            var result = await _ProcessRepository.RemoveSOSSequence(SOSSequenceId);

            var SOSHub = await _ProcessRepository.GetSOSSequence(SOSSequenceId);

            if (result > 0)
                return Ok(SOSHub);
            else
                return BadRequest("something wrong");
        }


        //ilustrations

        [HttpPost("Ilustrations/{Sequence_id}")]
        public async Task<ActionResult<FileUpload>> UploadIlustrations(int Sequence_id, IFormFile file)
        {

            var uploadResult = new FileUploadForCreationDto();
            string trustedFileNameForStorage = string.Empty;
            var unstrustedFileName = file.FileName;

            trustedFileNameForStorage = Path.GetRandomFileName();

            var path = Path.Combine(_env.ContentRootPath, "uploads\\SOSSequence\\Ilustrations", trustedFileNameForStorage);
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

            await _ProcessRepository.AddIlustrationToSOSSequence(Sequence_id, fileToReturn);
            await _ProcessRepository.SaveChangesAsync();

            return Ok(fileToReturn);
        }

        [HttpGet("Ilustrations/{fileid}")]
        public async Task<IActionResult> DownloadIlustrations(int fileid)
        {
            var FileInfo = await _ProcessRepository.FetchFileAsync(fileid);

            if (FileInfo is not null)
            {
                var path = Path.Combine(_env.ContentRootPath, "uploads\\SOSSequence\\Ilustrations", FileInfo.StorageFileName);

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

        [HttpDelete("Ilustrations/{SOS_SOSSequence_id}/remove/{ImageFile_id}")]
        public async Task<ActionResult<int>> RemoveImage(int SOS_SOSSequence_id, int ImageFile_id)
        {
            var result = await _ProcessRepository.RemoveIlustrationFromSOSSequence(SOS_SOSSequence_id, ImageFile_id);

            if (result > 0)
                return Ok();
            else
                return BadRequest("something went wrong");
        }


    }
}
