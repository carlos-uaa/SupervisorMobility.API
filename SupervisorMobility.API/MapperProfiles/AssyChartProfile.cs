using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SupervisorMobility.API.MapperProfiles
{
    public class AssyChartProfile : Profile
    {
        public AssyChartProfile()
        {
            CreateMap<DataAccess.Entities.AssyChart, Models.AssyChart.AssyChartWithoutNavigationProperties>().ReverseMap();
            CreateMap<DataAccess.Entities.AssyChart, Models.AssyChart.AssyChartForCreationDto>().ReverseMap();
            CreateMap<DataAccess.Entities.AssyChart, Models.AssyChart.AssyChartForUpdateDto>().ReverseMap();
        }
    }
}
