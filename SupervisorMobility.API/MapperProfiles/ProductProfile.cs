using AutoMapper;

namespace SupervisorMobility.API.MapperProfiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<DataAccess.Entities.Product, Models.ProductDtos.ProductDto>();
            CreateMap<DataAccess.Entities.Product, Models.ProductDtos.ProductForCreationDto>().ReverseMap();
            CreateMap<DataAccess.Entities.Product, Models.ProductDtos.ProductForUpdateDto>().ReverseMap();
        }
    }
}
