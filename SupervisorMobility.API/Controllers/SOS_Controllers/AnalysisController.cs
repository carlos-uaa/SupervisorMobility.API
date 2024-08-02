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
using SupervisorMobility.API.Models.SOS.SpecialCaseAbnormalSituationDtos;
using SupervisorMobility.API.Models.SOS.ToolDtos;
using SupervisorMobility.API.Models.SOSReviewDtos;
using System.Diagnostics;

namespace SupervisorMobility.API.Controllers.SOS_Controllers
{
    [Route("api/SOS/Analysis")]
    [ApiController]
    public class AnalysisController : Controller
    {
        private readonly ISOSAnalysis_ProcessRepository _AnalysisProcessRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly SupervisorMobilityContext _context;
        public AnalysisController(IWebHostEnvironment env, SupervisorMobilityContext context, IMapper mapper, ISOSAnalysis_ProcessRepository repository)
        {
            _AnalysisProcessRepository = repository;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ??
                  throw new ArgumentNullException(nameof(mapper));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        [HttpPost]
        public async Task<ActionResult<SOSAnalysisDto>> GenerateAnalysis(SOSAnalysisForCreateDto sOSAnalysisToCreate, int SOSHubCollection_Id)
        {
            SOSHub SOSEntity = await _AnalysisProcessRepository.GetSOSHub(SOSHubCollection_Id, includeInformation: true);

            //Nombre del documento GOS o processShet
            sOSAnalysisToCreate.OperationName = SOSEntity.ProcessSheet;
            sOSAnalysisToCreate.InternalControlNumber = SOSEntity.ProcessSheet;
            sOSAnalysisToCreate.ProcessName = SOSEntity.ProcessSheet;

            sOSAnalysisToCreate.CreatedDate = DateTime.Now;
            sOSAnalysisToCreate.IsActive = true;

            sOSAnalysisToCreate.SOSHubId = SOSHubCollection_Id;

            SOSAnalysis AnalysisToCreate = _mapper.Map<SOSAnalysis>(sOSAnalysisToCreate);

            var createdResult = await _AnalysisProcessRepository.CreateSOSAnalysis(AnalysisToCreate);
            if (createdResult != null)
                return Ok(sOSAnalysisToCreate);
            else
                return BadRequest(); ;

        }

        [HttpGet("{id}", Name = "GetSOSAnalysis")]
        public async Task<ActionResult<SOSAnalysisDto>> GetSOSAnalysis(int id, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false, bool includeImagesSOS = false)
        {

            var SOSAnalysis = await _AnalysisProcessRepository.GetSOSAnalysis(id, includeImages, includeNotes, includeLogbooks, includeSpecialCases, includeSOS, includeImagesSOS);
            if (SOSAnalysis == null)
            {
                return NotFound("SOSAnalysis not found!");
            }

            return Ok(_mapper.Map<SOSAnalysisDto>(SOSAnalysis));
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<SOSAnalysisDto>>> GetAllSOSAnalysis(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false)
        {

            var CheckpointEntities = await _AnalysisProcessRepository.GetAllSOSAnalysis(includeImages, includeNotes, includeLogbooks, includeSpecialCases, includeSOS);
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



            // Filtrar nuevos Comentarios
            List<UpdateCommentaryDto> filteredCommentaryList = sosUpdateEntity.Notes.Where(t => t.CommentaryId <= 0).ToList();
            // Filtrar nuevos AnalysisLogbooks
            List<SOSAnalysisLogbookForUpdateDto> filteredAnalysisLogbooksList = sosUpdateEntity.AnalysisLogbooks.Where(t => t.SOSAnalysisLogbookId <= 0).ToList();
            // Filtrar nuevos SpecialCasesAbnormalSituations
            List<SpecialCaseAbnormalSituationForUpdateDto> filteredSpecialCasesAbnormalSituationsList = sosUpdateEntity.SpecialCasesAbnormalSituations.Where(t => t.SpecialCaseAbnormalSituationId <= 0).ToList();


            // Remover nuevos Comentarios de la lista principal para evitar duplicados
            if (filteredCommentaryList.Any())
            {
                sosUpdateEntity.Notes.ToList().RemoveAll(t => t.CommentaryId == null || t.CommentaryId <= 0);

                // Mapear nuevas norms/standars
                List<Commentary> newCommentarys = _mapper.Map<List<Commentary>>(filteredCommentaryList);

                foreach (var newComentary in newCommentarys)
                {
                    newComentary.CommentaryId = 0;
                    newComentary.IsActive = true;
                }

                var resultAddCommentary = await _AnalysisProcessRepository.AddRangeCommentary(newCommentarys);

                if (resultAddCommentary != null)
                {
                    Debug.WriteLine("Commentarios añadidos con exitop");
                }

                List<UpdateCommentaryDto> newCommentarysCreated = _mapper.Map<List<UpdateCommentaryDto>>(newCommentarys);
                sosUpdateEntity.Notes.ToList().AddRange(newCommentarysCreated);
            }

            // Remover nuevos SpecialCaseAbnormalSituationId de la lista principal para evitar duplicados
            if (filteredSpecialCasesAbnormalSituationsList.Any())
            {
                sosUpdateEntity.SpecialCasesAbnormalSituations.ToList().RemoveAll(t => t.SpecialCaseAbnormalSituationId == null || t.SpecialCaseAbnormalSituationId <= 0);

                // Mapear nuevas SpecialCasesAbnormalSituations
                List<SpecialCaseAbnormalSituation> newSpecialCasesAbnormalSituations = _mapper.Map<List<SpecialCaseAbnormalSituation>>(filteredSpecialCasesAbnormalSituationsList);

                foreach (var newSpecialCases in newSpecialCasesAbnormalSituations)
                {
                    newSpecialCases.SpecialCaseAbnormalSituationId = 0;
                    newSpecialCases.IsActive = true;
                }

                var resultAddSpecialCaseAbnormalSituation = await _AnalysisProcessRepository.AddRangeSpecialCasesAbnormalSituations(newSpecialCasesAbnormalSituations);

                if (resultAddSpecialCaseAbnormalSituation > 0)
                {
                    Debug.WriteLine("SpecialCaseAbnormalSituation añadidos con exitop");
                }

                List<SpecialCaseAbnormalSituationForUpdateDto> newSpecialCaseAbnormalSituationCreated = _mapper.Map<List<SpecialCaseAbnormalSituationForUpdateDto>>(newSpecialCasesAbnormalSituations);
                sosUpdateEntity.SpecialCasesAbnormalSituations.ToList().AddRange(newSpecialCaseAbnormalSituationCreated);
            }

            // Remover nuevos AnalysisLogbooks de la lista principal para evitar duplicados
            if (filteredAnalysisLogbooksList.Any())
            {
                sosUpdateEntity.AnalysisLogbooks.ToList().RemoveAll(t => t.SOSAnalysisLogbookId == null || t.SOSAnalysisLogbookId <= 0);

                // Mapear nuevas norms/standars
                List<SOSAnalysisLogbook> newSOSAnalysisLogbook = _mapper.Map<List<SOSAnalysisLogbook>>(filteredAnalysisLogbooksList);

                foreach (var analysisLogbook in newSOSAnalysisLogbook)
                {
                    analysisLogbook.SOSAnalysisLogbookId = 0;
                    analysisLogbook.IsActive = true;
                }

                var resultAddSOSAnalysisLogbook = await _AnalysisProcessRepository.AddRangeSOSAnalysisLogbook(newSOSAnalysisLogbook);

                if (resultAddSOSAnalysisLogbook > 0)
                {
                    Debug.WriteLine("SOSAnalysisLogbook añadidos con exitop");
                }

                List<SOSAnalysisLogbookForUpdateDto> newSOSAnalysisLogbookCreated = _mapper.Map<List<SOSAnalysisLogbookForUpdateDto>>(newSOSAnalysisLogbook);
                sosUpdateEntity.AnalysisLogbooks.ToList().AddRange(newSOSAnalysisLogbookCreated);
            }

            SOSAnalysis _sosAnalysis = await _AnalysisProcessRepository.GetSOSAnalysis(sosAnalysis_Id, true, true, true, true);

            ////Aqui va el historico de ser necesario en  un futuro 

            ////Ejemplo de uso 
            ////Compare genera un string que menciona las diferencias
            ////string jsonResult = CompareAndGenerateJson(_mapper.Map<SOSHubForUpdateDto>(entitySOSHub), _SOSHubForUpdate);
            ////se crea un entity 
            ////SOSHubHistory newHistory = new SOSHubHistory();
            ////_mapper.Map(entitySOSHub, newHistory);
            ////newHistory.VersionChanges = jsonResult;
            ////se almacena la entity anterior y se le añade el resumen de cambios
            ////await _AnalysisProcessRepository.CreateHistorySOScollection(newHistory);


            List<Commentary> Bkup_Notes = new List<Commentary>();
            List<SOSAnalysisLogbook> Bkup_AnalysisLogbook = new List<SOSAnalysisLogbook>();
            List<SpecialCaseAbnormalSituation> Bkup_SpecialCases = new List<SpecialCaseAbnormalSituation>();

            //Crear bkup de datos relacionados
            //hacer update entity sin relaciones
            foreach (var note in sosUpdateEntity.Notes)
            {
                Commentary analysisBkaux = await _AnalysisProcessRepository.GetCommentaryById(note.CommentaryId);
                _mapper.Map(note, analysisBkaux);
                Bkup_Notes.Add(analysisBkaux);
            }
            foreach (var logbook in sosUpdateEntity.AnalysisLogbooks)
            {
                SOSAnalysisLogbook analysisBkaux = await _AnalysisProcessRepository.GetSOSAnalysisLogbookById(logbook.SOSAnalysisLogbookId);
                _mapper.Map(logbook, analysisBkaux);
                Bkup_AnalysisLogbook.Add(analysisBkaux);
            }
            foreach (var specialCase in sosUpdateEntity.SpecialCasesAbnormalSituations)
            {
                SpecialCaseAbnormalSituation analysisBkaux = await _AnalysisProcessRepository.GetSpecialCaseAbnormalSituationById(specialCase.SpecialCaseAbnormalSituationId);
                _mapper.Map(specialCase, analysisBkaux);
                Bkup_SpecialCases.Add(analysisBkaux);
            }

            //Nulleamos el update para evitar errores
            sosUpdateEntity.Notes = null;
            sosUpdateEntity.AnalysisLogbooks = null;
            sosUpdateEntity.SpecialCasesAbnormalSituations = null;

            await _AnalysisProcessRepository.SOSDataRemoveAllNotesFromSOSAnalysis(_sosAnalysis);
            await _AnalysisProcessRepository.SOSDataRemoveAllSOSAnalysisLogbookFromSOSAnalysis(_sosAnalysis);
            await _AnalysisProcessRepository.SOSDataRemoveAllSpecialCasesAbnormalSituationsFromSOSAnalysis(_sosAnalysis);

            var result = await _AnalysisProcessRepository.UpdateSOSAnalysis(sosUpdateEntity, _sosAnalysis);

            //Notes - Volver a añádir las notas
            if (Bkup_Notes.Any())
            {
                foreach (Commentary Comment in Bkup_Notes)
                {
                    _AnalysisProcessRepository.AddNoteToSOSAnalysis(_sosAnalysis, Comment);
                }
            }
            //SpecialCases
            if (Bkup_SpecialCases.Any())
            {
                foreach (SpecialCaseAbnormalSituation specialcase in Bkup_SpecialCases)
                {
                    _AnalysisProcessRepository.AddSpecialCasesAbnormalSituationsToSOSAnalysis(_sosAnalysis, specialcase);
                }
            }
            //Analysis Logbook
            if (Bkup_AnalysisLogbook.Any())
            {
                foreach (SOSAnalysisLogbook logbook in Bkup_AnalysisLogbook)
                {
                    _AnalysisProcessRepository.AddSOSAnalysisLogbookToSOSAnalysis(_sosAnalysis, logbook);
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
            var result = await _AnalysisProcessRepository.RemoveSOSAnalysis(SOSAnaysisId);

            var SOSHub = await _AnalysisProcessRepository.GetSOSHub(SOSAnaysisId);

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

            var fileToReturn = await _AnalysisProcessRepository.CreateFileAsync(uploadResult);

            await _AnalysisProcessRepository.AddIlustrationToSOSAnalysis(analysis_id, fileToReturn);
            await _AnalysisProcessRepository.SaveChangesAsync();

            return Ok(fileToReturn);
        }

        [HttpGet("Ilustrations/{fileid}")]
        public async Task<IActionResult> DownloadIlustrations(int fileid)
        {
            var FileInfo = await _AnalysisProcessRepository.FetchFileAsync(fileid);

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



    }
}
