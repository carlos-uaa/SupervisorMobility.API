using AutoMapper;

namespace SupervisorMobility.API.MapperProfiles
{
    public class ProductOperationProfile : Profile
    {
        public ProductOperationProfile()
        {
            CreateMap<DataAccess.Entities.ProductOperation, Models.ProductOperationDtos.ProductOperationWithoutNavigationPropertiesDto>().ReverseMap();
            CreateMap<DataAccess.Entities.ProductOperation, Models.ProductOperationDtos.ProductOperationForCreationDto>().ReverseMap();
            CreateMap<DataAccess.Entities.ProductOperation, Models.ProductOperationDtos.ProductOperationForUpdateDto>().ReverseMap();
        }
    }
}
