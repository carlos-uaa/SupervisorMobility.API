using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.RouteProductAssyChartDtos;

namespace SupervisorMobility.API.MapperProfiles
{
    public class RouteProductAssyChartProfile : Profile
    {
        public RouteProductAssyChartProfile(){        
            CreateMap<SOSCodePath, RouteProductAssyChartForCreationDto>().ReverseMap();
            CreateMap<RouteProductAssyChartWithOutNavigations, RouteProductAssyChartForCreationDto>().ReverseMap();
            CreateMap<RouteProductAssyChartForCreationDto, SOSCodePath>().ReverseMap();
            CreateMap<SOSCodePath, RouteProductAssyChartForUpdateDto>().ReverseMap();
            CreateMap<SOSCodePath, RouteProductAssyChartWithNavigations>().ReverseMap();
            CreateMap<SOSCodePath, RouteProductAssyChartWithOutNavigations>().ReverseMap();
            CreateMap<RouteProductAssyChartWithOutNavigations, SOSCodePath>().ReverseMap();
        }
    }
}
