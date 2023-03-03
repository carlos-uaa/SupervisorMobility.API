using AutoMapper;

namespace SupervisorMobility.API.MapperProfiles
{
    public class DistributionProfile : Profile
    {
        public DistributionProfile()
        {
            CreateMap<DataAccess.Entities.Distribution, Models.DistributionDtos.DistributionWithoutNavigationPropertiesDto>().ReverseMap();
            CreateMap<DataAccess.Entities.Distribution, Models.DistributionDtos.DistributionWithNavigationPropertiesDto>().ReverseMap();
            CreateMap<DataAccess.Entities.Distribution, Models.DistributionDtos.DistributionForCreationDto>().ReverseMap();
            CreateMap<DataAccess.Entities.Distribution, Models.DistributionDtos.DistributionForUpdateDto>().ReverseMap();
        }
    }
}
