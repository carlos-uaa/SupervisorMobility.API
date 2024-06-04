using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.IS;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.AppearanceDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelSpecificationDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.PartDtos;

namespace SupervisorMobility.API.MapperProfiles.ISMapperProfiles
{
    public class AppearanceProfile : Profile
    {
        public AppearanceProfile()
        {
            //data panel
            CreateMap<Appearance, AppearanceDto>().ReverseMap();
            CreateMap<Appearance, AppearanceForCreateDto>().ReverseMap();
            CreateMap<Appearance, AppearanceForUpdateDto>().ReverseMap();

         
        }
    }
}
