using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.CheckpointDtos;
using SupervisorMobility.API.DataAccess.Entities.IS;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.CheckpointNormDtos;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.FileUploadDto;

namespace SupervisorMobility.API.Controllers.IS_Controllers
{
    [Route("api/IS/Template/Checkpoints")]
    [ApiController]
    public class CheckpointController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IStampingRepository _stampingRepository;
        private readonly IWebHostEnvironment _env;
        private readonly SupervisorMobilityContext _context;
        public CheckpointController(IStampingRepository stampingRepository, SupervisorMobilityContext context, IWebHostEnvironment env, IMapper mapper)
        {
            _stampingRepository = stampingRepository ??
                throw new ArgumentNullException(nameof(stampingRepository));
            _mapper = mapper ??
                  throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        [HttpPost]
        public async Task<ActionResult<CheckpointDto>> CreateCheckpoint(CheckpointForCreateDto CheckpointForCreate)
        {
            Checkpoint DPEntity = _mapper.Map<Checkpoint>(CheckpointForCreate);
            DPEntity.ItemOrder = await _stampingRepository.CheckpointMaxItemOrderAsync();

            if (DPEntity.Standars?.Count > 0)
            {
                foreach (var (item, index) in DPEntity.Standars?.Select((item, index) => (item, index)))
                {
                    item.CheckpointNormId = 0;
                    item.ItemOrder = index + 1;
                }
            }

            var createdResult = await _stampingRepository.AddCheckpoint(DPEntity);
            if (createdResult != null)
                return Ok(DPEntity);
            else
                return BadRequest(); ;

        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<CheckpointDto>>> GetCheckpointCategories(bool includeStandars = false, bool includeSketches = false, bool includeSketchesStandars = false)
        {

            var CheckpointEntities = await _stampingRepository.getAllCheckpoints(includeStandars, includeSketches, includeSketchesStandars);
            if (CheckpointEntities == null)
            {
                return NotFound("CheckpointEntities categories not found!");
            }

            return Ok(_mapper.Map<IEnumerable<CheckpointDto>>(CheckpointEntities));
        }

        [HttpGet("{id}", Name = "GetCheckpointCategory")]
        public async Task<ActionResult<CheckpointDto>> GetCheckpointCategory(int id, bool includeStandars = false, bool includeSketches = false, bool includeSketchesStandars = false)
        {
            //Find Checklist category
            var CheckpointCategory = await _stampingRepository.getCheckpoint(id, includeStandars, includeSketches, includeSketchesStandars);
            if (CheckpointCategory == null)
            {
                return NotFound("Data Panel not found!");
            }

            return Ok(_mapper.Map<CheckpointDto>(CheckpointCategory));
        }

        [HttpPut("{CheckpointId}")]
        public async Task<ActionResult<CheckpointDto>> UpdateCheckpoint(int CheckpointId, CheckpointForUpdateDto _CheckpointForUpdate)
        {

            // Obtener el Checkpoint existente junto con sus norms/standars
            Checkpoint entityCheckpoint = await _stampingRepository.getCheckpoint(CheckpointId, includeStandars: true);

            if (entityCheckpoint == null)
            {
                return NotFound();
            }

            // Filtrar nuevas norms/standars
            List<CheckpointNormForUpdateDto> filteredList = _CheckpointForUpdate.Standars
                .Where(t => t.CheckpointNormId <= 0).ToList();

            // Remover nuevas norms/standars de la lista principal para evitar duplicados
            if (filteredList.Any())
            {
                _CheckpointForUpdate.Standars.ToList().RemoveAll(t => t.CheckpointNormId == null || t.CheckpointNormId <= 0);

                // Mapear nuevas norms/standars
                List<CheckpointNorm> newStandars = _mapper.Map<List<CheckpointNorm>>(filteredList);

                int sequence = await _stampingRepository.CheckpointNormMaxItemOrderAsync(CheckpointId);

                foreach (var newNorm in newStandars)
                {
                    newNorm.CheckpointNormId = 0;
                    newNorm.CheckpointId = CheckpointId;
                    newNorm.ItemOrder = sequence++;
                }

                _context.CheckpointsNorm.AddRange(newStandars);
                await _context.SaveChangesAsync();

                // Mapear y agregar nuevas norms/standars creadas al DTO de actualización
                List<CheckpointNormForUpdateDto> newStandarsCreated = _mapper.Map<List<CheckpointNormForUpdateDto>>(newStandars);
                _CheckpointForUpdate.Standars.ToList().AddRange(newStandarsCreated);
            }

            // Actualizar las propiedades del Checkpoint
            entityCheckpoint.IsActive = _CheckpointForUpdate.IsActive;
            entityCheckpoint.ItemOrder = _CheckpointForUpdate.ItemOrder;
            entityCheckpoint.CheckpointTitle = _CheckpointForUpdate.CheckpointTitle;
            entityCheckpoint.CheckpointDescription = _CheckpointForUpdate.CheckpointDescription;

            // Manejar norms/standars existentes
            foreach (var specDto in _CheckpointForUpdate.Standars)
            {
                var existingSpec = entityCheckpoint.Standars
                    .FirstOrDefault(s => s.CheckpointNormId == specDto.CheckpointNormId);

                if (existingSpec != null)
                {
                    existingSpec.IsActive = specDto.IsActive;
                    existingSpec.ItemOrder = specDto.ItemOrder;
                    existingSpec.Standard = specDto.Standard;
                }
            }

            // Guardar los cambios en el Checkpoint y sus norms/standars
            _context.Checkpoints.Update(entityCheckpoint);
            var result = await _context.SaveChangesAsync();

            //var result = await _stampingRepository.UpdateCheckpoint(_CheckpointForUpdate, entityCheckpoint);

            if (result > 0)
                return Ok(entityCheckpoint);
            else
                return BadRequest();
        }



        //[HttpPut("sequence/{Checkpoint_Id}")]
        //public async Task<ActionResult> UpdateCheckpointItemOrder(int Checkpoint_Id,
        //   CheckpointForUpdateSequenceDto Checkpoint)
        //{
        //    var CheckpointEntity = await _stampingRepository.getCheckpoint(Checkpoint_Id);
        //    if (CheckpointEntity == null)
        //    {
        //        return NotFound("Data Panel category not found.");
        //    }

        //    if (Checkpoint.ItemOrder == CheckpointEntity.ItemOrder)
        //    {
        //        return NoContent();
        //    }

        //    if (Checkpoint.ItemOrder < 1
        //        || Checkpoint.ItemOrder > await _stampingRepository.CheckpointMaxItemOrderAsync())
        //    {
        //        return BadRequest("ItemOrder must be greater than 1 and lower that the current max ItemOrder.");
        //    }

        //    var updateResult = await _stampingRepository.UpdateCheckpointsSequenceAsync(Checkpoint, CheckpointEntity);

        //    if (updateResult > 0)
        //    {
        //        return Ok();
        //    }

        //    return NoContent();

        //}

        [HttpDelete("{CheckpointId}")]
        public async Task<ActionResult> DeleteCheckpoint(int CheckpointId)
        {
            Checkpoint? entityCheckpoint = await _stampingRepository.getCheckpoint(CheckpointId);

            var result = await _stampingRepository.removeCheckpoint(entityCheckpoint);

            if (result > 0)
                return Ok();
            else
                return BadRequest();
        }

        [HttpPost("UploadSkecth/{Checkpoint_id}")]
        public async Task<ActionResult<FileUpload>> UploadPartSkecth(int Checkpoint_id, IFormFile file)
        {

            var uploadResult = new FileUploadForCreationDto();
            string trustedFileNameForStorage = string.Empty;
            var unstrustedFileName = file.FileName;

            trustedFileNameForStorage = Path.GetRandomFileName();

            var path = Path.Combine(_env.ContentRootPath, "uploads\\ChekpointSkecth", trustedFileNameForStorage);
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

            var fileToReturn = await _stampingRepository.CreateFileAsync(uploadResult);
            await _stampingRepository.AddSketchCheckpoint(Checkpoint_id, fileToReturn);
            await _stampingRepository.SaveChangesAsync();

            return Ok(fileToReturn);

        }

        [HttpGet("Sketch/{fileid}")]
        public async Task<IActionResult> DownloadPreviousEvidence(int fileid)
        {
            var FileInfo = await _stampingRepository.FetchFileAsync(fileid);

            if (FileInfo is not null)
            {
                var path = Path.Combine(_env.ContentRootPath, "uploads\\ChekpointSkecth", FileInfo.StorageFileName);

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


        [HttpGet("{CheckpointId}/Sketch/{fileUploadId}/remove")]
        public async Task<ActionResult<int>> RemoveEvidence(int CheckpointId, int fileUploadId)
        {
            await _stampingRepository.RemoveSketchCheckPoint(CheckpointId, fileUploadId);
            await _stampingRepository.SaveChangesAsync();
            return Ok();
        }
    }
}
