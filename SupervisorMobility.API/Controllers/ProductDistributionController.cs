﻿using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Migrations;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Models.ProductDistributionsDtos;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/product/{productId}/distributions")]
    [ApiController]
    public class ProductDistributionsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;

        public ProductDistributionsController(ISupervisorMobilityRepository supervisorMobilityRepository,
            IMapper mapper)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDistributionWithoutNavigationPropertiesDto>>> GetProductDistributions(int productId)
        {
            if (!await _supervisorMobilityRepository.ProductExistAsync(productId))
            {
                return NotFound();
            }


            var distributionsForProduct = await _supervisorMobilityRepository.GetDistributionsForProductAsync(productId);

            return Ok(_mapper.Map<IEnumerable<ProductDistributionWithoutNavigationPropertiesDto>>(distributionsForProduct));
        }

        [HttpPost]
        public async Task<ActionResult<ProductDistributionWithoutNavigationPropertiesDto>> CreateProductDistribution(int productId,
            ProductDistributionForCreationDto distribution)
        {

            if (!await _supervisorMobilityRepository.ProductExistAsync(productId))
            {
                return NotFound();
            }

            var finalProductDistribution = _mapper.Map<ProductDistribution>(distribution);

            await _supervisorMobilityRepository.AddDistributionForProductAsync(productId, finalProductDistribution);

            await _supervisorMobilityRepository.SaveChangesAsync();

            var createdDistributionToReturn =
                _mapper.Map<ProductDistributionWithoutNavigationPropertiesDto>(finalProductDistribution);


            //return CreatedAtRoute("GetProductDistribution",
            //    new
            //    {
            //        productId,
            //        productDistributionId= createdDistributionToReturn.ProductDistributionId
            //    },
            //    createdDistributionToReturn);

            return Ok();

        }


        [HttpPut("{productDistributionId}")]
        public async Task<ActionResult> UpdateProductDistribution(int productId, int productDistributionId, ProductDistributionForUpdateDto productDistribution)
        {

            if (!await _supervisorMobilityRepository.ProductExistAsync(productId))
            {
                return NotFound();
            }

            var distributionEntity = await _supervisorMobilityRepository
                .GetDistributionForProductAsync(productId, productDistributionId);
            if (distributionEntity == null)
            {
                return NotFound();
            }
            _mapper.Map(productDistribution, distributionEntity);

            await _supervisorMobilityRepository.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("{productDistributionId}")]
        public async Task<ActionResult> DeleteProductDistribution(int productId, int productDistributionId)
        {
            if (!await _supervisorMobilityRepository.ProductExistAsync(productId))
            {
                return NotFound();
            }

            var distributionEntity = await _supervisorMobilityRepository
                .GetDistributionForProductAsync(productId, productDistributionId);
            if (distributionEntity == null)
            {
                return NotFound();
            }

            _supervisorMobilityRepository.DeleteProductDistribution(distributionEntity);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return NoContent();
        }


    }

}