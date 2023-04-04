using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.DistributionDtos;
using SupervisorMobility.API.Models.PlantDtos;
using SupervisorMobility.API.Models.ProductDtos;
using SupervisorMobility.API.Services;
using System.Diagnostics;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;
        readonly IAssyChartService _assyChartService;
        readonly IMapper _mapper;
        public ProductsController(ISupervisorMobilityRepository supervisorMobilityRepository, IAssyChartService assyChartService,
            IMapper mapper)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
               throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _assyChartService = assyChartService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var productEntities = await _assyChartService.FetchProductsAsync();
            return Ok(_mapper.Map<IEnumerable<ProductDto>>(productEntities));
        }

        [HttpGet("{productId}", Name = "GetProduct")]
        public async Task<ActionResult<ProductWhitNavigationPropietiesDto>> GetProduct(int productId, bool collections = false)
        {
            //Find Job Observation type
            

            if (collections)
            {
                var product = await _assyChartService
                .FetchProductAsync(productId, collections);
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<ProductWhitNavigationPropietiesDto>(product));

            }
            else
            {
                var product = await _assyChartService
                .FetchProductAsync(productId);
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<ProductDto>(product));

            }

        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct(
            ProductForCreationDto product)
        {
            //Mpa the pbject
            var finalProduct = await _assyChartService.CreateProductAsync(product);


            var createProductToReturn =
                _mapper.Map<ProductDto>(finalProduct);

            return CreatedAtRoute("GetProduct",
                new
                {
                    productId = createProductToReturn.ProductId
                },
                createProductToReturn);
        }

       

        [HttpGet("{productId}/distributions/{distributionId}")]
        public async Task<ActionResult<DistributionWithNavigationPropertiesDto>> GetOneDistributionOnlyById(
          int distributionId, bool includeCollections = false)
        {

            var distribution = await _supervisorMobilityRepository.GetDistributionOnlyIdAsync(distributionId);
            if (distribution == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<DistributionWithoutNavigationPropertiesDto>(distribution));

        }

        [HttpPost("{productId}/distributions/add")]
        public async Task<ActionResult<DistributionWithoutNavigationPropertiesDto>> AddDistribution(int productId,
            int plantId,
            int areaId,
            DistributionWithoutNavigationPropertiesDto distribution)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound("No Plant Exist");
            }

            if (!await _supervisorMobilityRepository.AreaExistAsync(areaId))
            {
                return NotFound("No Area Exist");
            }
            if (!await _supervisorMobilityRepository.ProductExistAsync(productId))
            {
                return NotFound("No product Exist");
            }

         
            var finalDistribution = _mapper.Map<Distribution>(distribution);
            finalDistribution.AreaId = areaId;
            await _supervisorMobilityRepository.AddDistributionForProductAsync(productId, finalDistribution);

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

        [HttpPost("{productId}/distributions/create")]
        public async Task<ActionResult<DistributionWithoutNavigationPropertiesDto>> CreateDistribution(int productId,
           int plantId,
           int areaId,
           DistributionForCreationDto distribution)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound("No Plant Exist");
            }

            if (!await _supervisorMobilityRepository.AreaExistAsync(areaId))
            {
                return NotFound("No Area Exist");
            }

            var finalDistribution = _mapper.Map<Distribution>(distribution);

            await _supervisorMobilityRepository.AddDistributionForPlantAsync(plantId,
                areaId, finalDistribution);

            //add distribution to product

            await _supervisorMobilityRepository.AddDistributionForProductAsync(productId, finalDistribution);


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

        [HttpPut("{productId}/distributions/{distributionId}/update")]
        public async Task<ActionResult> UpdateDistributionOnlyOne(
           int distributionId,
           DistributionForUpdateDto distribution)
        {


            var distributionEntity = await _supervisorMobilityRepository
                .GetDistributionOnlyIdAsync(distributionId);
            if (distributionEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(distribution, distributionEntity);

            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{productId}/distributions/{distributionId}/remove")]
        public async Task<ActionResult> DeleteDistribution(int productId, int distributionId)
        {
            if (!await _supervisorMobilityRepository.ProductExistAsync(productId))
            {
                return NotFound("No product Exist");
            }


            await _supervisorMobilityRepository.RemoveDistributionForProductAsync(productId, distributionId);

            await _supervisorMobilityRepository.SaveChangesAsync();

            var product = await _assyChartService
                 .FetchProductAsync(productId, true);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ProductWhitNavigationPropietiesDto>(product));
        }


        [HttpPut("{productId}")]
        public async Task<ActionResult> UpdateProduct(int productId,
            ProductForUpdateDto product)
        {
            var productEntity = await _assyChartService.FetchProductAsync(productId);
            if (productEntity == null)
            {
                return NotFound();
            }

            await _assyChartService.UpdateProductAsync(product, productEntity);

            return Ok();

        }

        [HttpPatch("{productId}")]
        public async Task<ActionResult> PartiallyUpdateProduct(
            int productId,
            JsonPatchDocument<ProductForUpdateDto> patchDocumentProduct)
        {
            var productEntity = await _assyChartService.FetchProductAsync(productId);
            if (productEntity == null)
            {
                return NotFound();
            }

            var productToPatch = _mapper.Map<ProductForUpdateDto>(productEntity);

            patchDocumentProduct.ApplyTo(productToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(patchDocumentProduct))
            {
                return BadRequest();
            }

            await _assyChartService.UpdateProductAsync(productToPatch, productEntity);

            return Ok();
        }

        [HttpDelete("{productId}")]
        public async Task<ActionResult> DeleteProduct(int productId)
        {
            var productEntity = await _assyChartService.FetchProductAsync(productId);
            if (productEntity == null)
            {
                return NotFound();
            }

            await _assyChartService.RemoveProductAsync(productEntity);

            return Ok();
        }

       

    }
}
