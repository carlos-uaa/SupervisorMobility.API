using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.IS;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelSpecificationDtos;

namespace SupervisorMobility.API.MapperProfiles.ISMapperProfiles
{
    public class DataPanelSpecificationProfile : Profile
    {
        public DataPanelSpecificationProfile()
        {

            //data panel specification
            CreateMap<DataPanelSpecification, DataPanelSpecificationDto>().ReverseMap();
            CreateMap<DataPanelSpecification, DataPanelSpecificationForCreateDto>().ReverseMap();
            CreateMap<DataPanelSpecification, DataPanelSpecificationForUpdateDto>().ReverseMap();
            CreateMap<DataPanelSpecification, DataPanelSpecificationForUpdateSequenceDto>().ReverseMap();
        }
    }
}
