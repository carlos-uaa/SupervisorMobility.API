using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.PlantDtos;
using SupervisorMobility.API.Models.ProductDtos;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        readonly IAssyChartService _assyChartService;
        readonly IMapper _mapper;
        public ProductsController(IAssyChartService assyChartService,
            IMapper mapper)
        {
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
        public async Task<ActionResult> GetProduct(int productId, bool collections = false)
        {
            //Find Job Observation type
            var product = await _assyChartService
                .FetchProductAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            if (collections)
            {
                return Ok(_mapper.Map<ProductDto>(product));

            }

            return Ok(_mapper.Map<ProductDto>(product));
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

        [HttpDelete("{productId}/distributions/{distributionId}")]
        public async Task<ActionResult> DeleteDistribution(int productId, int distributionId)
        {
            var productEntity = await _assyChartService.FetchProductAsync(productId);
            if (productEntity == null)
            {
                return NotFound();
            }



            //remove distribution from collection

            //await _assyChartService.RemoveProductAsync(productEntity);

            return Ok();
        }


    }
}
