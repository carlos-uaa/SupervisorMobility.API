using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.CheckpointDtos;
using SupervisorMobility.API.DataAccess.Entities.IS;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.CheckpointNormDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.CheckpointNormDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.CheckpointNormDtos;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.FileUploadDto;

namespace SupervisorMobility.API.Controllers.IS_Controllers
{
    [Route("api/IS/Template/Checkpoints/Norms")]
    [ApiController]
    public class CheckpointNormsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IStampingRepository _stampingRepository;
        private readonly IWebHostEnvironment _env;
        private readonly SupervisorMobilityContext _context;
        public CheckpointNormsController(IStampingRepository stampingRepository, SupervisorMobilityContext context, IWebHostEnvironment env, IMapper mapper)
        {
            _stampingRepository = stampingRepository ??
                throw new ArgumentNullException(nameof(stampingRepository));
            _mapper = mapper ??
                  throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }


        [HttpPost]
        public async Task<ActionResult<CheckpointNormDto>> CreateDatePanelCategorie(CheckpointNormForCreateDto CheckpointNormSpectForCreate)
        {
            Checkpoint DPEntity = await _stampingRepository.getCheckpoint((int)CheckpointNormSpectForCreate.CheckpointId, includeStandars: true);

            CheckpointNormSpectForCreate.ItemOrder = await _stampingRepository.CheckpointNormMaxItemOrderAsync((int)CheckpointNormSpectForCreate.CheckpointId);

            CheckpointNorm DPSpecEntity = _mapper.Map<CheckpointNorm>(CheckpointNormSpectForCreate);

            var createdResult = await _stampingRepository.AddCheckpointNorm(DPEntity, DPSpecEntity);
            if (createdResult != null)
                return Ok(DPEntity);
            else
                return BadRequest(); ;

        }

      
        [HttpGet("{id}", Name = "GetCheckpointNorms")]
        public async Task<ActionResult<CheckpointNormDto>> GetCheckpointNormCategory(int id, bool includeSketches = false)
        {
            //Find Checklist category
            var CheckpointNormEntiti = await _stampingRepository.getCheckpointNorm(id, includeSketches);
            if (CheckpointNormEntiti == null)
            {
                return NotFound("Data Panel not found!");
            }

            return Ok(_mapper.Map<CheckpointNormDto>(CheckpointNormEntiti));
        }

        //[HttpPut("sequence/{dataSpecification_Id}")]
        //public async Task<ActionResult> UpdateDataSpecificationItemOrder(int dataSpecification_Id,
        //CheckpointNormForUpdateSequenceDto dataSpecification)
        //{
        //    var CheckpointNormEntity = await _stampingRepository.getCheckpointNorm(dataSpecification_Id);
        //    if (CheckpointNormEntity == null)
        //    {
        //        return NotFound("Data Panel Specification category not found.");
        //    }

        //    if (dataSpecification.ItemOrder == CheckpointNormEntity.ItemOrder)
        //    {
        //        return NoContent();
        //    }

        //    if (dataSpecification.ItemOrder < 1
        //        || dataSpecification.ItemOrder > await _stampingRepository.CheckpointNormMaxItemOrderAsync((int)CheckpointNormEntity.CheckpointNormId))
        //    {
        //        return BadRequest("ItemOrder must be greater than 1 and lower that the current max ItemOrder.");
        //    }

        //    var updateResult = await _stampingRepository.UpdateCheckpointNormSequenceAsync(dataSpecification, CheckpointNormEntity);

        //    if (updateResult > 0)
        //    {
        //        return Ok();
        //    }

        //    return NoContent();

        //}

        [HttpDelete("{CheckpointId}")]
        public async Task<ActionResult> DeleteCheckpoint(int CheckpointId)
        {
            CheckpointNorm? entityCheckpoint = await _stampingRepository.getCheckpointNorm(CheckpointId);

            var result = await _stampingRepository.removeCheckpointNorm(entityCheckpoint);

            if (result > 0)
                return Ok();
            else
                return BadRequest();
        }

        [HttpPost("UploadSkecth/{CheckpointNorm_id}")]
        public async Task<ActionResult<FileUpload>> UploadPartSkecth(int CheckpointNorm_id, IFormFile file)
        {

            var uploadResult = new FileUploadForCreationDto();
            string trustedFileNameForStorage = string.Empty;
            var unstrustedFileName = file.FileName;

            trustedFileNameForStorage = Path.GetRandomFileName();

            var path = Path.Combine(_env.ContentRootPath, "uploads\\ChekpointNormSkecth", trustedFileNameForStorage);
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
            await _stampingRepository.AddSketchChekpointNorm(CheckpointNorm_id, fileToReturn);
            await _stampingRepository.SaveChangesAsync();

            return Ok(fileToReturn);

        }

        [HttpGet("Sketch/{fileid}")]
        public async Task<IActionResult> DownloadPreviousEvidence(int fileid)
        {
            var FileInfo = await _stampingRepository.FetchFileAsync(fileid);

            if (FileInfo is not null)
            {
                var path = Path.Combine(_env.ContentRootPath, "uploads\\ChekpointNormSkecth", FileInfo.StorageFileName);

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
