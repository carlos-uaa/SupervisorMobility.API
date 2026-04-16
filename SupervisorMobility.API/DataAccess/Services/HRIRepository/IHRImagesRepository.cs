using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.Models.HRIDtos.HRImagesDto;

namespace SupervisorMobility.API.DataAccess.Services.HRIRepository
{
    public interface IHRImagesRepository
    {
        Task<ServiceResponse<List<HRImages>>> GetImagesByHRIIdAsync(int hriId);

        Task<ServiceResponse<HRImages>> GetHRImageByImageIdAsync(int imageId);
        Task<ServiceResponse<List<HRImages>>> CreateHRImagesAsync(List<CreateHRImageDto> images);
        Task<ServiceResponse<List<HRImages>>> UpdateHRImageAsync(List<UpdateHRImageDto> images);
        Task<ServiceResponse<bool>> DeleteHRImageAsync(int imageId);
    }
}
