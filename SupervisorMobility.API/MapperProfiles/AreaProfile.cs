using AutoMapper;
using SupervisorMobility.API.DataAccess;

namespace SupervisorMobility.API.MapperProfiles
{
    public class AreaProfile : Profile
    {
        public AreaProfile()
        {
            CreateMap<DataAccess.Entities.Area, Models.AreaDtos.AreaWithoutNavigationPropertiesDto>().ReverseMap();
            CreateMap<DataAccess.Entities.Area, Models.AreaDtos.AreaForCreationDto>().ReverseMap();
            CreateMap<DataAccess.Entities.Area, Models.AreaDtos.AreaForUpdateDto>().ReverseMap();
        }
    }
}
