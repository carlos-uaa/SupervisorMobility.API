using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities.SOS.History;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.SOS.EquipmentDtos;
using SupervisorMobility.API.Models.SOS.MaterialDtos;
using SupervisorMobility.API.Models.SOS.SOSAnalysisDtos;
using SupervisorMobility.API.Models.SOS.SOSAnalysisLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.Models.SOS.ToolDtos;
using SupervisorMobility.API.Models.SOSReviewDtos;
using System.Diagnostics;

namespace SupervisorMobility.API.Controllers.SOS_Controllers
{
    [Route("api/SOS/Analysis")]
    [ApiController]
    public class AnalysisController : Controller
    {
        private readonly ISOS_ProcessRepository _ProcessRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        public AnalysisController(IWebHostEnvironment env, IMapper mapper, ISOS_ProcessRepository repository)
        {
            _ProcessRepository = repository;
            _mapper = mapper ??
                  throw new ArgumentNullException(nameof(mapper));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        [HttpPost]
        public async Task<ActionResult<SOSAnalysisDto>> GenerateAnalysis(SOSAnalysisForCreateDto sOSAnalysisToCreate, int SOSHubCollection_Id)
        {
            SOSHub SOSEntity = await _ProcessRepository.GetSOSHub(SOSHubCollection_Id, includeInformation: true);

            if(sOSAnalysisToCreate.SOSAnalysisId == 0)
            {
                //Nombre del documento GOS o processShet
                //sOSAnalysisToCreate.InternalControlNumber = SOSEntity.Folio;
                //sOSAnalysisToCreate.ProcessName = SOSEntity.ProcessSheet;

                sOSAnalysisToCreate.CreatedDate = DateTime.Now;
                sOSAnalysisToCreate.IsActive = true;

                sOSAnalysisToCreate.SOSHubId = SOSHubCollection_Id;


                SOSAnalysis AnalysisToCreate = _mapper.Map<SOSAnalysis>(sOSAnalysisToCreate);
                
                var createdResult = await _ProcessRepository.CreateSOSAnalysis(AnalysisToCreate);
                if (createdResult != null)
                    return Ok(AnalysisToCreate);
                else
                    return BadRequest(); 
            }
            else
            {
                //only add revision
                SOSAnalysis _sosAnalysis = await _ProcessRepository.GetSOSAnalysis(sOSAnalysisToCreate.SOSAnalysisId, true, true, true, true,true,true);
              
                SOSAnalysisLogbook _logbookToCreate = _mapper.Map<SOSAnalysisLogbook>(sOSAnalysisToCreate.AnalysisLogbooks?.Last());
                _logbookToCreate.SOSAnalysisId = _sosAnalysis.SOSAnalysisId;

                var resultAddSections = await _ProcessRepository.CreateSOSAnalysisLogbook(_logbookToCreate);

                if (resultAddSections > 0)
                {
                    Debug.WriteLine("SOSAnalysisLogbook añadidas con exito");
                await _ProcessRepository.AddSOSAnalysisLogbookToSOSAnalysis(_sosAnalysis, _logbookToCreate);
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

            var SOSAnalysis = await _ProcessRepository.GetSOSAnalysis(id, includeImages, includeNotes, includeLogbooks, includeSpecialCases, includeSOS, includeImagesSOS);
            if (SOSAnalysis == null)
            {
                return NotFound("SOSAnalysis not found!");
            }

            return Ok(_mapper.Map<SOSAnalysisDto>(SOSAnalysis));
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<SOSAnalysisDto>>> GetAllSOSAnalysis(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false)
        {

            var CheckpointEntities = await _ProcessRepository.GetAllSOSAnalysis(includeImages, includeNotes, includeLogbooks, includeSpecialCases, includeSOS);
            if (CheckpointEntities == null)
            {
                return NotFound("Get All Sos Analisis not found!");
            }

            return Ok(_mapper.Map<IEnumerable<SOSAnalysisDto>>(CheckpointEntities));
        }

        //Update

        [HttpPut("{sosAnalysis_Id}")]
        public async Task<ActionResult> UpdateSOSAnalysis(int sosAnalysis_Id, SOSAnalysisForUpdateDto sosUpdateEntity)
        {
            List<Commentary> Bkup_Notes = new List<Commentary>();
            List<SOSAnalysisLogbook> Bkup_AnalysisLogbook = new List<SOSAnalysisLogbook>();

            // Filtrar nuevos Comentarios
            List<UpdateCommentaryDto> filteredCommentaryList = sosUpdateEntity.Notes.Where(t => t.CommentaryId <= 0).ToList();
            // Filtrar nuevos AnalysisLogbooks
            List<SOSAnalysisLogbookForUpdateDto> filteredAnalysisLogbooksList = sosUpdateEntity.AnalysisLogbooks.Where(t => t.SOSAnalysisLogbookId <= 0).ToList();
           

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

                var resultAddSOSAnalysisLogbook = await _ProcessRepository.AddRangeSOSAnalysisLogbook(newSOSAnalysisLogbook);

                if (resultAddSOSAnalysisLogbook > 0)
                {
                    Debug.WriteLine("SOSAnalysisLogbook añadidos con exitop");
                }

                List<SOSAnalysisLogbookForUpdateDto> newSOSAnalysisLogbookCreated = _mapper.Map<List<SOSAnalysisLogbookForUpdateDto>>(newSOSAnalysisLogbook);
                sosUpdateEntity.AnalysisLogbooks.ToList().AddRange(newSOSAnalysisLogbookCreated);
            }

            SOSAnalysis _sosAnalysis = await _ProcessRepository.GetSOSAnalysis(sosAnalysis_Id, true, true, true, true);

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
                var analysisUpdate = await _ProcessRepository.UpdateAnalysisLogbook(logbook);
                SOSAnalysisLogbook analysisBkaux = await _ProcessRepository.GetSOSAnalysisLogbookById(logbook.SOSAnalysisLogbookId);
                Bkup_AnalysisLogbook.Add(analysisBkaux);
            }
           


            //Nulleamos el update para evitar errores
            sosUpdateEntity.Notes = null;
            sosUpdateEntity.AnalysisLogbooks = null;

            await _ProcessRepository.SOSDataRemoveAllNotesFromSOSAnalysis(_sosAnalysis);
            await _ProcessRepository.SOSDataRemoveAllSOSAnalysisLogbookFromSOSAnalysis(_sosAnalysis);

            var result = await _ProcessRepository.UpdateSOSAnalysis(sosUpdateEntity, _sosAnalysis);

            //Notes - Volver a añádir las notas
            if (Bkup_Notes.Any())
            {
                foreach (Commentary Comment in Bkup_Notes)
                {
                  await  _ProcessRepository.AddNoteToSOSAnalysis(_sosAnalysis, Comment);
                }
            }
            
            //Analysis Logbook
            if (Bkup_AnalysisLogbook.Any())
            {
                foreach (SOSAnalysisLogbook logbook in Bkup_AnalysisLogbook)
                {
                   await _ProcessRepository.AddSOSAnalysisLogbookToSOSAnalysis(_sosAnalysis, logbook);
                }
            }



            if (result != null)
            {
                return Ok(_sosAnalysis);
            }
            else
                return BadRequest();

        }//end Update 

        [HttpDelete("{SOSAnaysisId}")]
        public async Task<ActionResult<int>> RemoveSOSHub(int SOSAnaysisId)
        {
            var result = await _ProcessRepository.RemoveSOSAnalysis(SOSAnaysisId);

            var SOSHub = await _ProcessRepository.GetSOSHub(SOSAnaysisId);

            if (result > 0)
                return Ok(SOSHub);
            else
                return BadRequest("something wrong");
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

            await _ProcessRepository.AddIlustrationToSOSAnalysis(analysis_id, fileToReturn);
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
            var result = await _ProcessRepository.RemoveIlustrationFromSOSAnalysis(SOS_SOSAnalysis_id, ImageFile_id);

            if (result > 0)
                return Ok();
            else
                return BadRequest("something went wrong");
        }

    }
}
