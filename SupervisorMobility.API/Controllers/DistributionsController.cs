using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.DistributionDtos;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Models.ProductDtos;
using SupervisorMobility.API.Services;
using System.Diagnostics;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/plants/{plantId}/areas/{areaId}/distributions")]
    [ApiController]
    public class DistributionsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;

        public DistributionsController(ISupervisorMobilityRepository supervisorMobilityRepository,
            IMapper mapper)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DistributionWithNavigationPropertiesDto>>> GetDistributions(
                    int plantId, int areaId, bool includecollections = false)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound();
            }

            if (!await _supervisorMobilityRepository.AreaExistAsync(areaId))
            {
                return NotFound();
            }

            if (includecollections)
            {
                var areasForPlant = await _supervisorMobilityRepository
                .GetDistributionsForAreaAsync(areaId, includecollections);
                return Ok(_mapper.Map<IEnumerable<DistributionWithNavigationPropertiesDto>>(areasForPlant));

            }
            else
            {
                var areasForPlant = await _supervisorMobilityRepository
                .GetDistributionsForAreaAsync(areaId);
                return Ok(_mapper.Map<IEnumerable<DistributionWithoutNavigationPropertiesDto>>(areasForPlant));


            }



        }

        [HttpGet("{distributionId}", Name = "GetDistribution")]
        public async Task<ActionResult<DistributionWithNavigationPropertiesDto>> GetDistribution(
           int plantId, int areaId, int distributionId, bool includeCollections = false)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound();
            }

            if (!await _supervisorMobilityRepository.AreaExistAsync(areaId))
            {
                return NotFound();
            }

            if (includeCollections)
            {
                var distribution = await _supervisorMobilityRepository
                        .GetDistributionForAreaAsync(areaId, distributionId, includeCollections);
                if (distribution == null)
                {
                    return NotFound();
                }

                return Ok(_mapper.Map<DistributionWithNavigationPropertiesDto>(distribution));
            }
            else
            {
                var distribution = await _supervisorMobilityRepository
        .GetDistributionForAreaAsync(areaId, distributionId);
                if (distribution == null)
                {
                    return NotFound();
                }

                return Ok(_mapper.Map<DistributionWithoutNavigationPropertiesDto>(distribution));
            }


        }

        [HttpPost]
        public async Task<ActionResult<DistributionWithoutNavigationPropertiesDto>> CreateDistribution(
            int plantId,
            int areaId,
            DistributionForCreationDto distribution)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound();
            }

            if (!await _supervisorMobilityRepository.AreaExistAsync(areaId))
            {
                return NotFound();
            }

            var finalDistribution = _mapper.Map<Distribution>(distribution);

            await _supervisorMobilityRepository.AddDistributionForPlantAsync(plantId,
                areaId, finalDistribution);

            await _supervisorMobilityRepository.SaveChangesAsync();

            var createdDistributionToReturn =
                _mapper.Map<DistributionWithoutNavigationPropertiesDto>(finalDistribution);

            return CreatedAtRoute("GetDistribution",
                new
                {
                    plantId,
                    areaId,
                    distributionId = createdDistributionToReturn.DistributionId
                },
                createdDistributionToReturn);
        }

        [HttpPut("{distributionid}")]
        public async Task<ActionResult> UpdateDistribution(int plantId, int areaId,
            int distributionId,
            DistributionForUpdateDto distribution)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound();
            }

            if (!await _supervisorMobilityRepository.AreaExistAsync(areaId))
            {
                return NotFound();
            }

            var distributionEntity = await _supervisorMobilityRepository
                .GetDistributionForAreaAsync(areaId, distributionId);
            if (distributionEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(distribution, distributionEntity);

            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();
        }

        [HttpPatch("{distributionid}")]
        public async Task<ActionResult> PartiallyUpdateDistribution(
            int plantId, int areaId, int distributionId,
            JsonPatchDocument<DistributionForUpdateDto> patchDocumentDistribution)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound();
            }

            if (!await _supervisorMobilityRepository.AreaExistAsync(areaId))
            {
                return NotFound();
            }

            var distributionEntity = await _supervisorMobilityRepository
                .GetDistributionForAreaAsync(areaId, distributionId);
            if (distributionEntity == null)
            {
                return NotFound();
            }

            var distributionToPatch = _mapper.Map<DistributionForUpdateDto>(distributionEntity);

            patchDocumentDistribution.ApplyTo(distributionToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(distributionToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(distributionToPatch, distributionEntity);

            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{distributionId}")]
        public async Task<ActionResult> DeleteDistribution(int plantId, int areaId, int distributionId)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound();
            }

            if (!await _supervisorMobilityRepository.AreaExistAsync(areaId))
            {
                return NotFound();
            }

            var distributionEntity = await _supervisorMobilityRepository
                .GetDistributionForAreaAsync(areaId, distributionId);
            if (distributionEntity == null)
            {
                return NotFound();
            }

            _supervisorMobilityRepository.DeleteDistribution(distributionEntity);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();
        }


        [HttpPost("{distributionId}/products")]
        public async Task<ActionResult<DistributionWithoutNavigationPropertiesDto>> CreateProduct(int plantId, int areaId, int distributionId, 
      ProductForCreationDto product)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound("No Plant Exist");
            }

            if (!await _supervisorMobilityRepository.AreaExistAsync(areaId))
            {
                return NotFound("No Area Exist");
            }  
            
            if (!await _supervisorMobilityRepository.DistributionExistsAsync(distributionId))
            {
                return NotFound("No Distribution Exist");
            }

            //var finalDistribution = _mapper.Map<Distribution>(distribution);

            //await _supervisorMobilityRepository.AddDistributionForPlantAsync(plantId,
            //    areaId, finalDistribution);

            ////add distribution to product

            //await _supervisorMobilityRepository.AddDistributionForProductAsync(productId, finalDistribution);


            //await _supervisorMobilityRepository.SaveChangesAsync();

            //var createdDistributionToReturn =
            //    _mapper.Map<DistributionWithoutNavigationPropertiesDto>(finalDistribution);

            //return CreatedAtRoute("GetDistribution",
            //    new
            //    {
            //        plantId,
            //        areaId,
            //        distributionId = createdDistributionToReturn.DistributionId
            //    },
            //    createdDistributionToReturn);

            return Ok();
        }

        [HttpPost("{distributionId}/products/add")]
        public async Task<ActionResult<DistributionWithoutNavigationPropertiesDto>> AddProduct(int plantId, int areaId, int distributionId,
            ProductDto product)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound("No Plant Exist");
            }

            if (!await _supervisorMobilityRepository.AreaExistAsync(areaId))
            {
                return NotFound("No Area Exist");
            }

            if (!await _supervisorMobilityRepository.DistributionExistsAsync(distributionId))
            {
                return NotFound("No Distribution Exist");
            }


            var finalProduct = _mapper.Map<Product>(product);
            await _supervisorMobilityRepository.AddProductForDistributionAsync(areaId, distributionId, finalProduct);

            await _supervisorMobilityRepository.SaveChangesAsync();


            return CreatedAtRoute("GetDistribution",
                new
                {
                    plantId,
                    areaId,
                    distributionId
                }, finalProduct
                );
        }


        [HttpDelete("{distributionId}/products/{productId}")]
        public async Task<ActionResult> DeleteOperation(int plantId, int areaId, int distributionId, int productId)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound();
            }

            if (!await _supervisorMobilityRepository.AreaExistAsync(areaId))
            {
                return NotFound();
            }

            if (!await _supervisorMobilityRepository.DistributionExistsAsync(distributionId))
            {
                return NotFound();
            }

            //Ver como hacer la baja logica para no afectar demas distrubuciones que usen el mismo producto

            //Propuesta: solo sacarlo de la collection 

            //var operationEntity = await _assyChartService
            //    .FetchOperationAsync(distributionId, operationId);
            //if (operationEntity == null)
            //{
            //    return NotFound();
            //}

            //await _assyChartService.RemoveOperationAsync(operationEntity);

            return Ok();
        }


    }



}
