using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.IS;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.CheckpointDtos;
using SupervisorMobility.API.Models.SOS.EquipmentDtos;
using SupervisorMobility.API.Models.SOS.MaterialDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
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
            SOSHub SOSEntity = _mapper.Map<SOSHub>(SOSHubForCreate);

            //Comentarios 
            if (SOSEntity.ProcessSheetCommentary?.Count > 0)
            {
                foreach (var (item, index) in SOSEntity.ProcessSheetCommentary?.Select((item, index) => (item, index)))
                {
                    item.ComentaryId = 0;
                }
            }

            // Filtrar nuevos Equipment
            List<EquipmentDto> filteredEquipmentList = SOSHubForCreate.SafetyEquipment
               .Where(t => t.EquipmentId <= 0).ToList();
            // Filtrar nuevos ToolsUsed
            List<ToolDto> filteredToolsUsedList = SOSHubForCreate.ToolsUsed
               .Where(t => t.ToolId <= 0).ToList();
            // Filtrar nuevos Materials
            List<MaterialDto> filteredMaterialsList = SOSHubForCreate.MaterialsUsed
               .Where(t => t.MaterialId <= 0).ToList();


            // Remover nuevos Equipment de la lista principal para evitar duplicados
            if (filteredEquipmentList.Any())
            {
                SOSHubForCreate.SafetyEquipment.ToList().RemoveAll(t => t.EquipmentId == null || t.EquipmentId <= 0);

                // Mapear nuevas norms/standars
                List<Equipment> newEquipments = _mapper.Map<List<Equipment>>(filteredEquipmentList);

                foreach (var newEquipment in newEquipments)
                {
                    newEquipment.EquipmentId = 0;
                    newEquipment.IsActive = true;
                }

                var resultAddEquipment = await _AnalysisProcessRepository.AddRangeEquipment(newEquipments);

                if (resultAddEquipment > 0)
                {
                    Debug.WriteLine("Equipaments añadidos con exitop");
                }
                // Mapear y agregar nuevas norms/standars creadas al DTO de actualización
                List<EquipmentDto> newEquipmentsCreated = _mapper.Map<List<EquipmentDto>>(newEquipments);
                SOSHubForCreate.SafetyEquipment.ToList().AddRange(newEquipmentsCreated);
            }

            // Remover nuevos ToolsUsed de la lista principal para evitar duplicados
            if (filteredToolsUsedList.Any())
            {
                SOSHubForCreate.ToolsUsed.ToList().RemoveAll(t => t.ToolId == null || t.ToolId <= 0);

                // Mapear nuevas norms/standars
                List<Tool> newToolsUseds = _mapper.Map<List<Tool>>(filteredToolsUsedList);

                foreach (var newTool in newToolsUseds)
                {
                    newTool.ToolId = 0;
                    newTool.IsActive = true;
                }

                var resultAddToolsUsed = await _AnalysisProcessRepository.AddRangeTool(newToolsUseds);

                if (resultAddToolsUsed > 0)
                {
                    Debug.WriteLine("Tools añadidos con exitop");
                }
                // Mapear y agregar nuevas norms/standars creadas al DTO de actualización
                List<ToolDto> newToolsUsedsCreated = _mapper.Map<List<ToolDto>>(newToolsUseds);
                SOSHubForCreate.ToolsUsed.ToList().AddRange(newToolsUsedsCreated);
            }

            // Remover nuevos Materials de la lista principal para evitar duplicados
            if (filteredMaterialsList.Any())
            {
                SOSHubForCreate.MaterialsUsed.ToList().RemoveAll(t => t.MaterialId == null || t.MaterialId <= 0);

                // Mapear nuevas norms/standars
                List<Material> newMaterials = _mapper.Map<List<Material>>(filteredMaterialsList);

                foreach (var newComentary in newMaterials)
                {
                    newComentary.MaterialId = 0;
                    newComentary.IsActive = true;
                }

                var resultAddMaterial = await _AnalysisProcessRepository.AddRangeMaterial(newMaterials);

                if (resultAddMaterial > 0)
                {
                    Debug.WriteLine("Materials añadidos con exitop");
                }
                // Mapear y agregar nuevas norms/standars creadas al DTO de actualización
                List<MaterialDto> newMaterialsCreated = _mapper.Map<List<MaterialDto>>(newMaterials);
                SOSHubForCreate.MaterialsUsed.ToList().AddRange(newMaterialsCreated);
            }



            var createdResult = await _AnalysisProcessRepository.CreateSOScollection(SOSEntity);
            if (createdResult != null)
                return Ok(SOSEntity);
            else
                return BadRequest(); ;

        }

        //get
        [HttpGet("{id}", Name = "GetSOSHub")]
        public async Task<ActionResult<SOSHubDto>> GetSOSHub(int id, bool includeImages = false, bool includeVideos = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false)
        {

            var SOSHub = await _AnalysisProcessRepository.GetSOSHub(id, includeImages, includeVideos, includeTools, includeEquipments, includeMaterials, includeInformation, includePeople, includeDocuments);
            if (SOSHub == null)
            {
                return NotFound("SOSHub not found!");
            }

            return Ok(_mapper.Map<SOSHubDto>(SOSHub));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SOSHubDto>>> GetAllSOSHub(bool includeImages = false, bool includeVideos = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false)
        {

            var CheckpointEntities = await _AnalysisProcessRepository.GetAllSOSHub(includeImages, includeVideos, includeTools, includeEquipments, includeMaterials, includeInformation, includePeople, includeDocuments);
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

            // Obtener el SOSHub existente junto con sus norms/standars
            SOSHub entitySOSHub = await _AnalysisProcessRepository.GetSOSHub(SOSHubId, false, false, true, true, true, true, true);

            if (entitySOSHub == null)
            {
                return NotFound();
            }

            // Filtrar nuevos Comentarios
            List<UpdateCommentaryDto> filteredCommentaryList = _SOSHubForUpdate.ProcessSheetCommentary
                .Where(t => t.ComentaryId <= 0).ToList();
            // Filtrar nuevos Equipment
            List<EquipmentForUpdateDto> filteredEquipmentList = _SOSHubForUpdate.SafetyEquipment
               .Where(t => t.EquipmentId <= 0).ToList();
            // Filtrar nuevos ToolsUsed
            List<ToolForUpdateDto> filteredToolsUsedList = _SOSHubForUpdate.ToolsUsed
               .Where(t => t.ToolId <= 0).ToList();
            // Filtrar nuevos Materials
            List<MaterialForUpdateDto> filteredMaterialsList = _SOSHubForUpdate.MaterialsUsed
               .Where(t => t.MaterialId <= 0).ToList();

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
                // Mapear y agregar nuevas norms/standars creadas al DTO de actualización
                List<UpdateCommentaryDto> newCommentarysCreated = _mapper.Map<List<UpdateCommentaryDto>>(newCommentarys);
                _SOSHubForUpdate.ProcessSheetCommentary.ToList().AddRange(newCommentarysCreated);
            }

            // Remover nuevos Equipment de la lista principal para evitar duplicados
            if (filteredEquipmentList.Any())
            {
                _SOSHubForUpdate.SafetyEquipment.ToList().RemoveAll(t => t.EquipmentId == null || t.EquipmentId <= 0);

                // Mapear nuevas norms/standars
                List<Equipment> newEquipments = _mapper.Map<List<Equipment>>(filteredEquipmentList);

                foreach (var newEquipment in newEquipments)
                {
                    newEquipment.EquipmentId = 0;
                    newEquipment.IsActive = true;
                }

                var resultAddEquipment = await _AnalysisProcessRepository.AddRangeEquipment(newEquipments);

                if (resultAddEquipment > 0)
                {
                    Debug.WriteLine("Equipaments añadidos con exitop");
                }
                // Mapear y agregar nuevas norms/standars creadas al DTO de actualización
                List<EquipmentForUpdateDto> newEquipmentsCreated = _mapper.Map<List<EquipmentForUpdateDto>>(newEquipments);
                _SOSHubForUpdate.SafetyEquipment.ToList().AddRange(newEquipmentsCreated);
            }

            // Remover nuevos ToolsUsed de la lista principal para evitar duplicados
            if (filteredToolsUsedList.Any())
            {
                _SOSHubForUpdate.ToolsUsed.ToList().RemoveAll(t => t.ToolId == null || t.ToolId <= 0);

                // Mapear nuevas norms/standars
                List<Tool> newToolsUseds = _mapper.Map<List<Tool>>(filteredToolsUsedList);

                foreach (var newTool in newToolsUseds)
                {
                    newTool.ToolId = 0;
                    newTool.IsActive = true;
                }

                var resultAddToolsUsed = await _AnalysisProcessRepository.AddRangeTool(newToolsUseds);

                if (resultAddToolsUsed > 0)
                {
                    Debug.WriteLine("Tools añadidos con exitop");
                }
                // Mapear y agregar nuevas norms/standars creadas al DTO de actualización
                List<ToolForUpdateDto> newToolsUsedsCreated = _mapper.Map<List<ToolForUpdateDto>>(newToolsUseds);
                _SOSHubForUpdate.ToolsUsed.ToList().AddRange(newToolsUsedsCreated);
            }

            // Remover nuevos Materials de la lista principal para evitar duplicados
            if (filteredMaterialsList.Any())
            {
                _SOSHubForUpdate.MaterialsUsed.ToList().RemoveAll(t => t.MaterialId == null || t.MaterialId <= 0);

                // Mapear nuevas norms/standars
                List<Material> newMaterials = _mapper.Map<List<Material>>(filteredMaterialsList);

                foreach (var newComentary in newMaterials)
                {
                    newComentary.MaterialId = 0;
                    newComentary.IsActive = true;
                }

                var resultAddMaterial = await _AnalysisProcessRepository.AddRangeMaterial(newMaterials);

                if (resultAddMaterial > 0)
                {
                    Debug.WriteLine("Materials añadidos con exitop");
                }
                // Mapear y agregar nuevas norms/standars creadas al DTO de actualización
                List<MaterialForUpdateDto> newMaterialsCreated = _mapper.Map<List<MaterialForUpdateDto>>(newMaterials);
                _SOSHubForUpdate.MaterialsUsed.ToList().AddRange(newMaterialsCreated);
            }



            var result = await _AnalysisProcessRepository.UpdateSOSHub(_SOSHubForUpdate, entitySOSHub);

            if (result > 0)
                return Ok(entitySOSHub);
            else
                return BadRequest();
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

            await _AnalysisProcessRepository.AddImageToSOSData(pool_id, fileToReturn);
            await _AnalysisProcessRepository.SaveChangesAsync();

            return Ok(fileToReturn);
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

        /// Subir y borrar documento common direction

    }// End SOS Data pool controller
}//end namespace
