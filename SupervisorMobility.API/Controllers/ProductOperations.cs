using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Models.ProductOperationDtos;
using SupervisorMobility.API.Services;
using System.Xml.Linq;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/products/{productId}/distributions/{productDistributionId}/operations")]
    [ApiController]
    public class ProductOperationsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAssyChartService _assyChartService;

        public ProductOperationsController(IAssyChartService assyChartService,
            IMapper mapper)
        {
            _assyChartService = assyChartService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductOperationWithoutNavigationPropertiesDto>>> GetProductOperations(
                     int productId, int productDistributionId)
        {

            if (!await _assyChartService.CheckProductExistance(productId))
            {
                return NotFound();
            }

            if (!await _assyChartService.CheckProductDistributionExistance(productDistributionId))
            {
                return NotFound();
            }

            var operationsForDistribution = await _assyChartService
                .FetchProductOperationsAsync(productDistributionId);

            return Ok(_mapper.Map<IEnumerable<ProductOperationWithoutNavigationPropertiesDto>>(operationsForDistribution));
        }

        [HttpGet("{productOperationId}", Name = "GetProductOperation")]
        public async Task<ActionResult<ProductOperationWithoutNavigationPropertiesDto>> GetProductOperation(
         int productId, int productDistributionId, int productOperationId)
        {
            if (!await _assyChartService.CheckProductExistance(productId))
            {
                return NotFound();
            }

            if (!await _assyChartService.CheckProductDistributionExistance(productDistributionId))
            {
                return NotFound();
            }

            var productOperation = await _assyChartService
                .FetchProductOperationAsync(productDistributionId, productOperationId);

            if (productOperation == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ProductOperationWithoutNavigationPropertiesDto>(productOperation));
        }

        [HttpPost]
        public async Task<ActionResult<ProductOperationWithoutNavigationPropertiesDto>> CreateOperation(
            int productId,
            int productDistributionId,
            ProductOperationForCreationDto productOperation)
        {

            if (!await _assyChartService.CheckProductExistance(productId))
            {
                return NotFound();
            }

            if (!await _assyChartService.CheckProductDistributionExistance(productDistributionId))
            {
                return NotFound();
            }

            var finalOperation = _mapper.Map<ProductOperation>(productOperation);

            await _assyChartService.CreateProductOperationAsync(productId, productDistributionId, finalOperation);

            var createdProductOperationToReturn =
                _mapper.Map<ProductOperationWithoutNavigationPropertiesDto>(finalOperation);

            return CreatedAtRoute("GetProductOperation",
                new
                {
                    productId,
                    productDistributionId,
                    productOperationId = createdProductOperationToReturn.ProductOperationId
                },
                createdProductOperationToReturn);
        }

        [HttpPut("{productOperationId}")]
         public async Task<ActionResult> UpdateProductOperation(int productId, int productDistributionId,
                int productOperationId,
                ProductOperationForUpdateDto productOperation)
                {
                if (!await _assyChartService.CheckProductExistance(productId))
                {
                    return NotFound();
                }

                if (!await _assyChartService.CheckProductDistributionExistance(productDistributionId))
                {
                    return NotFound();
                }

                var productOperationEntity = await _assyChartService
                    .FetchProductOperationAsync(productDistributionId, productOperationId);

                if (productOperationEntity == null)
                {
                    return NotFound();
                }

                await _assyChartService.UpdateProductOperationAsync(productOperation, productOperationEntity);

                return NoContent();
         }

        [HttpPatch("{productOperationid}")]
        public async Task<ActionResult> PartiallyUpdateProductOperation(
         int productId, int productDistributionId, int productOperationId,
         JsonPatchDocument<ProductOperationForUpdateDto> patchDocumentProductOperation)
        {
            if (!await _assyChartService.CheckProductExistance(productId))
            {
                return NotFound();
            }

            if (!await _assyChartService.CheckProductDistributionExistance(productDistributionId))
            {
                return NotFound();
            }

            var productOperationEntity = await _assyChartService
                .FetchProductOperationAsync(productDistributionId, productOperationId);

            if (productOperationEntity == null)
            {
                return NotFound();
            }

            var productOperationToPatch = _mapper.Map<ProductOperationForUpdateDto>(productOperationEntity);

            patchDocumentProductOperation.ApplyTo(productOperationToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(productOperationToPatch))
            {
                return BadRequest(ModelState);
            }

            await _assyChartService.UpdateProductOperationAsync(productOperationToPatch, productOperationEntity);

            return NoContent();
        }


        [HttpDelete("{productOperationId}")]
        public async Task<ActionResult> DeleteProductOperation(int productId, int productDistributionId, int productOperationId)
        {
            if (!await _assyChartService.CheckProductExistance(productId))
            {
                return NotFound();
            }

            if (!await _assyChartService.CheckProductDistributionExistance(productDistributionId))
            {
                return NotFound();
            }

            var productOperationEntity = await _assyChartService
                .FetchProductOperationAsync(productDistributionId, productOperationId);

            if (productOperationEntity == null)
            {
                return NotFound();
            }

            await _assyChartService.RemoveProductOperationAsync(productOperationEntity);

            return NoContent();
        }




    }
}
