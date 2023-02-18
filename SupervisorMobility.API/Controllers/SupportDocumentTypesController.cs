using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.Models.PlantDtos;
using SupervisorMobility.API.Models.SupportDocumentTypeDtos;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/supportdocumenttypes")]
    [ApiController]
    public class SupportDocumentTypesController : ControllerBase
    {
        readonly IAssyChartService _assyChartService;
        readonly IMapper _mapper;
        public SupportDocumentTypesController(IAssyChartService assyChartService,
            IMapper mapper)
        {
            _assyChartService = assyChartService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupportDocumentTypeDto>>> GetSupportDocumentTypes()
        {
            var supportDocumentTypeEntities = await _assyChartService.FetchSupportDocumentTypesAsync();
            return Ok(_mapper.Map<IEnumerable<SupportDocumentTypeDto>>(supportDocumentTypeEntities));
        }

        [HttpGet("{supportDocumentTypeId}", Name = "GetSupportDocumentType")]
        public async Task<IActionResult> GetSupportDocumentType(int supportDocumentTypeId)
        {
            var supportDocumentType = await _assyChartService
                .FetchSupportDocumentTypeAsync(supportDocumentTypeId);
            if (supportDocumentType == null)
            {
                return NotFound($"No support document was found with id: {supportDocumentTypeId}");
            }

            return Ok(_mapper.Map<SupportDocumentTypeDto>(supportDocumentType));
        }

        [HttpPost]
        public async Task<ActionResult<SupportDocumentTypeDto>> CreateSupportDocumentType(
            SupportDocumentTypeForCreationDto supportDocumentType)
        {
            //Mpa the object
            var finalSupportDocumentType = await _assyChartService
                .CreateSupportDocumentTypeAsync(_mapper.Map<DataAccess.Entities.SupportDocumentType>(supportDocumentType));

            var createSupportDocumentTypeToReturn =
                _mapper.Map<SupportDocumentTypeDto>(finalSupportDocumentType);

            return CreatedAtRoute("GetSupportDocumentType",
                new
                {
                    supportDocumentTypeId = createSupportDocumentTypeToReturn.SupportDocumentTypeId
                },
                createSupportDocumentTypeToReturn);
        }


        [HttpPut("{supportDocumentTypeId}")]
        public async Task<ActionResult> UpdateSupportDocumentType(int supportDocumentTypeId,
            SupportDocumentTypeForUpdateDto supportDocumentType)
        {
            var supportDocumentTypeEntity = await _assyChartService.FetchSupportDocumentTypeAsync(supportDocumentTypeId);
            if (supportDocumentTypeEntity == null)
            {
                return NotFound($"No support document was found with id: {supportDocumentTypeId}");
            }

            await _assyChartService
                .UpdateSupportDocumentTypeAsync(supportDocumentType, supportDocumentTypeEntity);

            return Ok();

        }

        [HttpPatch("{supportDocumentTypeId}")]
        public async Task<ActionResult> PartiallyUpdateSupportDocumentType(
            int supportDocumentTypeId,
            JsonPatchDocument<SupportDocumentTypeForUpdateDto> patchDocumentSupportDocumentType)
        {
            var supportDocumentTypeEntity = await _assyChartService.FetchSupportDocumentTypeAsync(supportDocumentTypeId);
            if (supportDocumentTypeEntity == null)
            {
                return NotFound();
            }

            var supportDocumentTypeToPatch = _mapper.Map<SupportDocumentTypeForUpdateDto>(supportDocumentTypeEntity);

            patchDocumentSupportDocumentType.ApplyTo(supportDocumentTypeToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(patchDocumentSupportDocumentType))
            {
                return BadRequest();
            }

            await _assyChartService
                .UpdateSupportDocumentTypeAsync(supportDocumentTypeToPatch, supportDocumentTypeEntity);

            return Ok();
        }

        [HttpDelete("{supportDocumentTypeId}")]
        public async Task<ActionResult> DeleteSupportDocumentType(int supportDocumentTypeId)
        {
            var supportDocumentTypeEntity = await _assyChartService.FetchSupportDocumentTypeAsync(supportDocumentTypeId);
            if (supportDocumentTypeEntity == null)
            {
                return NotFound();
            }

            await _assyChartService.RemoveSupportDocumentTypeAsync(supportDocumentTypeEntity);

            return Ok();
        }
    }
}
