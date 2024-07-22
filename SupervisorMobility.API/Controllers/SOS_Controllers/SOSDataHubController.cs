using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Drawing;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using KellermanSoftware.CompareNetObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.IS;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities.SOS.History;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.CheckpointDtos;
using SupervisorMobility.API.Models.SOS.EquipmentDtos;
using SupervisorMobility.API.Models.SOS.MaterialDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos.AnalysisBkupDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos.SectionDtos;
using SupervisorMobility.API.Models.SOS.ToolDtos;
using System.Diagnostics;

namespace SupervisorMobility.API.Controllers.SOS_Controllers
{
    [Route("api/SOS/DataPool")]
    [ApiController]
    public class SOSDataHubController : Controller
    {
        private readonly ISOSAnalysis_ProcessRepository _AnalysisProcessRepository;
        private readonly IMapper _mapper;
        //private readonly SupervisorMobilityContext _context;
        private readonly IWebHostEnvironment _env;

        public SOSDataHubController(ISOSAnalysis_ProcessRepository repository, IWebHostEnvironment env, IMapper mapper)
        {
            _AnalysisProcessRepository = repository;
            _mapper = mapper ??
                  throw new ArgumentNullException(nameof(mapper));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        [HttpPost]
        public async Task<ActionResult<SOSHubDto>> CreateSOSHub(SOSHubForCreateDto SOSHubForCreate)
        {
            List<Tool> tools = new List<Tool>();
            List<Material> materials = new List<Material>();
            List<Equipment> equipments = new List<Equipment>();

            foreach (var tool in SOSHubForCreate.ToolsUsed)
            {
                Tool toolaux = await _AnalysisProcessRepository.GetToolById(tool.ToolId);
                tools.Add(toolaux);
            }
            foreach (var material in SOSHubForCreate.MaterialsUsed)
            {
                Material mataux = await _AnalysisProcessRepository.GetMaterialById(material.MaterialId);
                materials.Add(mataux);
            }
            foreach (var equipment in SOSHubForCreate.SafetyEquipment)
            {
                Equipment equipmentaux = await _AnalysisProcessRepository.GetEquipmentById(equipment.EquipmentId);
                equipments.Add(equipmentaux);
            }

            SOSHubForCreate.ToolsUsed = null;
            SOSHubForCreate.MaterialsUsed = null;
            SOSHubForCreate.SafetyEquipment = null;

            SOSHub SOSEntity = new SOSHub();

            _mapper.Map(SOSHubForCreate, SOSEntity);

            var createdResult = await _AnalysisProcessRepository.CreateSOScollection(SOSEntity);

            if (tools.Any())
            {
                foreach (Tool tool in tools)
                {
                    _AnalysisProcessRepository.AddToolToSOSCollection(SOSEntity, tool);
                }
            }

            if (materials.Any())
            {
                foreach (Material material in materials)
                {
                    _AnalysisProcessRepository.AddMaterialToSOSCollection(SOSEntity, material);
                }
            }

            if (equipments.Any())
            {
                foreach (Equipment equipment in equipments)
                {
                    _AnalysisProcessRepository.AddEquipmentToSOSCollection(SOSEntity, equipment);
                }
            }

            await _AnalysisProcessRepository.SaveChangesAsync();

            if (createdResult != null)
            {
                return Ok(SOSEntity);
            }
            else
                return BadRequest();

        }

        //get
        [HttpGet("{id}", Name = "GetSOSHub")]
        public async Task<ActionResult<SOSHubDto>> GetSOSHub(int id, bool includeAnalysesBkup = false, bool includeSections = false, bool includeImages = false, bool includeVideos = false, bool includeCommentaries = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false, bool includeModel = false)
        {

            var SOSHub = await _AnalysisProcessRepository.GetSOSHub(id, includeAnalysesBkup, includeSections, includeImages, includeVideos, includeCommentaries, includeTools, includeEquipments, includeMaterials, includeInformation, includePeople, includeDocuments, includeModel);
            if (SOSHub == null)
            {
                return NotFound("SOSHub not found!");
            }

            return Ok(_mapper.Map<SOSHubDto>(SOSHub));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SOSHubDto>>> GetAllSOSHub(bool includeImages = false, bool includeVideos = false, bool includeCommentaries = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false)
        {

            var CheckpointEntities = await _AnalysisProcessRepository.GetAllSOSHub(includeImages, includeVideos, includeCommentaries, includeTools, includeEquipments, includeMaterials, includeInformation, includePeople, includeDocuments);
            if (CheckpointEntities == null)
            {
                return NotFound("Get All Sos Hub not found!");
            }

            return Ok(_mapper.Map<IEnumerable<SOSHubDto>>(CheckpointEntities));
        }

        //Update
        [HttpPut("{SOSHubId}")]
        public async Task<ActionResult<SOSHubDto>> UpdateSOSHub(int SOSHubId, SOSHubForUpdateDto _SOSHubForUpdate)
        {

            // Filtrar nuevos Comentarios
            List<UpdateCommentaryDto> filteredCommentaryList = _SOSHubForUpdate.ProcessSheetCommentary
                .Where(t => t.ComentaryId <= 0).ToList();


            // Remover nuevos Comentarios de la lista principal para evitar duplicados
            if (filteredCommentaryList.Any())
            {
                _SOSHubForUpdate.ProcessSheetCommentary.ToList().RemoveAll(t => t.ComentaryId == null || t.ComentaryId <= 0);

                // Mapear nuevas norms/standars
                List<Commentary> newCommentarys = _mapper.Map<List<Commentary>>(filteredCommentaryList);

                foreach (var newComentary in newCommentarys)
                {
                    newComentary.ComentaryId = 0;
                    newComentary.IsActive = true;
                }

                var resultAddCommentary = await _AnalysisProcessRepository.AddRangeCommentary(newCommentarys);

                if (resultAddCommentary > 0)
                {
                    Debug.WriteLine("Commentarios añadidos con exitop");
                }

                //_context.Comments.AddRange(newCommentarys);
                //await _context.SaveChangesAsync();

                List<UpdateCommentaryDto> newCommentarysCreated = _mapper.Map<List<UpdateCommentaryDto>>(newCommentarys);
                _SOSHubForUpdate.ProcessSheetCommentary.ToList().AddRange(newCommentarysCreated);
            }

            List<AnalysisBkupForUpdateDto> filteredAnalysisBkupList = _SOSHubForUpdate.AnalysesBkup
                .Where(t => t.AnalysisBkupId <= 0).ToList();

            // Remover nuevos AnalysisBkup de la lista principal para evitar duplicados
            if (filteredAnalysisBkupList.Any())
            {
                _SOSHubForUpdate.AnalysesBkup.ToList().RemoveAll(t => t.AnalysisBkupId == null || t.AnalysisBkupId <= 0);

                // Mapear nuevas AnalysisBkup
                List<AnalysisBkup> newAnalysisBkups = _mapper.Map<List<AnalysisBkup>>(filteredAnalysisBkupList);

                foreach (var newAnalysisBkup in newAnalysisBkups)
                {
                    newAnalysisBkup.AnalysisBkupId = 0;
                    newAnalysisBkup.IsActive = true;
                }

                var resultAddAnalysisBkup = await _AnalysisProcessRepository.AddRangeAnalysisBkup(newAnalysisBkups);

                if (resultAddAnalysisBkup > 0)
                {
                    Debug.WriteLine("AnalysisBkup añadidos con exitop");
                }

                // Mapear y agregar nuevas AnalysisBkup creadas al DTO de actualización
                List<AnalysisBkupForUpdateDto> newAnalysisBkupCreated = _mapper.Map<List<AnalysisBkupForUpdateDto>>(newAnalysisBkups);
                _SOSHubForUpdate.AnalysesBkup.ToList().AddRange(newAnalysisBkupCreated);
            }

            List<SectionForUpdateDto> filteredSectionList = _SOSHubForUpdate.Sections
              .Where(t => t.SectionId <= 0).ToList();

            // Remover nuevos Section de la lista principal para evitar duplicados
            if (filteredSectionList.Any())
            {
                _SOSHubForUpdate.Sections.ToList().RemoveAll(t => t.SectionId == null || t.SectionId <= 0);

                // Mapear nuevas sections
                List<Section> newSections = _mapper.Map<List<Section>>(filteredSectionList);

                foreach (var newSec in newSections)
                {
                    newSec.SectionId = 0;
                    newSec.IsActive = true;
                }

                var resultAddSections = await _AnalysisProcessRepository.AddRangeSections(newSections);

                if (resultAddSections > 0)
                {
                    Debug.WriteLine("Sections añadidas con exitop");
                }

                // Mapear y agregar nuevas secciones creadas al DTO de actualización
                List<SectionForUpdateDto> newSectionsCreated = _mapper.Map<List<SectionForUpdateDto>>(newSections);
                _SOSHubForUpdate.Sections.ToList().AddRange(newSectionsCreated);
            }


            SOSHub entitySOSHub = await _AnalysisProcessRepository.GetSOSHub(SOSHubId, true, true, true, true, true, true, true, true, true, true, true);


            string jsonResult = CompareAndGenerateJson(_mapper.Map<SOSHubForUpdateDto>(entitySOSHub), _SOSHubForUpdate);

            SOSHubHistory newHistory = new SOSHubHistory();
            _mapper.Map(entitySOSHub, newHistory);
            newHistory.VersionChanges = jsonResult;

            await _AnalysisProcessRepository.CreateHistorySOScollection(newHistory);




            List<Commentary> ProcessSheetCommentaries = new List<Commentary>();
            List<AnalysisBkup> AnalysisBkups = new List<AnalysisBkup>();
            List<Section> Sections = new List<Section>();
            List<Tool> tools = new List<Tool>();
            List<Material> materials = new List<Material>();
            List<Equipment> equipments = new List<Equipment>();

            foreach (var commentary in _SOSHubForUpdate.ProcessSheetCommentary)
            {
                Commentary analysisBkaux = await _AnalysisProcessRepository.GetCommentaryById(commentary.ComentaryId);
                ProcessSheetCommentaries.Add(analysisBkaux);
            }

            foreach (var analysisBkup in _SOSHubForUpdate.AnalysesBkup)
            {
                AnalysisBkup analysisBkaux = await _AnalysisProcessRepository.GetAnalysisBkupId(analysisBkup.AnalysisBkupId);
                AnalysisBkups.Add(analysisBkaux);
            }

            foreach (var section in _SOSHubForUpdate.Sections)
            {
                Section sectionToAdd = await _AnalysisProcessRepository.GetSectionById(section.SectionId);
                Sections.Add(sectionToAdd);
            }

            foreach (var tool in _SOSHubForUpdate.ToolsUsed)
            {
                Tool toolaux = await _AnalysisProcessRepository.GetToolById(tool.ToolId);
                tools.Add(toolaux);
            }
            foreach (var material in _SOSHubForUpdate.MaterialsUsed)
            {
                Material mataux = await _AnalysisProcessRepository.GetMaterialById(material.MaterialId);
                materials.Add(mataux);
            }
            foreach (var equipment in _SOSHubForUpdate.SafetyEquipment)
            {
                Equipment equipmentaux = await _AnalysisProcessRepository.GetEquipmentById(equipment.EquipmentId);
                equipments.Add(equipmentaux);
            }

            _SOSHubForUpdate.ProcessSheetCommentary = null;
            _SOSHubForUpdate.AnalysesBkup = null;
            _SOSHubForUpdate.Sections = null;
            _SOSHubForUpdate.ToolsUsed = null;
            _SOSHubForUpdate.MaterialsUsed = null;
            _SOSHubForUpdate.SafetyEquipment = null;


            await _AnalysisProcessRepository.SOSDataRemoveAllProcessSheetCommentary(entitySOSHub);
            await _AnalysisProcessRepository.SOSDataRemoveAllAnalysisBkups(entitySOSHub);
            await _AnalysisProcessRepository.SOSDataRemoveAllSections(entitySOSHub);
            await _AnalysisProcessRepository.SOSDataRemoveAllToolsEquipmentMaterial(entitySOSHub);


            if (entitySOSHub == null)
            {
                return NotFound();
            }

            var result = await _AnalysisProcessRepository.UpdateSOSHub(_SOSHubForUpdate, entitySOSHub);

            //ProcessSheetCommentaries
            if (ProcessSheetCommentaries.Any())
            {
                foreach (Commentary Comment in ProcessSheetCommentaries)
                {
                    _AnalysisProcessRepository.AddProcessSheetCommentaryToSOSCollection(entitySOSHub, Comment);
                }
            }
            //Sections
            if (Sections.Any())
            {
                foreach (Section sec in Sections)
                {
                    _AnalysisProcessRepository.AddSectionSOSCollection(entitySOSHub, sec);
                }
            }
            //Analysis Backups
            if (AnalysisBkups.Any())
            {
                foreach (AnalysisBkup analysisBk in AnalysisBkups)
                {
                    _AnalysisProcessRepository.AddAnaysisBkupToSOSCollection(entitySOSHub, analysisBk);
                }
            }
            //Tools
            if (tools.Any())
            {
                foreach (Tool tool in tools)
                {
                    _AnalysisProcessRepository.AddToolToSOSCollection(entitySOSHub, tool);
                }
            }
            //Materials
            if (materials.Any())
            {
                foreach (Material material in materials)
                {
                    _AnalysisProcessRepository.AddMaterialToSOSCollection(entitySOSHub, material);
                }
            }
            //Equipments
            if (equipments.Any())
            {
                foreach (Equipment equipment in equipments)
                {
                    _AnalysisProcessRepository.AddEquipmentToSOSCollection(entitySOSHub, equipment);
                }
            }

            await _AnalysisProcessRepository.AddHistoryToSOSCollection(entitySOSHub, newHistory);

            if (result != null)
            {
                return Ok(entitySOSHub);
            }
            else
                return BadRequest();

        }

        static string CompareAndGenerateJson(SOSHubForUpdateDto obj1, SOSHubForUpdateDto obj2)
        {
            var compareLogic = new CompareLogic
            {
                Config = new ComparisonConfig
                {
                    CompareChildren = true,
                    MaxDifferences = int.MaxValue
                }
            };

            ComparisonResult result = compareLogic.Compare(obj1, obj2);

            var differencesList = new List<DifferenceDetail>();

            foreach (var difference in result.Differences)
            {
                var differenceDetail = new DifferenceDetail
                {
                    Property = difference.PropertyName,
                    Before = difference.Object1Value?.ToString(),
                    After = difference.Object2Value?.ToString()
                };
                differencesList.Add(differenceDetail);
            }

            ValueConverter<List<DifferenceDetail>, string> jsonListConverter = new ValueConverter<List<DifferenceDetail>, string>(
                        v => JsonConvert.SerializeObject(v),
                        v => JsonConvert.DeserializeObject<List<DifferenceDetail>>(v)
                    );

            string jsonResult = (string)jsonListConverter.ConvertToProvider(differencesList);
            return jsonResult;

        }

        public class DifferenceDetail
        {
            public string Property { get; set; }
            public string Before { get; set; }
            public string After { get; set; }
        }

        [HttpDelete("{SOSHubId}")]
        public async Task<ActionResult<int>> RemoveSOSHub(int SOSHubId)
        {
            var result = await _AnalysisProcessRepository.RemoveSOSHub(SOSHubId);

            var SOSHub = await _AnalysisProcessRepository.GetSOSHub(SOSHubId);

            if (result > 0)
                return Ok(SOSHub);
            else
                return BadRequest("something wrong");
        }

        #region UploadFiles

        [HttpPost("Image/{pool_id}")]
        public async Task<ActionResult<FileUpload>> UploadImage(int pool_id, IFormFile file)
        {

            var uploadResult = new FileUploadForCreationDto();
            string trustedFileNameForStorage = string.Empty;
            var unstrustedFileName = file.FileName;

            trustedFileNameForStorage = Path.GetRandomFileName();

            var path = Path.Combine(_env.ContentRootPath, "uploads\\SOSData\\Images", trustedFileNameForStorage);
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

            await _AnalysisProcessRepository.AddImageToSOSData(pool_id, fileToReturn);
            await _AnalysisProcessRepository.SaveChangesAsync();

            return Ok(fileToReturn);
        }

        [HttpPost("Video/{pool_id}")]
        public async Task<ActionResult<FileUpload>> UploadVideo(int pool_id, IFormFile file)
        {

            var uploadResult = new FileUploadForCreationDto();
            string trustedFileNameForStorage = string.Empty;
            var unstrustedFileName = file.FileName;

            trustedFileNameForStorage = Path.GetRandomFileName();

            var path = Path.Combine(_env.ContentRootPath, "uploads\\SOSData\\Videos", trustedFileNameForStorage);
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

            await _AnalysisProcessRepository.AddVideoToSOSData(pool_id, fileToReturn);
            await _AnalysisProcessRepository.SaveChangesAsync();

            return Ok(fileToReturn);
        }

        [HttpPost("CD/{pool_id}")]
        public async Task<ActionResult<FileUpload>> UploadCD(int pool_id, IFormFile file)
        {

            var uploadResult = new FileUploadForCreationDto();
            string trustedFileNameForStorage = string.Empty;
            var unstrustedFileName = file.FileName;

            trustedFileNameForStorage = Path.GetRandomFileName();

            var path = Path.Combine(_env.ContentRootPath, "uploads\\SOSData\\CommonDirection", trustedFileNameForStorage);
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

            await _AnalysisProcessRepository.AddImageToSOSData(pool_id, fileToReturn);
            await _AnalysisProcessRepository.SaveChangesAsync();

            return Ok(fileToReturn);
        }

        [HttpGet("Image/{fileid}")]
        public async Task<IActionResult> DownloadImage(int fileid)
        {
            var FileInfo = await _AnalysisProcessRepository.FetchFileAsync(fileid);

            if (FileInfo is not null)
            {
                var path = Path.Combine(_env.ContentRootPath, "uploads\\SOSData\\Images", FileInfo.StorageFileName);

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

        [HttpGet("Video/{fileid}")]
        public async Task<IActionResult> DownloadVideo(int fileid)
        {
            var FileInfo = await _AnalysisProcessRepository.FetchFileAsync(fileid);

            if (FileInfo is not null)
            {
                var path = Path.Combine(_env.ContentRootPath, "uploads\\SOSData\\Videos", FileInfo.StorageFileName);

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

        [HttpGet("CD/{fileid}")]
        public async Task<IActionResult> DownloadCD(int fileid)
        {
            var FileInfo = await _AnalysisProcessRepository.FetchFileAsync(fileid);

            if (FileInfo is not null)
            {
                var path = Path.Combine(_env.ContentRootPath, "uploads\\SOSData\\CommonDirection", FileInfo.StorageFileName);

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
        #endregion

        [HttpDelete("Image/{pool_id}/remove/{fileUploadId}")]
        public async Task<ActionResult<int>> RemoveImage(int pool_id, int fileUploadId)
        {
            var result = await _AnalysisProcessRepository.RemoveImageFromSOSData(pool_id, fileUploadId);

            if (result > 0)
                return Ok();
            else
                return BadRequest("something wrong");
        }

        [HttpDelete("Video/{pool_id}/remove/{fileUploadId}")]
        public async Task<ActionResult<int>> RemoveVideo(int pool_id, int fileUploadId)
        {
            var result = await _AnalysisProcessRepository.RemoveVideoFromSOSData(pool_id, fileUploadId);

            if (result > 0)
                return Ok();
            else
                return BadRequest("something wrong");
        }

        [HttpDelete("CD/{pool_id}/remove/{fileUploadId}")]
        public async Task<ActionResult<int>> RemoveCD(int pool_id, int fileUploadId)
        {
            var result = await _AnalysisProcessRepository.RemoveCDFromSOSData(pool_id, fileUploadId);

            if (result > 0)
                return Ok();
            else
                return BadRequest("something wrong");
        }
        /// Subir y borrar documento common direction


        //History
        [HttpGet("{id}/History", Name = "GetSOSHubHistory")]

        public async Task<ActionResult<List<SOSHubDto>>> GetSOSHubHistory(int id, bool includeAnalysesBkup = false, bool includeSections = false, bool includeImages = false, bool includeVideos = false, bool includeCommentaries = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false)
        {

            var SOSHubs = await _AnalysisProcessRepository.GetAllHistorySOSHub(id, includeAnalysesBkup, includeSections, includeImages, includeVideos, includeCommentaries, includeTools, includeEquipments, includeMaterials, includeInformation, includePeople, includeDocuments);
            if (SOSHubs == null)
            {
                return NotFound("SOSHub History not found!");
            }

            return Ok(_mapper.Map<List<SOSHubDto>>(SOSHubs));
        }
    }// End SOS Data pool controller
}//end namespace
