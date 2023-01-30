using AutoMapper;

namespace SupervisorMobility.API.MapperProfiles
{
    public class ProductDistributionProfile : Profile
    {
        public ProductDistributionProfile()
        {
            CreateMap<DataAccess.Entities.ProductDistribution, Models.ProductDistributionsDtos.ProductDistributionWithoutNavigationPropertiesDto>().ReverseMap();
            CreateMap<DataAccess.Entities.ProductDistribution, Models.ProductDistributionsDtos.ProductDistributionForCreationDto>().ReverseMap();
            CreateMap<DataAccess.Entities.ProductDistribution, Models.ProductDistributionsDtos.ProductDistributionForUpdateDto>().ReverseMap();
        }
    }
}
