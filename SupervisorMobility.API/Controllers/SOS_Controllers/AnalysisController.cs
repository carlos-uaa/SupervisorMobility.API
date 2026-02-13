using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities.SOS.History;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.DataAccess.Services.SOS_AnalysisRepository;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.SOS.EquipmentDtos;
using SupervisorMobility.API.Models.SOS.MaterialDtos;
using SupervisorMobility.API.Models.SOS.SOSAnalysisDtos;
using SupervisorMobility.API.Models.SOS.SOSAnalysisLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.Models.SOS.SOSTimeDtos;
using SupervisorMobility.API.Models.SOS.ToolDtos;
using SupervisorMobility.API.Models.SOSReviewDtos;
using System.Diagnostics;

namespace SupervisorMobility.API.Controllers.SOS_Controllers
{
    [Route("api/SOS/Analysis")]
    [ApiController]
    public class AnalysisController : ControllerBase
    {
        private readonly ISOS_ProcessRepository _ProcessRepository;
        private readonly ISOS_AnalysisRepository _AnalysusRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        public AnalysisController(IWebHostEnvironment env, IMapper mapper, ISOS_ProcessRepository repository, ISOS_AnalysisRepository analysisRepository)
        {
            _ProcessRepository = repository;
            _mapper = mapper ??
                  throw new ArgumentNullException(nameof(mapper));
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _AnalysusRepository = analysisRepository;
        }

        [HttpPost]
        public async Task<ActionResult<SOSAnalysisDto>> GenerateAnalysis(SOSAnalysisForCreateDto sOSAnalysisToCreate, int SOSHubCollection_Id)
        {
            SOSHub SOSEntity = await _ProcessRepository.GetSOSHub(SOSHubCollection_Id, includeInformation: true);

            if (sOSAnalysisToCreate.SOSAnalysisId == 0)
            {
                //Nombre del documento GOS o processShet
                //sOSAnalysisToCreate.InternalControlNumber = SOSEntity.Folio;
                //sOSAnalysisToCreate.ProcessName = SOSEntity.ProcessSheet;

                sOSAnalysisToCreate.CreatedDate = DateTime.Now;
                sOSAnalysisToCreate.IsActive = true;

                sOSAnalysisToCreate.SOSHubId = SOSHubCollection_Id;


                SOSAnalysis AnalysisToCreate = _mapper.Map<SOSAnalysis>(sOSAnalysisToCreate);

                var createdResult = await _AnalysusRepository.CreateSOSAnalysis(AnalysisToCreate);
                if (createdResult != null)
                    return Ok(AnalysisToCreate);
                else
                    return BadRequest();
            }
            else
            {
                //only add revision
                SOSAnalysis _sosAnalysis = await _AnalysusRepository.GetSOSAnalysis(sOSAnalysisToCreate.SOSAnalysisId, true, true, true, true, true, true);

                SOSAnalysisLogbook _logbookToCreate = _mapper.Map<SOSAnalysisLogbook>(sOSAnalysisToCreate.AnalysisLogbooks?.Last());
                _logbookToCreate.SOSAnalysisId = _sosAnalysis.SOSAnalysisId;

                var resultAddSections = await _AnalysusRepository.CreateSOSAnalysisLogbook(_logbookToCreate);

                if (resultAddSections > 0)
                {
                    Debug.WriteLine("SOSAnalysisLogbook añadidas con exito");
                    await _AnalysusRepository.AddSOSAnalysisLogbookToSOSAnalysis(_sosAnalysis, _logbookToCreate);
                }
                else
                {
                    Debug.WriteLine("Error Sections añadidos");
                    return BadRequest();
                }



                return Ok(_sosAnalysis);
            }

        }

        [HttpGet("{id}", Name = "GetSOSAnalysis")]
        public async Task<ActionResult<SOSAnalysisDto>> GetSOSAnalysis(int id, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false, bool includeImagesSOS = false)
        {

            var SOSAnalysis = await _AnalysusRepository.GetSOSAnalysis(id, includeImages, includeNotes, includeLogbooks, includeSpecialCases, includeSOS, includeImagesSOS);
            if (SOSAnalysis == null)
            {
                return NotFound("SOSAnalysis not found!");
            }

            return Ok(_mapper.Map<SOSAnalysisDto>(SOSAnalysis));
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<SOSAnalysisDto>>> GetAllSOSAnalysis(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false)
        {

            var CheckpointEntities = await _AnalysusRepository.GetAllSOSAnalysis(includeImages, includeNotes, includeLogbooks, includeSpecialCases, includeSOS);
            if (CheckpointEntities == null)
            {
                return NotFound("Get All Sos Analisis not found!");
            }

            return Ok(_mapper.Map<IEnumerable<SOSAnalysisDto>>(CheckpointEntities));
        }

         [HttpGet("byDistribution")]
        public async Task<ActionResult<IEnumerable<SOSAnalysisDto>>> GetAllSOSAnalysisbyDistribution(int Distribution_Id, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false)
        {

            var CheckpointEntities = await _AnalysusRepository.GetAllSOSAnalysisByDistribution(Distribution_Id, includeImages, includeNotes, includeLogbooks, includeSpecialCases, includeSOS);
            if (CheckpointEntities == null)
            {
                return NotFound("Get All Sos Analisis not found!");
            }

            return Ok(_mapper.Map<IEnumerable<SOSAnalysisDto>>(CheckpointEntities));
        }

        // Emndpoint para obtener todos los análisis por área
        [HttpGet("byArea")]
        public async Task<ActionResult<IEnumerable<SOSAnalysisDto>>> GetAllSOSAnalysisbyArea(int Area_Id, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false)
        {
            var CheckpointEntities = await _AnalysusRepository.GetAllSOSAnalysisByArea(Area_Id, includeImages, includeNotes, includeLogbooks, includeSpecialCases, includeSOS);
            if (CheckpointEntities == null)
                return NotFound("Get All Sos Analisis not found!");

            return Ok(_mapper.Map<IEnumerable<SOSAnalysisDto>>(CheckpointEntities));
        }

        //Update

        [HttpPut("{sosAnalysis_Id}")]
        public async Task<ActionResult> UpdateSOSAnalysis(int sosAnalysis_Id, int userId, SOSAnalysisForUpdateDto sosUpdateEntity)
        {
            List<Commentary> Bkup_Notes = new List<Commentary>();
            List<SOSAnalysisLogbook> Bkup_AnalysisLogbook = new List<SOSAnalysisLogbook>();
            List<SOSTime> Bkup_Times = new List<SOSTime>();

            // Filtrar nuevos Comentarios
            List<UpdateCommentaryDto> filteredCommentaryList = sosUpdateEntity.Notes.Where(t => t.CommentaryId <= 0).ToList();
            // Filtrar nuevos AnalysisLogbooks
            List<SOSAnalysisLogbookForUpdateDto> filteredAnalysisLogbooksList = sosUpdateEntity.AnalysisLogbooks.Where(t => t.SOSAnalysisLogbookId <= 0).ToList();
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



            // Remover nuevos AnalysisLogbooks de la lista principal para evitar duplicados
            if (filteredAnalysisLogbooksList.Any())
            {
                sosUpdateEntity.AnalysisLogbooks.RemoveAll(t => t.SOSAnalysisLogbookId == null || t.SOSAnalysisLogbookId <= 0);

                // Mapear nuevas norms/standars
                List<SOSAnalysisLogbook> newSOSAnalysisLogbook = _mapper.Map<List<SOSAnalysisLogbook>>(filteredAnalysisLogbooksList);

                foreach (var analysisLogbook in newSOSAnalysisLogbook)
                {
                    analysisLogbook.SOSAnalysisLogbookId = 0;
                    analysisLogbook.IsActive = true;
                }

                var resultAddSOSAnalysisLogbook = await _AnalysusRepository.AddRangeSOSAnalysisLogbook(newSOSAnalysisLogbook);


                if (resultAddSOSAnalysisLogbook != null)
                {
                    Debug.WriteLine("AnalysisLogbook añadidos con exitop");
                    Bkup_AnalysisLogbook.AddRange(resultAddSOSAnalysisLogbook);
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


            SOSAnalysis _sosAnalysis = await _AnalysusRepository.GetSOSAnalysis(sosAnalysis_Id, true, true, true, true);

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

            foreach (var logbook in sosUpdateEntity.AnalysisLogbooks)
            {
                var analysisUpdate = await _AnalysusRepository.UpdateAnalysisLogbook(logbook);
                SOSAnalysisLogbook analysisBkaux = await _AnalysusRepository.GetSOSAnalysisLogbookById(logbook.SOSAnalysisLogbookId);
                Bkup_AnalysisLogbook.Add(analysisBkaux);
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
            sosUpdateEntity.AnalysisLogbooks = null;

            await _AnalysusRepository.SOSDataRemoveAllNotesFromSOSAnalysis(_sosAnalysis);
            await _ProcessRepository.RemoveAllTimesFromSOSAnalysis(_sosAnalysis);
            await _AnalysusRepository.SOSDataRemoveAllSOSAnalysisLogbookFromSOSAnalysis(_sosAnalysis);

            var result = await _AnalysusRepository.UpdateSOSAnalysis(sosUpdateEntity, _sosAnalysis);

            //Notes - Volver a añádir las notas
            if (Bkup_Notes.Any())
            {
                foreach (Commentary Comment in Bkup_Notes)
                {
                    await _AnalysusRepository.AddNoteToSOSAnalysis(_sosAnalysis, Comment);
                }
            }

            if (Bkup_Times.Any())
            {
                foreach (SOSTime time in Bkup_Times)
                {
                    await _ProcessRepository.AddSOSTimeToSOSAnalysis(_sosAnalysis, time);
                }
            }

            //Analysis Logbook
            if (Bkup_AnalysisLogbook.Any())
            {
                foreach (SOSAnalysisLogbook logbook in Bkup_AnalysisLogbook)
                {
                    await _AnalysusRepository.AddSOSAnalysisLogbookToSOSAnalysis(_sosAnalysis, logbook);
                }
            }


            //Aqui iba el flujo :'v
            //pero esta mal xd

            if (result != null)
            {
                return Ok(_sosAnalysis);
            }
            else
                return BadRequest();

        }//end Update 


        [HttpDelete("{SOSAnalysisId}")]
        public async Task<ActionResult<int>> RemoveSOSHub(int SOSAnalysisId)
        {
            try
            {
                var result = await _AnalysusRepository.RemoveSOSAnalysis(SOSAnalysisId);

                if (result > 0)
                    return Ok();
                else
                    return BadRequest("something wrong");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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

            var fileToReturn = await _ProcessRepository.CreateFileAsync(uploadResult);

            await _AnalysusRepository.AddIlustrationToSOSAnalysis(analysis_id, fileToReturn);
            await _ProcessRepository.SaveChangesAsync();

            return Ok(fileToReturn);
        }

        [HttpGet("Ilustrations/{fileid}")]
        public async Task<IActionResult> DownloadIlustrations(int fileid)
        {
            var FileInfo = await _ProcessRepository.FetchFileAsync(fileid);

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

        [HttpDelete("Ilustrations/{SOS_SOSAnalysis_id}/remove/{ImageFile_id}")]
        public async Task<ActionResult<int>> RemoveImage(int SOS_SOSAnalysis_id, int ImageFile_id)
        {
            var result = await _AnalysusRepository.RemoveIlustrationFromSOSAnalysis(SOS_SOSAnalysis_id, ImageFile_id);

            if (result > 0)
                return Ok();
            else
                return BadRequest("something went wrong");
        }

     

    }

}
