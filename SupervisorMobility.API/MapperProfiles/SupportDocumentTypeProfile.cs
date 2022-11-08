using AutoMapper;

namespace SupervisorMobility.API.MapperProfiles
{
    public class SupportDocumentTypeProfile : Profile
    {
        public SupportDocumentTypeProfile()
        {
            CreateMap<DataAccess.Entities.SupportDocumentType, Models.SupportDocumentTypeDtos.SupportDocumentTypeDto>();
            CreateMap<DataAccess.Entities.SupportDocumentType, Models.SupportDocumentTypeDtos.SupportDocumentTypeForCreationDto>().ReverseMap();
            CreateMap<DataAccess.Entities.SupportDocumentType, Models.SupportDocumentTypeDtos.SupportDocumentTypeForUpdateDto>().ReverseMap();
        }
    }
}
