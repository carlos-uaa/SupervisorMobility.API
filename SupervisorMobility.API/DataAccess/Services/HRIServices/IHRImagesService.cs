using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.Models.HRIDtos.HRImagesDto;
using Microsoft.AspNetCore.Http;

namespace SupervisorMobility.API.DataAccess.Services.HRIServices
{
    public interface IHRImagesService
    {
        Task<ServiceResponse<List<HRImages>>> GetImagesByHRIIdAsync(int hriId);
        Task<ServiceResponse<List<HRImages>>> CreateHRImagesAsync(List<CreateHRImageDto> images);
        Task<ServiceResponse<List<HRImages>>> UpdateHRImageAsync(List<UpdateHRImageDto> image);
        Task<ServiceResponse<bool>> DeleteHRImageAsync(int imageId);
        Task<ServiceResponse<string>> SaveImageInTempFolderAsync(IFormFile image);
    }
}
