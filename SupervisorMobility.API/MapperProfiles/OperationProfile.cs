using AutoMapper;

namespace SupervisorMobility.API.MapperProfiles
{
    public class OperationProfile : Profile
    {
        public OperationProfile()
        {
            CreateMap<Entities.Operation, Models.OperationDtos.OperationWithoutNavigationPropertiesDto>().ReverseMap();
            CreateMap<Entities.Operation, Models.OperationDtos.OperationForCreationDto>().ReverseMap();
            CreateMap<Entities.Operation, Models.OperationDtos.OperationForUpdateDto>().ReverseMap();
        }
    }
}
