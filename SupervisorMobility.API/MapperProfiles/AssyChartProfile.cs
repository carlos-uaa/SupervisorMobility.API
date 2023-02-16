using AutoMapper;

namespace SupervisorMobility.API.MapperProfiles
{
    public class AssyChartProfile : Profile
    {
        public AssyChartProfile()
        {
            CreateMap<Entities.AssyChart, Models.AssyChart.AssyChartWithoutNavigationProperties>().ReverseMap();
            CreateMap<Entities.AssyChart, Models.AssyChart.AssyChartForCreationRecived>().ReverseMap();
            CreateMap<Entities.AssyChart, Models.AssyChart.AssyChartForCreation>().ReverseMap();
            CreateMap<Entities.AssyChart, Models.AssyChart.AssyChartWhitInfo>().ReverseMap();
            CreateMap<Entities.AssyChart, Models.AssyChart.AssyChartForUpdateDto>().ReverseMap();
            CreateMap<Entities.AssyChart, Models.AssyChart.AssyChartDataToBulk>().ReverseMap();
        }
    }
}
