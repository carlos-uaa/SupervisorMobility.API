using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.Models.HRIDtos.HRImagesDto;

namespace SupervisorMobility.API.DataAccess.Services.HRIRepository
{
    public class HRImagesRepository : IHRImagesRepository
    {
        private readonly SupervisorMobilityContext _context;

        public HRImagesRepository(SupervisorMobilityContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<List<HRImages>>> GetImagesByHRIIdAsync(int hriId)
        {
            try
            {
                var images = await _context.HRImages.Where(i => i.HriId == hriId).ToListAsync();
                var message = images.Count > 0 ? "HRI images retrieved successfully." : "No HRI images found.";

                return new ServiceResponse<List<HRImages>>
                {
                    Data = images,
                    Success = true,
                    Message = message
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<HRImages>>
                {
                    Data = null,
                    Success = false,
                    Message = $"An error occurred while retrieving HRI images in DB: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<HRImages>> GetHRImageByImageIdAsync(int imageId)
        {
            try
            {
                var image = await _context.HRImages.FirstOrDefaultAsync(i => i.ImageId == imageId);
                var message = image != null ? "HRI image retrieved successfully." : "HRI image not found.";

                return new ServiceResponse<HRImages>
                {
                    Data = image,
                    Success = image != null,
                    Message = message
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<HRImages>
                {
                    Data = null,
                    Success = false,
                    Message = $"An error occurred while retrieving the HRI image by image ID: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<List<HRImages>>> CreateHRImagesAsync(List<CreateHRImageDto> images)
        {
            try
            {
                var hrImages = images.Select(i => new HRImages
                {
                    HriId = i.HriId,
                    ImageUrl = i.ImageUrl,
                    ImageType = i.ImageType
                }).ToList();
                await _context.HRImages.AddRangeAsync(hrImages);
                await _context.SaveChangesAsync();

                return new ServiceResponse<List<HRImages>>
                {
                    Data = hrImages,
                    Success = true,
                    Message = "HRI images created successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<HRImages>>
                {
                    Data = null,
                    Success = false,
                    Message = $"Error creating HRI images: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<List<HRImages>>> UpdateHRImageAsync(List<UpdateHRImageDto> image)
        {
            try
            {
                // Delete existing images for the HRI
                var existingImages = await _context.HRImages.Where(i => i.HriId == image.First().HriId).ToListAsync();
                _context.HRImages.RemoveRange(existingImages);

                // Add updated images
                var hrImages = image.Select(i => new HRImages
                {
                    HriId = i.HriId,
                    ImageUrl = i.ImageUrl,
                    ImageType = i.ImageType
                }).ToList();
                await _context.HRImages.AddRangeAsync(hrImages);
                await _context.SaveChangesAsync();
                return new ServiceResponse<List<HRImages>>
                {
                    Data = hrImages,
                    Success = true,
                    Message = "HRI images updated successfully."
                };                
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<HRImages>>
                {
                    Data = null,
                    Success = false,
                    Message = $"Error updating HRI image: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<bool>> DeleteHRImageAsync(int imageId)
        {
            try
            {
                var existingImage = await _context.HRImages.FirstOrDefaultAsync(i => i.ImageId == imageId);
                if (existingImage == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Data = false,
                        Success = false,
                        Message = "HRI image not found."
                    };
                }

                _context.HRImages.Remove(existingImage);
                await _context.SaveChangesAsync();

                return new ServiceResponse<bool>
                {
                    Data = true,
                    Success = true,
                    Message = "HRI image deleted successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Success = false,
                    Message = $"Error deleting HRI image: {ex.Message}"
                };
            }
        }
    }
}
