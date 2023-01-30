using AutoMapper;

namespace SupervisorMobility.API.MapperProfiles
{
    public class AssyChartProfile : Profile
    {
        public AssyChartProfile()
        {
            CreateMap<Entities.AssyChart, Models.AssyChart.AssyChartWithoutNavigationProperties>().ReverseMap();
            CreateMap<Entities.AssyChart, Models.AssyChart.AssyChartForCreationDto>().ReverseMap();
            CreateMap<Entities.AssyChart, Models.AssyChart.AssyChartForUpdateDto>().ReverseMap();
        }
    }
}
