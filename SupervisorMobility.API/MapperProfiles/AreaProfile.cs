using AutoMapper;

namespace SupervisorMobility.API.MapperProfiles
{
    public class AreaProfile : Profile
    {
        public AreaProfile()
        {
            CreateMap<DataAccess.Entities.Area, Models.AreaDtos.AreaWithJustOperationsDto>();
            CreateMap<DataAccess.Entities.Area, Models.AreaDtos.AreaWithoutNavigationPropertiesDto>().ReverseMap();
            CreateMap<DataAccess.Entities.Area, Models.AreaDtos.AreaForCreationDto>().ReverseMap();
            CreateMap<DataAccess.Entities.Area, Models.AreaDtos.AreaForUpdateDto>().ReverseMap();
            CreateMap<DataAccess.Entities.Area, Models.AreaDtos.GetAreaForHRIDto>().ReverseMap();
        }
    }
}
