using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.IS;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelSpecificationDtos;

namespace SupervisorMobility.API.MapperProfiles.ISMapperProfiles
{
    public class DataPanelProfile : Profile
    {
        public DataPanelProfile()
        {
            //data panel
            CreateMap<DataPanel, DataPanelDto>().ReverseMap();
            CreateMap<DataPanel, DataPanelForCreateDto>().ReverseMap();
            CreateMap<DataPanel, DataPanelForUpdateDto>().ReverseMap();
            CreateMap<DataPanel, DataPanelForUpdateSequenceDto>().ReverseMap();

            //data panel specification
            CreateMap<DataPanelSpecification, DataPanelSpecificationDto>().ReverseMap();
        }
    }
}
