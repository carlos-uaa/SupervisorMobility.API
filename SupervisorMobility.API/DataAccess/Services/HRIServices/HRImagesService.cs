using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.DataAccess.Services.HRIRepository;
using SupervisorMobility.API.Models.HRIDtos.HRImagesDto;
using Microsoft.AspNetCore.Http;

namespace SupervisorMobility.API.DataAccess.Services.HRIServices
{
    public class HRImagesService : IHRImagesService
    {
        private readonly IHRImagesRepository _hrImagesRepository;

        public HRImagesService(IHRImagesRepository hrImagesRepository)
        {
            _hrImagesRepository = hrImagesRepository;
        }

        public async Task<ServiceResponse<List<HRImages>>> GetImagesByHRIIdAsync(int hriId)
        {
            try
            {
                return await _hrImagesRepository.GetImagesByHRIIdAsync(hriId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<HRImages>>
                {
                    Success = false,
                    Message = $"An error occurred while retrieving HRI images in Service: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<List<HRImages>>> CreateHRImagesAsync(List<CreateHRImageDto> images)
        {
            try
            {
                if (images == null || images.Count == 0)
                {
                    return new ServiceResponse<List<HRImages>>
                    {
                        Success = false,
                        Message = "The image list is required."
                    };
                }

                if (images.Any(i => string.IsNullOrWhiteSpace(i.ImageUrl)))
                {
                    return new ServiceResponse<List<HRImages>>
                    {
                        Success = false,
                        Message = "All images must have ImageUrl."
                    };
                }

                if (images.Any(i => string.IsNullOrWhiteSpace(i.ImageType)))
                {
                    return new ServiceResponse<List<HRImages>>
                    {
                        Success = false,
                        Message = "All images must have ImageType."
                    };
                }

                return await _hrImagesRepository.CreateHRImagesAsync(images);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<HRImages>>
                {
                    Success = false,
                    Message = $"Error creating HRI images: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<List<HRImages>>> UpdateHRImageAsync(List<UpdateHRImageDto> images)
        {
            try
            {
                if (images == null || images.Count == 0)
                {
                    return new ServiceResponse<List<HRImages>>
                    {
                        Success = false,
                        Message = "The image list is required."
                    };
                }

                try
                {
                    images.ForEach(image =>
                    {
                        if (string.IsNullOrWhiteSpace(image.ImageUrl))
                        {
                            throw new Exception("The field ImageUrl is required.");
                        }

                        if (string.IsNullOrWhiteSpace(image.ImageType))
                        {
                            throw new Exception("The field ImageType is required.");
                        }
                    });
                }
                catch (Exception ex)
                {
                    return new ServiceResponse<List<HRImages>>
                    {
                        Success = false,
                        Message = $"Validation error: {ex.Message}"
                    };
                }
                

                return await _hrImagesRepository.UpdateHRImageAsync(images);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<HRImages>>
                {
                    Success = false,
                    Message = $"Error updating the HRI image: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<bool>> DeleteHRImageAsync(int imageId)
        {
            try
            {
                var existing = await _hrImagesRepository.GetHRImageByImageIdAsync(imageId);
                if (existing == null || existing.Data == null || !existing.Success)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = $"There is no existing HRI image with ID {imageId}."
                    };
                }

                return await _hrImagesRepository.DeleteHRImageAsync(imageId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting the HRI image: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<string>> SaveImageInTempFolderAsync(IFormFile image)
        {
            try
            {
                if (image == null || image.Length == 0)
                {
                    return new ServiceResponse<string>
                    {
                        Success = false,
                        Message = "An image file is required."
                    };
                }

                if (string.IsNullOrWhiteSpace(image.ContentType) || !image.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                {
                    return new ServiceResponse<string>
                    {
                        Success = false,
                        Message = "Only image files are allowed."
                    };
                }

                var extension = Path.GetExtension(image.FileName);
                if (string.IsNullOrWhiteSpace(extension))
                {
                    extension = ".tmp";
                }

                var tempDirectory = Path.Combine(Path.GetTempPath(), "SupervisorMobility", "HRIImages");
                if (!Directory.Exists(tempDirectory))
                {
                    Directory.CreateDirectory(tempDirectory);
                }

                var fileName = $"{Guid.NewGuid()}{extension}";
                var tempFilePath = Path.Combine(tempDirectory, fileName);

                await using var stream = new FileStream(tempFilePath, FileMode.Create);
                await image.CopyToAsync(stream);

                return new ServiceResponse<string>
                {
                    Data = tempFilePath,
                    Message = "Image saved successfully in temp folder."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Message = $"Error saving image in temp folder: {ex.Message}"
                };
            }
        }
    }
}
