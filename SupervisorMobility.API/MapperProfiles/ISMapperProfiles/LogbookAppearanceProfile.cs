using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.IS;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.AppearanceDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelSpecificationDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.LogbookAppearanceDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.PartDtos;

namespace SupervisorMobility.API.MapperProfiles.ISMapperProfiles
{
    public class LogbookAppearanceProfile : Profile
    {
        public LogbookAppearanceProfile()
        {
            //data panel
            CreateMap<LogbookAppearance, LogbookAppearanceDto>().ReverseMap();
            CreateMap<LogbookAppearance, LogbookAppearanceForCreateDto>().ReverseMap();
            CreateMap<LogbookAppearance, LogbookAppearanceForUpdateDto>().ReverseMap();

         
        }
    }
}
