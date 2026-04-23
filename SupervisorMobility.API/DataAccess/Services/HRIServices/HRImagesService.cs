using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.DataAccess.Services.HRIRepository;
using SupervisorMobility.API.Models.HRIDtos.HRImagesDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

namespace SupervisorMobility.API.DataAccess.Services.HRIServices
{
    public class HRImagesService : IHRImagesService
    {
        private readonly IHRImagesRepository _hrImagesRepository;
        private readonly IWebHostEnvironment _env;
        private readonly string _uploadsRoot;
        private readonly FileExtensionContentTypeProvider _contentTypeProvider = new();

        public HRImagesService(IHRImagesRepository hrImagesRepository, IWebHostEnvironment env)
        {
            _hrImagesRepository = hrImagesRepository;
            _env = env;
            _uploadsRoot = Path.GetFullPath(Path.Combine(env.ContentRootPath, "uploads"));
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

                // Move from temp folder to definiv folder
                images.ForEach(img =>
                {
                    var tempFilePath = Path.Combine(_env.ContentRootPath, img.ImageUrl);
                    if (File.Exists(tempFilePath))
                    {
                        var fileName = Path.GetFileName(tempFilePath);
                        var destinationDirectory = Path.Combine(_env.ContentRootPath, "uploads", "HRIImages");
                        if (!Directory.Exists(destinationDirectory))
                        {
                            Directory.CreateDirectory(destinationDirectory);
                        }

                        destinationDirectory = Path.Combine(destinationDirectory, img.HriId.ToString());
                        if(!Directory.Exists(destinationDirectory))
                        {
                            Directory.CreateDirectory(destinationDirectory);
                        }

                        var destinationFilePath = Path.Combine(destinationDirectory, fileName);
                        File.Move(tempFilePath, destinationFilePath);

                        // Update the ImageUrl to the new location
                        img.ImageUrl = Path.Combine("uploads", "HRIImages", img.HriId.ToString(), fileName).Replace("\\", "/");
                    }
                    else
                    {
                        throw new Exception($"The temporary file {tempFilePath} does not exist.");
                    }
                });


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

                var tempDirectory = Path.Combine(_env.ContentRootPath, "uploads\\temp", "HRIImages");
                if (!Directory.Exists(tempDirectory))
                {
                    Directory.CreateDirectory(tempDirectory);
                }

                var fileName = $"{Guid.NewGuid()}{extension}";
                var tempFilePath = Path.Combine(tempDirectory, fileName);

                await using var stream = new FileStream(tempFilePath, FileMode.Create);
                await image.CopyToAsync(stream);

                var relativePath = Path.Combine("uploads", "temp", "HRIImages", fileName).Replace("\\", "/");
                return new ServiceResponse<string>
                {
                    Data = relativePath,
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

        public ServiceResponse<HRImageContentDto> GetImageContent(string path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    return new ServiceResponse<HRImageContentDto>
                    {
                        Success = false,
                        Message = "Image path is required."
                    };
                }

                var filePath = ResolvePathWithinUploads(path);
                if (filePath is null)
                {
                    return new ServiceResponse<HRImageContentDto>
                    {
                        Success = false,
                        Message = "Image path is invalid."
                    };
                }

                if (!File.Exists(filePath))
                {
                    return new ServiceResponse<HRImageContentDto>
                    {
                        Success = false,
                        Message = "Image file does not exist."
                    };
                }

                if (!IsSupportedImage(filePath))
                {
                    return new ServiceResponse<HRImageContentDto>
                    {
                        Success = false,
                        Message = "Only image files are allowed."
                    };
                }

                if (!_contentTypeProvider.TryGetContentType(filePath, out var contentType))
                {
                    contentType = "application/octet-stream";
                }

                return new ServiceResponse<HRImageContentDto>
                {
                    Data = new HRImageContentDto
                    {
                        FilePath = filePath,
                        ContentType = contentType
                    }
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<HRImageContentDto>
                {
                    Success = false,
                    Message = $"Error resolving image content: {ex.Message}"
                };
            }
        }

        private string? ResolvePathWithinUploads(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return null;
            }

            var normalized = relativePath
                .Replace('/', Path.DirectorySeparatorChar)
                .TrimStart(Path.DirectorySeparatorChar);

            var combinedPath = Path.GetFullPath(Path.Combine(_uploadsRoot, normalized));
            if (!combinedPath.StartsWith(_uploadsRoot, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return combinedPath;
        }

        private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".png",
            ".jpg",
            ".jpeg",
            ".webp",
            ".gif",
            ".bmp",
            ".svg"
        };

        private static bool IsSupportedImage(string path)
        {
            var extension = Path.GetExtension(path);
            return AllowedImageExtensions.Contains(extension);
        }

    }
}
