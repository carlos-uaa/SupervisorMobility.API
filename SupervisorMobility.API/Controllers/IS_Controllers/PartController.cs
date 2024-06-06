using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.IS;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelSpecificationDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.PartDtos;

namespace SupervisorMobility.API.Controllers.IS_Controllers
{
    [Route("api/IS/Part")]
    [ApiController]
    public class PartController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IStampingRepository _stampingRepository;
        private readonly IWebHostEnvironment _env;
        public PartController(IStampingRepository stampingRepository, IWebHostEnvironment env, IMapper mapper)
        {
            _stampingRepository = stampingRepository ??
                throw new ArgumentNullException(nameof(stampingRepository));
            _mapper = mapper ??
                  throw new ArgumentNullException(nameof(mapper));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        [HttpPost]
        public async Task<ActionResult<DataPanelDto>> CreatePart(PartForCreateDto PartForCreate)
        {
            Part PartEntity = _mapper.Map<Part>(PartForCreate);

            var createdResult = await _stampingRepository.AddPart(PartEntity);
            if (createdResult != null)
                return Ok(PartEntity);
            else
                return BadRequest(); ;

        }

        [HttpGet("{part_id}", Name = "GetPart")]
        public async Task<ActionResult<PartDto>> GetPart(int part_id, bool includeScketes = false)
        {

            var partEntity = await _stampingRepository.GetPart(part_id, includeScketes);
            if (partEntity == null)
            {
                return NotFound("Part not found!");
            }

            return Ok(_mapper.Map<PartDto>(partEntity));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PartDto>>> GetAllParts(bool includeScketes = false)
        {

            var partEntity = await _stampingRepository.GetAllParts(includeScketes);
            if (partEntity == null)
            {
                return NotFound("Part not found!");
            }

            return Ok(_mapper.Map<IEnumerable<PartDto>>(partEntity));
        }

        [HttpPut("{part_id}")]
        public async Task<ActionResult<PartDto>> UpdatePart(int part_id, PartForUpdateDto _PartForUpdate)
        {

            var partEntity = await _stampingRepository.GetPart(part_id, true);
            if (partEntity == null)
            {
                return NotFound("Part not found!");
            }

            var result = await _stampingRepository.UpdatePart(_PartForUpdate, partEntity);

            if (result > 0)
                return Ok(partEntity);
            else
                return BadRequest();
        }

        [HttpDelete("{part_id}")]
        public async Task<ActionResult> DeletePart(int part_id)
        {
            Part? entityDataPanel = await _stampingRepository.GetPart(part_id);

            var result = await _stampingRepository.DeletePart(entityDataPanel);

            if (result > 0)
                return Ok();
            else
                return BadRequest();
        }

        [HttpPost("UploadPartSkecth/{part_id}")]
        public async Task<ActionResult<FileUpload>> UploadPartSkecth(int part_id, IFormFile file)
        {

            var uploadResult = new FileUploadForCreationDto();
            string trustedFileNameForStorage = string.Empty;
            var unstrustedFileName = file.FileName;

            trustedFileNameForStorage = Path.GetRandomFileName();

            var path = Path.Combine(_env.ContentRootPath, "uploads\\PartsSkecth", trustedFileNameForStorage);
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
            await _stampingRepository.AddPartSketch(part_id, fileToReturn);
            await _stampingRepository.SaveChangesAsync();

            return Ok(fileToReturn);

        }

        [HttpGet("PartSketch/{fileid}")]
        public async Task<IActionResult> DownloadPreviousEvidence(int fileid)
        {
            var FileInfo = await _stampingRepository.FetchFileAsync(fileid);

            if (FileInfo is not null)
            {
                var path = Path.Combine(_env.ContentRootPath, "uploads\\PartsSkecth", FileInfo.StorageFileName);

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
