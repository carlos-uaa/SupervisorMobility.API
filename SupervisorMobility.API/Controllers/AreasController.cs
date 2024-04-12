
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Services.TreeServices;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Services;
using System.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using SupervisorMobility.API.DataAccess.Entities.Paths;
using FuzzyString;
using SupervisorMobility.API.Models.AssyChart;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using SupervisorMobility.API.Models.OperationDtos;
using Slugify;
using SupervisorMobility.API.Models.DistributionDtos;
using SupervisorMobility.API.DataAccess.Entities.TreeStruct;
using SupervisorMobility.API.Entities.CDMS;
using SupervisorMobility.API.Context;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.EMMA;
using Irony.Parsing;
using DuoVia.FuzzyStrings;
using SupervisorMobility.API.Models.HCIDtos;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/plants/{plantId}/areas")]
    [ApiController]
    public class AreasController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;
        private readonly IAssyChartService _assyChartService;
        private readonly IEmailService _email;
        private readonly ITreeService _treeService;
        private readonly CustomHttpClientService customHttp;
        private readonly SupervisorMobilityContext _context;
        private readonly IWebHostEnvironment _env;
        public AreasController(ISupervisorMobilityRepository supervisorMobilityRepository, SupervisorMobilityContext context, IWebHostEnvironment env,
            IMapper mapper, IAssyChartService assyChartService, IEmailService email, ITreeService treeService, CustomHttpClientService customHttpClientService)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _assyChartService = assyChartService ??
                throw new ArgumentNullException(nameof(assyChartService));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _email = email ?? throw new ArgumentNullException(nameof(email));
            _treeService = treeService;
            customHttp = customHttpClientService;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AreaWithJustOperationsDto>>> GetAreas(
                    int plantId, bool includeCollections = false)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound();
            }

            if (includeCollections)
            {
                var areasForPlantWhitDistributions = await _supervisorMobilityRepository.GetAreasForPlantAsync(plantId, includeCollections);
                return Ok(_mapper.Map<IEnumerable<AreaWithJustOperationsDto>>(areasForPlantWhitDistributions));

            }
            else
            {
                var areasForPlant = await _supervisorMobilityRepository
                                .GetAreasForPlantAsync(plantId);
                return Ok(_mapper.Map<IEnumerable<AreaWithoutNavigationPropertiesDto>>(areasForPlant));

            }


        }

        [HttpGet("{areaId}", Name = "GetArea")]
        public async Task<ActionResult<AreaWithoutNavigationPropertiesDto>> GetArea(
           int plantId, int areaId, bool includeOperations = false)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound();
            }

            var area = await _supervisorMobilityRepository
                .GetAreaForPlantAsync(plantId, areaId, includeOperations);

            if (area == null)
            {
                return NotFound();
            }

            if (includeOperations)
            {
                return Ok(_mapper.Map<AreaWithJustOperationsDto>(area));
            }
            return Ok(_mapper.Map<AreaWithoutNavigationPropertiesDto>(area));
        }
        
        [HttpPost]
        public async Task<ActionResult<AreaWithoutNavigationPropertiesDto>> CreateArea(
            int plantId,
            AreaForCreationDto area)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound();
            }

            var finalArea = _mapper.Map<Area>(area);
            finalArea.PlantId = plantId;

            await _supervisorMobilityRepository.AddArea(finalArea);


            await _supervisorMobilityRepository.AddAreaForPlantAsync(
                plantId, finalArea);

            await _supervisorMobilityRepository.SaveChangesAsync();

            var createdAreaToReturn =
                _mapper.Map<AreaWithoutNavigationPropertiesDto>(finalArea);

            return CreatedAtRoute("GetArea",
                new
                {
                    plantId,
                    areaId = createdAreaToReturn.AreaId
                },
                createdAreaToReturn);
        }

        [HttpPut("{areaid}")]
        public async Task<ActionResult> UpdateArea(int plantId, int areaId,
            AreaForUpdateDto area)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound();
            }

            var areaEntity = await _supervisorMobilityRepository
                .GetAreaForPlantAsync(plantId, areaId);
            if (areaEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(area, areaEntity);

            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();
        }

        [HttpPatch("{areaid}")]
        public async Task<ActionResult> PartiallyUpdateArea(
            int plantId, int areaId,
            JsonPatchDocument<AreaForUpdateDto> patchDocumentArea)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound();
            }

            var areaEntity = await _supervisorMobilityRepository
                .GetAreaForPlantAsync(plantId, areaId);
            if (areaEntity == null)
            {
                return NotFound();
            }

            var areaToPatch = _mapper.Map<AreaForUpdateDto>(areaEntity);

            patchDocumentArea.ApplyTo(areaToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(areaToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(areaToPatch, areaEntity);

            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{areaId}")]
        public async Task<ActionResult> DeleteArea(int plantId, int areaId)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound();
            }

            var areaEntity = await _supervisorMobilityRepository
                .GetAreaForPlantAsync(plantId, areaId);
            if (areaEntity == null)
            {
                return NotFound();
            }

            _supervisorMobilityRepository.DeleteArea(areaEntity);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();
        }
    }
}
