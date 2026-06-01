using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.DataAccess.Services.HRIServices;
using SupervisorMobility.API.Models.HRIDtos.HRImagesDto;

namespace SupervisorMobility.API.Controllers.HRIControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HRImagesController : ControllerBase
    {
        private readonly IHRImagesService _hrImagesService;

        public HRImagesController(IHRImagesService hrImagesService)
        {
            _hrImagesService = hrImagesService;
        }

        [HttpPost("CreateHRImagesAsync")]
        public async Task<ServiceResponse<List<HRImages>>> CreateHRImagesAsync(List<CreateHRImageDto> images)
        {
            return await _hrImagesService.CreateHRImagesAsync(images);
        }

        
        [HttpGet("GetImagesByHRIIdAsync/{hriId}")]
        public async Task<ServiceResponse<List<HRImages>>> GetImagesByHRIIdAsync(int hriId)
        {
            return await _hrImagesService.GetImagesByHRIIdAsync(hriId);
        }
        [HttpPut("UpdateHRImageAsync")]
        public async Task<ServiceResponse<List<HRImages>>> UpdateHRImageAsync(List<UpdateHRImageDto> images)
        {
            return await _hrImagesService.UpdateHRImageAsync(images);
        }

        [HttpDelete("DeleteHRImageAsync/{imageId}")]
        public async Task<ServiceResponse<bool>> DeleteHRImageAsync(int imageId)
        {
            return await _hrImagesService.DeleteHRImageAsync(imageId);
        }

        [HttpPost("SaveImageInTempFolderAsync")]
        public async Task<ServiceResponse<string>> SaveImageInTempFolderAsync(IFormFile image)
        {
            return await _hrImagesService.SaveImageInTempFolderAsync(image);
        }
        
        [HttpGet("content")]
        public IActionResult GetImage([FromQuery] string path)
        {
            var imageResponse = _hrImagesService.GetImageContent(path);
            if (!imageResponse.Success || imageResponse.Data is null)
            {
                if (string.Equals(imageResponse.Message, "Image file does not exist.", StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound();
                }

                return BadRequest(imageResponse.Message);
            }

            return PhysicalFile(imageResponse.Data.FilePath, imageResponse.Data.ContentType, enableRangeProcessing: true);
        }
    }
}
