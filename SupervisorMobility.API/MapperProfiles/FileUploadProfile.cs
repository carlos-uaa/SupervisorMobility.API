using AutoMapper;
using SupervisorMobility.API.DataAccess;

namespace SupervisorMobility.API.MapperProfiles
{
    public class FileUploadProfile : Profile
    {
        public FileUploadProfile()
        {
            CreateMap<DataAccess.Entities.FileUpload, Models.FileUploadDto.FileUploadGeneralDto>();
            CreateMap<DataAccess.Entities.FileUpload, Models.FileUploadDto.FileUploadForCreationDto>().ReverseMap();
            //CreateMap<DataAccess.Entities.FileUpload, Models.FileUploadDto.AreaForCreationDto>().ReverseMap();
            //CreateMap<DataAccess.Entities.FileUpload, Models.FileUploadDto.AreaForUpdateDto>().ReverseMap();
        }
    }
}
