using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.RouteProductAssyChartDtos;

namespace SupervisorMobility.API.MapperProfiles
{
    public class RouteProductAssyChartProfile : Profile
    {
        public RouteProductAssyChartProfile(){        
            CreateMap<RouteProductAssyChart, RouteProductAssyChartForCreationDto>().ReverseMap();
            CreateMap<RouteProductAssyChartForCreationDto, RouteProductAssyChart>().ReverseMap();
            CreateMap<RouteProductAssyChart, RouteProductAssyChartForUpdateDto>().ReverseMap();
            CreateMap<RouteProductAssyChart, RouteProductAssyChartWithNavigations>().ReverseMap();
            CreateMap<RouteProductAssyChart, RouteProductAssyChartWithOutNavigations>().ReverseMap();
        }
    }
}
