using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Services;
using System.Xml.Linq;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/plants/{plantId}/areas/{areaId}/distributions/{distributionId}/operations")]
    [ApiController]
    public class OperationsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAssyChartService _assyChartService;

        public OperationsController(IAssyChartService assyChartService,
            IMapper mapper)
        {
            _assyChartService = assyChartService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OperationWithoutNavigationPropertiesDto>>> GetOperations(
                    int plantId, int areaId, int distributionId)
        {
            if (!await _assyChartService.CheckPlantExistance(plantId))
            {
                return NotFound();
            }

            if (!await _assyChartService.CheckAreaExistance(areaId))
            {
                return NotFound();
            }

            if (!await _assyChartService.CheckDistributionExistance(distributionId))
            {
                return NotFound();
            }

            var operationsForDistribution = await _assyChartService
                .FetchOperationsAsync(distributionId);

            return Ok(_mapper.Map<IEnumerable<OperationWithoutNavigationPropertiesDto>>(operationsForDistribution));
        }

        [HttpGet("{operationId}", Name = "GetOperation")]
        public async Task<ActionResult<OperationWithoutNavigationPropertiesDto>> GetOperation(
           int plantId, int areaId, int distributionId, int operationId)
        {
            if (!await _assyChartService.CheckPlantExistance(plantId))
            {
                return NotFound();
            }

            if (!await _assyChartService.CheckAreaExistance(areaId))
            {
                return NotFound();
            }

            if (!await _assyChartService.CheckDistributionExistance(distributionId))
            {
                return NotFound();
            }

            var operation = await _assyChartService
                .FetchOperationAsync(distributionId, operationId);

            if (operation == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<OperationWithoutNavigationPropertiesDto>(operation));
        }

        [HttpPost]
        public async Task<ActionResult<OperationWithoutNavigationPropertiesDto>> CreateOperation(
            int plantId,
            int areaId,
            int distributionId,
            OperationForCreationDto operation)
        {
            if (!await _assyChartService.CheckPlantExistance(plantId))
            {
                return NotFound();
            }

            if (!await _assyChartService.CheckAreaExistance(areaId))
            {
                return NotFound();
            }

            if (!await _assyChartService.CheckDistributionExistance(distributionId))
            {
                return NotFound();
            }

            var finalOperation = _mapper.Map<Operation>(operation);

            await _assyChartService.CreateOperationAsync(areaId, distributionId, finalOperation);

            var createdOperationToReturn =
                _mapper.Map<OperationWithoutNavigationPropertiesDto>(finalOperation);

            return CreatedAtRoute("GetOperation",
                new
                {
                    plantId,
                    areaId,
                    distributionId,
                    operationId = createdOperationToReturn.OperationId
                },
                createdOperationToReturn);
        }

        [HttpPut("{operationid}")]
        public async Task<ActionResult> UpdateOperation(int plantId, int areaId, int distributionId,
            int operationId,
            OperationForUpdateDto operation)
        {
            if (!await _assyChartService.CheckPlantExistance(plantId))
            {
                return NotFound();
            }

            if (!await _assyChartService.CheckAreaExistance(areaId))
            {
                return NotFound();
            }

            if (!await _assyChartService.CheckDistributionExistance(distributionId))
            {
                return NotFound();
            }

            var operationEntity = await _assyChartService
                .FetchOperationAsync(distributionId, operationId);
            if (operationEntity == null)
            {
                return NotFound();
            }

            await _assyChartService.UpdateOperationAsync(operation, operationEntity);

            return NoContent();
        }

        [HttpPatch("{operationid}")]
        public async Task<ActionResult> PartiallyUpdateOperation(
            int plantId, int areaId, int distributionId,int operationId,
            JsonPatchDocument<OperationForUpdateDto> patchDocumentOperation)
        {
            if (!await _assyChartService.CheckPlantExistance(plantId))
            {
                return NotFound();
            }

            if (!await _assyChartService.CheckAreaExistance(areaId))
            {
                return NotFound();
            }

            if (!await _assyChartService.CheckDistributionExistance(distributionId))
            {
                return NotFound();
            }

            var operationEntity = await _assyChartService
                .FetchOperationAsync(distributionId, operationId);
            if (operationEntity == null)
            {
                return NotFound();
            }

            var operationToPatch = _mapper.Map<OperationForUpdateDto>(operationEntity);

            patchDocumentOperation.ApplyTo(operationToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(operationToPatch))
            {
                return BadRequest(ModelState);
            }

            await _assyChartService.UpdateOperationAsync(operationToPatch, operationEntity);

            return NoContent();
        }

        [HttpDelete("{operationId}")]
        public async Task<ActionResult> DeleteOperation(int plantId, int areaId, int distributionId, int operationId)
        {
            if (!await _assyChartService.CheckPlantExistance(plantId))
            {
                return NotFound();
            }

            if (!await _assyChartService.CheckAreaExistance(areaId))
            {
                return NotFound();
            }

            if (!await _assyChartService.CheckDistributionExistance(distributionId))
            {
                return NotFound();
            }

            var operationEntity = await _assyChartService
                .FetchOperationAsync(distributionId, operationId);
            if (operationEntity == null)
            {
                return NotFound();
            }

            await _assyChartService.RemoveOperationAsync(operationEntity);

            return NoContent();
        }
    }
}
