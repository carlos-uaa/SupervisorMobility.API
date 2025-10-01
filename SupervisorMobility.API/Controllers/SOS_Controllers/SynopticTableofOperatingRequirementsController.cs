// - Core .NET imports
using System.Diagnostics;

// - External imports
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

// - Context imports
using SupervisorMobility.API.DataAccess.Services;

// - Entity's imports
using SupervisorMobility.API.DataAccess.Entities.SOS.STRO.Dtos;
using SupervisorMobility.API.DataAccess.Entities.SOS;

// - Interface's imports
using SupervisorMobility.API.Interfaces.SOS;

// - Model's imports
using SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsOperationSequenceDtos;
using SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos.SectionDtos;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.Models.FileUploadDto;



namespace SupervisorMobility.API.Controllers.SOS_Controllers
{
    [Route("api/SOS/SynopticTableofOperatingRequirements")]
    [ApiController]
    public class SynopticTableofOperatingRequirementsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly ISOS_ProcessRepository _ProcessRepository;
        private readonly ISTOperatingRequirementsService _STOperatingRequirementsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SynopticTableofOperatingRequirementsController"/> class.
        /// </summary>
        /// <param name="env">Provides information about the web hosting environment.</param>
        /// <param name="mapper">Automapper instance for mapping entities to DTOs and vice versa.</param>
        /// <param name="repository">Repository used to access SOS process data.</param>
        /// <param name="STOperatingRequirementsService">Service used to manage Synoptic Table of Operating Requirements (STRO) operations.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="mapper"/> or <paramref name="env"/> is null.</exception>
        public SynopticTableofOperatingRequirementsController(IWebHostEnvironment env, IMapper mapper, ISOS_ProcessRepository repository, ISTOperatingRequirementsService STOperatingRequirementsService)
        {
            _ProcessRepository = repository;
            _STOperatingRequirementsService = STOperatingRequirementsService;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        /// <summary>
        /// Creates a new Synoptic Table of Operating Requirements or adds a revision to an existing one.
        /// </summary>
        /// <param name="sOSSynopticTableofOperatingRequirementsToCreate">
        /// DTO containing data for creating or updating the Synoptic Table of Operating Requirements.
        /// </param>
        /// <param name="SOSHubCollection_Id">Identifier of the SOS Hub Collection.</param>
        /// <returns> Returns the created Synoptic Table or a revision confirmation message.</returns>
        /// <response code="200">Creation or revision was successful.</response>
        /// <response code="400">Invalid request or failed creation/revision.</response>
        [HttpPost]
        public async Task<ActionResult<SOSSynopticRequirementsDto>> GenerateSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirementsForCreateDto sOSSynopticTableofOperatingRequirementsToCreate, int SOSHubCollection_Id)
        {
             // ============ CREATE NEW SYNOPTIC TABLE ============= \\
            if (sOSSynopticTableofOperatingRequirementsToCreate.SOSSynopticTableofOperatingRequirementsId == 0)
            {
                // NOTE: Set creation metadata
                sOSSynopticTableofOperatingRequirementsToCreate.CreatedAt = DateTime.Now;
                sOSSynopticTableofOperatingRequirementsToCreate.IsActive = true;
                sOSSynopticTableofOperatingRequirementsToCreate.SOSHubId = SOSHubCollection_Id;

                // NOTE: Map DTO to entity
                SOSSynopticTableofOperatingRequirements SynopticTableofOperatingRequirementsToCreate = _mapper.Map<SOSSynopticTableofOperatingRequirements>(sOSSynopticTableofOperatingRequirementsToCreate);

                SynopticTableofOperatingRequirementsToCreate.SOSHubs = new List<SOSHub>();
                SynopticTableofOperatingRequirementsToCreate.SOSSynopticRequirementsOperationSequence = new List<SOSSynopticRequirementsOperationSequence>();

                // NOTE: Populate SOSHubs and operation sequences
                foreach (var SOSHub in sOSSynopticTableofOperatingRequirementsToCreate.SOSHubs!)
                {
                    SOSHub SOSHubDb = await _ProcessRepository.GetSOSHub(SOSHub.SOSHubId);
                    SynopticTableofOperatingRequirementsToCreate.SOSHubs.Add(SOSHubDb);

                    SOSDistribution distribution = SOSHub?.SOSDistribution?.FirstOrDefault()!;
                    SOSDistribution SOSdistributionComplete = await _ProcessRepository.GetSOSDistribution(distribution.SOSDistributionId);


                    foreach (var sequenceDistribution in SOSdistributionComplete.SOSDistributionOperationSequence!)
                    {
                        SynopticTableofOperatingRequirementsToCreate.SOSSynopticRequirementsOperationSequence.Add(
                                new SOSSynopticRequirementsOperationSequence
                                {
                                    Sequence = sequenceDistribution.SequenceId,
                                    SectionId = sequenceDistribution.SectionId,
                                    SosHubId = SOSHub?.SOSHubId,
                                    OperationPersonText = sequenceDistribution?.Section?.Step,
                                    OperationMachineText = sequenceDistribution?.Section?.Step,
                                    IsOperationPersonRequired = true,
                                    IsOperationMachineRequired = false,
                                }
                            );
                    }
                }

                // NOTE: Create synoptic table in repository
                var createdResult = await _ProcessRepository.CreateSOSSynopticTableofOperatingRequirements(SynopticTableofOperatingRequirementsToCreate);
                if (createdResult != null)
                    return Ok(SynopticTableofOperatingRequirementsToCreate);
                else
                    return BadRequest();
            }
            else
            {
                // NOTE: Fetch existing synoptic table with all related data
                SOSSynopticTableofOperatingRequirements _sosSynopticTableofOperatingRequirements = await _ProcessRepository.GetSOSSynopticTableofOperatingRequirements(sOSSynopticTableofOperatingRequirementsToCreate.SOSSynopticTableofOperatingRequirementsId, true, true, true);

                // NOTE: Map the last logbook entry from DTO
                SOSSynopticRequirementsLogbook _logbookToCreate = _mapper.Map<SOSSynopticRequirementsLogbook>(sOSSynopticTableofOperatingRequirementsToCreate.SynopticRequirementsLogbooks?.Last());
                _logbookToCreate.SOSSynopticRequirementsLogbookId = _sosSynopticTableofOperatingRequirements.SOSSynopticTableofOperatingRequirementsId;

                // NOTE: Add logbook entry to repository
                var resultAddSections = await _ProcessRepository.CreateSOSSynopticRequirementsLogbook(_logbookToCreate);

                if (resultAddSections > 0)
                {
                    // NOTE: Link logbook entry to synoptic table
                    await _ProcessRepository.AddSOSSynopticRequirementsLogbookToSOSSynopticTableofOperatingRequirements(_sosSynopticTableofOperatingRequirements, _logbookToCreate);
                }
                else
                {
                    return BadRequest();
                }



                return Ok("Revision");
            }

        }

        [HttpGet("{id}", Name = "GetSOSSynopticTableofOperatingRequirements")]
        public async Task<ActionResult<SOSSynopticRequirementsDto>> GetSOSSynopticTableofOperatingRequirements(int id, bool includeLogbooks = false, bool includeSOS = false, bool includeCollections = false)
        {

            var SOSSynopticTableofOperatingRequirements = await _ProcessRepository.GetSOSSynopticTableofOperatingRequirements(id, includeLogbooks, includeSOS, includeCollections);
            if (SOSSynopticTableofOperatingRequirements == null)
            {
                return NotFound("SOSSynopticTableofOperatingRequirements not found!");
            }

            return Ok(_mapper.Map<SOSSynopticRequirementsDto>(SOSSynopticTableofOperatingRequirements));
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<SOSSynopticRequirementsDto>>> GetAllSOSSynopticTableofOperatingRequirements(bool includeLogbooks = false, bool includeSOS = false, bool includeCollections = false)
        {

            var CheckpointEntities = await _ProcessRepository.GetAllSOSSynopticTableofOperatingRequirements(includeLogbooks, includeSOS, includeCollections);
            if (CheckpointEntities == null)
            {
                return NotFound("Get All Sos Analisis not found!");
            }

            return Ok(_mapper.Map<IEnumerable<SOSSynopticRequirementsDto>>(CheckpointEntities));

        }

        /// <summary>
        /// Generates an Excel file for the Synoptic Table of Operating Requirements (STOR)
        /// for the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The STOR record identifier.</param>
        /// <returns>A file result containing the Excel file named "STOR.xlsx".</returns>
        /// <response code="200">The Excel file was generated successfully.</response>
        /// <response code="400">If the provided <paramref name="id"/> is invalid or the generation fails.</response>
        [HttpGet("GenerateExcelSTOperatingRequirements/{id}")]
        public async Task<ActionResult<int>> GenerateExcelSTOperatingRequirements(int id)
        {
            byte[] resGenerate = await _STOperatingRequirementsService.GenerateExcelSTOperatingRequirements(id);

            return File(resGenerate, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "STOR.xlsx");
        }

        //Update

        [HttpPut("{sosSynopticTableofOperatingRequirements_Id}")]
        public async Task<ActionResult> UpdateSOSSynopticTableofOperatingRequirements(int sosSynopticTableofOperatingRequirements_Id, SOSSynopticTableofOperatingRequirementsForUpdateDto sosUpdateEntity)
        {
            try
            {
                var _sosSynopticTableofOperatingRequirements = await _ProcessRepository.UpdateSOSSynopticTableofOperatingRequirements(sosSynopticTableofOperatingRequirements_Id, sosUpdateEntity);
                return Ok(_sosSynopticTableofOperatingRequirements);

            }
            catch (Exception error)
            {
                return BadRequest(error);
            }
        }//end Update 

        //[HttpDelete("{SOSSynopticTableofOperatingRequirementsId}")]
        //public async Task<ActionResult<int>> RemoveSOSHub(int SOSSynopticTableofOperatingRequirementsId)
        //{
        //    var result = await _ProcessRepository.RemoveSOSSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirementsId);

        //    if (result > 0)
        //        return Ok();
        //    else
        //        return BadRequest("something wrong");
        //}


        ////ilustrations

        //[HttpPost("Ilustrations/{SynopticTableofOperatingRequirements_id}")]
        //public async Task<ActionResult<FileUpload>> UploadIlustrations(int SynopticTableofOperatingRequirements_id, IFormFile file)
        //{

        //    var uploadResult = new FileUploadForCreationDto();
        //    string trustedFileNameForStorage = string.Empty;
        //    var unstrustedFileName = file.FileName;

        //    trustedFileNameForStorage = Path.GetRandomFileName();

        //    var path = Path.Combine(_env.ContentRootPath, "uploads\\SOSSynopticTableofOperatingRequirements\\Ilustrations", trustedFileNameForStorage);
        //    // Asegurarse de que el directorio de destino exista
        //    var directory = Path.GetDirectoryName(path);
        //    if (!Directory.Exists(directory))
        //    {
        //        Directory.CreateDirectory(directory);
        //    }


        //    await using FileStream fs = new(path, FileMode.Create);
        //    await file.CopyToAsync(fs);

        //    uploadResult.FileName = unstrustedFileName;
        //    uploadResult.StorageFileName = trustedFileNameForStorage;
        //    uploadResult.ContentType = file.ContentType;
        //    uploadResult.UploadDate = DateTime.Now;
        //    uploadResult.IsActive = true;

        //    var fileToReturn = await _ProcessRepository.CreateFileAsync(uploadResult);

        //    await _ProcessRepository.AddIlustrationToSOSSynopticTableofOperatingRequirements(SynopticTableofOperatingRequirements_id, fileToReturn);
        //    await _ProcessRepository.SaveChangesAsync();

        //    return Ok(fileToReturn);
        //}

        //[HttpGet("Ilustrations/{fileid}")]
        //public async Task<IActionResult> DownloadIlustrations(int fileid)
        //{
        //    var FileInfo = await _ProcessRepository.FetchFileAsync(fileid);

        //    if (FileInfo is not null)
        //    {
        //        var path = Path.Combine(_env.ContentRootPath, "uploads\\SOSSynopticTableofOperatingRequirements\\Ilustrations", FileInfo.StorageFileName);

        //        var memory = new MemoryStream();
        //        using (var stream = new FileStream(path, FileMode.Open))
        //        {
        //            await stream.CopyToAsync(memory);
        //        }
        //        memory.Position = 0;

        //        var result = File(memory, FileInfo.ContentType, Path.GetFileName(path));
        //        result.EnableRangeProcessing = true;

        //        return result;
        //    }
        //    return NotFound("Error File download");
        //}

        //[HttpDelete("Ilustrations/{SOS_SOSSynopticTableofOperatingRequirements_id}/remove/{ImageFile_id}")]
        //public async Task<ActionResult<int>> RemoveImage(int SOS_SOSSynopticTableofOperatingRequirements_id, int ImageFile_id)
        //{
        //    var result = await _ProcessRepository.RemoveIlustrationFromSOSSynopticTableofOperatingRequirements(SOS_SOSSynopticTableofOperatingRequirements_id, ImageFile_id);

        //    if (result > 0)
        //        return Ok();
        //    else
        //        return BadRequest("something went wrong");
        //}

    }
}
