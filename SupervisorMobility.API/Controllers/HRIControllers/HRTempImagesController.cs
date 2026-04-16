using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Services.HRIServices;

namespace SupervisorMobility.API.Controllers.HRIControllers
{
    [ApiController]
    [Route("api/HRI/TempImages")]
    public class HRTempImagesController : ControllerBase
    {
        private readonly IHRImagesService _hrImagesService;

        public HRTempImagesController(IHRImagesService hrImagesService)
        {
            _hrImagesService = hrImagesService;
        }

        [HttpPost("Upload")]
        [Consumes("multipart/form-data")]
        public async Task<ServiceResponse<string>> Upload([FromForm] IFormFile image)
        {
            return await _hrImagesService.SaveImageInTempFolderAsync(image);
        }
    }
}
