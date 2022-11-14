using AutoMapper;

namespace SupervisorMobility.API.MapperProfiles
{
    public class DistributionProfile : Profile
    {
        public DistributionProfile()
        {
            CreateMap<DataAccess.Entities.Distribution, Models.OperationDtos.DistributionWithoutNavigationPropertiesDto>().ReverseMap();
            CreateMap<DataAccess.Entities.Distribution, Models.OperationDtos.DistributionForCreationDto>().ReverseMap();
            CreateMap<DataAccess.Entities.Distribution, Models.OperationDtos.DistributionForUpdateDto>().ReverseMap();
        }
    }
}
